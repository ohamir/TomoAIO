using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TomoAIO.Infrastructure;
using TomoAIO.Models;

namespace TomoAIO.Services
{
    public class IslandService
    {
        private readonly SaveFileRepository _repo;

        public IslandService(SaveFileRepository repo)
        {
            _repo = repo;
        }

        // ─── Read ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Reads the island name and current balance from the player save file.
        /// Returns null if the file does not exist.
        /// </summary>
        public IslandData? LoadIslandData(string playerSavPath)
        {
            if (!_repo.FileExists(playerSavPath)) return null;

            byte[] data = _repo.ReadFile(playerSavPath);
            var result = new IslandData();

            // Balance (stored as integer cents)
            int totalCents = _repo.FindHashOffset(data, "365FAB1F");
            if (totalCents >= 0)
                result.BalanceDollars = (decimal)totalCents / 100;

            // Island name (UTF-16 LE, null-terminated, max 64 bytes)
            int nameOffset = _repo.FindHashOffset(data, "D46DF986");
            if (nameOffset > 0)
            {
                byte[] nameBytes = data.Skip(nameOffset).Take(64).ToArray();
                string raw = Encoding.Unicode.GetString(nameBytes);
                int nullIdx = raw.IndexOf('\0');
                result.IslandName = nullIdx >= 0 ? raw.Substring(0, nullIdx) : raw;
            }

            return result;
        }

        // ─── Write ────────────────────────────────────────────────────────────

        /// <summary>
        /// Writes a new balance (in dollars) to the player save file.
        /// Throws InvalidOperationException if the money hash is not found.
        /// </summary>
        public void SaveMoney(string playerSavPath, decimal dollars)
        {
            if (!_repo.FileExists(playerSavPath))
                throw new FileNotFoundException("Player save file not found.", playerSavPath);

            byte[] data = _repo.ReadFile(playerSavPath);

            int idx = _repo.FindHashIndex(data, "365FAB1F");
            if (idx == -1)
                throw new InvalidOperationException("Money hash (365FAB1F) not found in save file.");

            int cents = (int)(dollars * 100);
            Array.Copy(BitConverter.GetBytes(cents), 0, data, idx + 4, 4);
            _repo.WriteFile(playerSavPath, data);
        }

        // ─── Unlock ───────────────────────────────────────────────────────────

        /// <summary>
        /// Unlocks items in the given category.
        /// arrayHashes: list of hash keys whose arrays should be set to "obtained".
        /// unlockRooms: if true, reads room_hashes.csv and unlocks each room.
        /// Returns true if the file was modified.
        /// Throws FileNotFoundException if room_hashes.csv is missing when unlockRooms is true.
        /// </summary>
        public bool UnlockCategory(string playerSavPath, string[]? arrayHashes, bool unlockRooms)
        {
            if (!_repo.FileExists(playerSavPath))
                throw new FileNotFoundException("Player.sav not found.", playerSavPath);

            byte[] data = _repo.ReadFile(playerSavPath);
            byte[] obtainedState = { 0x7C, 0xEF, 0x5F, 0xBC };
            bool modified = false;

            // ── Array-based items (clothes, food, quik builds, etc.) ──────────
            if (arrayHashes != null)
            {
                foreach (string hash in arrayHashes)
                {
                    int offset = _repo.FindHashOffset(data, hash);
                    if (offset <= 0 || offset >= data.Length - 4) continue;

                    int count = BitConverter.ToInt32(data, offset);
                    int start = offset + 4;

                    if (count > 0 && start + (count * 4) <= data.Length)
                    {
                        for (int j = 0; j < count; j++)
                            Array.Copy(obtainedState, 0, data, start + (j * 4), 4);
                        modified = true;
                    }
                }
            }

            // ── Room unlocks via CSV ──────────────────────────────────────────
            if (unlockRooms)
            {
                string csvPath = Path.Combine(
                    Application.StartupPath, "services", "room_hashes.csv");

                if (!_repo.FileExists(csvPath))
                    throw new FileNotFoundException(
                        "'services\\room_hashes.csv' was not found.", csvPath);

                foreach (string line in _repo.ReadAllLines(csvPath))
                {
                    string h = line.Trim();
                    if (h.Equals("Hash", StringComparison.OrdinalIgnoreCase) || h.Length != 8)
                        continue;

                    byte[] hBytes = Enumerable.Range(0, 4)
                        .Select(x => Convert.ToByte(h.Substring(x * 2, 2), 16))
                        .Reverse()
                        .ToArray();

                    for (int i = 0; i <= data.Length - 8; i++)
                    {
                        if (data.Skip(i).Take(4).SequenceEqual(hBytes))
                        {
                            Array.Copy(obtainedState, 0, data, i + 4, 4);
                            modified = true;
                            break;
                        }
                    }
                }
            }

            if (modified)
                _repo.WriteFile(playerSavPath, data);

            return modified;
        }
    }
}