using System.Text;

namespace TomoAIO.Services
{
    public sealed class MiiSaveService
    {
        public byte[] BuildMiiPackage(byte[] miiBytes, int slot, IReadOnlyList<string> personalityHashes, string saveDir)
        {
            using MemoryStream ms = new();
            using BinaryWriter bw = new(ms);
            bw.Write((byte)3);
            bw.Write(new byte[] { 1, 1, 0 });

            int dO = SaveEngine.GetDataOffset(miiBytes, "881CA27A") + 4;
            bw.Write(miiBytes, dO + (slot * 156), 156);

            foreach (string hash in personalityHashes)
            {
                int offset = SaveEngine.GetDataOffset(miiBytes, hash) + 4;
                bw.Write(miiBytes, offset + (slot * 4), 4);
            }

            int nO = SaveEngine.GetDataOffset(miiBytes, "2499BFDA") + 4;
            int prO = SaveEngine.GetDataOffset(miiBytes, "3A5EDA05") + 4;
            bw.Write(miiBytes, nO + (slot * 64), 64);
            bw.Write(miiBytes, prO + (slot * 128), 128);

            int sxO = SaveEngine.GetDataOffset(miiBytes, "DFC82223") + 4;
            List<int> bits = DecodeBits(miiBytes.Skip(sxO).Take(27).ToArray());
            bw.Write(bits.Skip(slot * 3).Take(3).Select(static b => (byte)b).ToArray());
            bw.Write((byte)0);

            int fO = SaveEngine.GetDataOffset(miiBytes, "5E32ADF4") + 4;
            int faceID = miiBytes[fO + (slot * 4)];
            bw.Write(new byte[] { 0xA3, 0xA3, 0xA3, 0xA3 });
            if (faceID != 255)
            {
                string canvasPath = Path.Combine(saveDir, "Ugc", $"UgcFacePaint{faceID:D3}.canvas.zs");
                if (File.Exists(canvasPath)) bw.Write(File.ReadAllBytes(canvasPath));
            }

            bw.Write(new byte[] { 0xA4, 0xA4, 0xA4, 0xA4 });
            if (faceID != 255)
            {
                string texPath = Path.Combine(saveDir, "Ugc", $"UgcFacePaint{faceID:D3}.ugctex.zs");
                if (File.Exists(texPath)) bw.Write(File.ReadAllBytes(texPath));
            }

            return ms.ToArray();
        }

        public void ImportMii(string currentMiiSavPath, int slot, string filePath, IReadOnlyList<string> personalityHashes)
        {
            CreateSaveBackup(currentMiiSavPath);
            byte[] originalPkg = File.ReadAllBytes(filePath);
            List<byte> pkgList = originalPkg.ToList();
            if (pkgList[0] < 3)
            {
                pkgList.RemoveAt(4);
                if (pkgList[0] == 2)
                {
                    pkgList.Insert(427, 0);
                    int cIdx = FindMarker(pkgList.ToArray(), new byte[] { 0xA3, 0xA3, 0xA3 });
                    if (cIdx != -1) pkgList.Insert(cIdx + 3, 0xA3);
                    int uIdx = FindLastMarker(pkgList.ToArray(), new byte[] { 0xA3, 0xA3, 0xA3 });
                    if (uIdx != -1)
                    {
                        pkgList[uIdx] = 0xA4; pkgList[uIdx + 1] = 0xA4; pkgList[uIdx + 2] = 0xA4;
                        pkgList.Insert(uIdx + 3, 0xA4);
                    }
                }
            }

            byte[] pkg = pkgList.ToArray();
            byte[] miiBytes = File.ReadAllBytes(currentMiiSavPath);
            string? saveDir = Path.GetDirectoryName(currentMiiSavPath);
            if (string.IsNullOrWhiteSpace(saveDir))
            {
                throw new InvalidOperationException("Save folder path is invalid.");
            }

            string playerPath = Path.Combine(saveDir, "Player.sav");
            byte[] pBytes = File.ReadAllBytes(playerPath);
            int dnaO = SaveEngine.GetDataOffset(miiBytes, "881CA27A") + 4;

            using MemoryStream ms = new(pkg);
            using BinaryReader br = new(ms);
            br.ReadBytes(4);
            Array.Copy(br.ReadBytes(156), 0, miiBytes, dnaO + (slot * 156), 156);
            foreach (string h in personalityHashes)
                Array.Copy(br.ReadBytes(4), 0, miiBytes, SaveEngine.GetDataOffset(miiBytes, h) + 4 + (slot * 4), 4);
            Array.Copy(br.ReadBytes(64), 0, miiBytes, SaveEngine.GetDataOffset(miiBytes, "2499BFDA") + 4 + (slot * 64), 64);
            Array.Copy(br.ReadBytes(128), 0, miiBytes, SaveEngine.GetDataOffset(miiBytes, "3A5EDA05") + 4 + (slot * 128), 128);

            byte[] mySx = br.ReadBytes(3);
            br.ReadByte();
            int sxO = SaveEngine.GetDataOffset(miiBytes, "DFC82223") + 4;
            List<int> bits = DecodeBits(miiBytes.Skip(sxO).Take(27).ToArray());
            for (int i = 0; i < 3; i++) bits[(slot * 3) + i] = mySx[i];
            Array.Copy(EncodeBits(bits), 0, miiBytes, sxO, 27);

            int cS = FindMarker(pkg, new byte[] { 0xA3, 0xA3, 0xA3, 0xA3 }) + 4;
            int tS = FindMarker(pkg, new byte[] { 0xA4, 0xA4, 0xA4, 0xA4 }) + 4;
            int fO = SaveEngine.GetDataOffset(miiBytes, "5E32ADF4") + 4;
            int faceID = miiBytes[fO + (slot * 4)];
            int oldFaceID = faceID;
            int canvasSize = tS - 4 - cS;
            int texSize = pkg.Length - tS;

            if (cS > 3 && tS > cS && canvasSize > 0 && texSize > 0)
            {
                if (faceID == 255)
                {
                    for (int i = 0; i < 70; i++)
                    {
                        bool used = false;
                        for (int s = 0; s < 70; s++) if (miiBytes[fO + (s * 4)] == i) used = true;
                        if (!used) { faceID = i; break; }
                    }
                    if (faceID < 70) Array.Copy(new byte[] { (byte)faceID, 0, 0, 0 }, 0, miiBytes, fO + (slot * 4), 4);
                }

                if (faceID < 70)
                {
                    miiBytes[dnaO + (slot * 156) + 43] = 1;
                    UpdatePlayerRegistry(pBytes, faceID);
                    string ugcDir = Path.Combine(saveDir, "Ugc");
                    Directory.CreateDirectory(ugcDir);
                    File.WriteAllBytes(Path.Combine(ugcDir, $"UgcFacePaint{faceID:D3}.canvas.zs"), pkg.Skip(cS).Take(canvasSize).ToArray());
                    File.WriteAllBytes(Path.Combine(ugcDir, $"UgcFacePaint{faceID:D3}.ugctex.zs"), pkg.Skip(tS).ToArray());
                }
            }
            else
            {
                miiBytes[dnaO + (slot * 156) + 43] = 0;
                Array.Copy(new byte[] { 255, 255, 255, 255 }, 0, miiBytes, fO + (slot * 4), 4);
                if (oldFaceID != 255 && oldFaceID < 70) ClearPlayerRegistry(pBytes, oldFaceID);
            }

            File.WriteAllBytes(currentMiiSavPath, miiBytes);
            File.WriteAllBytes(playerPath, pBytes);
        }

        public IReadOnlyList<string> BuildMiiEntries(string currentMiiSavPath)
        {
            if (string.IsNullOrWhiteSpace(currentMiiSavPath) || !File.Exists(currentMiiSavPath))
            {
                return [];
            }

            byte[] miiBytes = File.ReadAllBytes(currentMiiSavPath);
            int nO = SaveEngine.GetDataOffset(miiBytes, "2499BFDA") + 4;
            int dO = SaveEngine.GetDataOffset(miiBytes, "881CA27A") + 4;
            List<string> entries = [];

            for (int i = 0; i < 70; i++)
            {
                if (miiBytes.Skip(dO + (i * 156)).Take(156).Sum(static b => (int)b) == 152) continue;
                byte[] nameB = new byte[64];
                Array.Copy(miiBytes, nO + (i * 64), nameB, 0, 64);
                entries.Add($"{i + 1}: {Encoding.Unicode.GetString(nameB).Replace("\0", string.Empty)}");
            }

            return entries;
        }

        private static void CreateSaveBackup(string currentMiiSavPath)
        {
            string? saveDir = Path.GetDirectoryName(currentMiiSavPath);
            if (string.IsNullOrWhiteSpace(saveDir) || !Directory.Exists(saveDir))
            {
                return;
            }

            string backupDir = Path.Combine(Application.StartupPath, "backup", DateTime.Now.ToString("dd_MM_yyyy_HHmmss"));
            Directory.CreateDirectory(backupDir);
            foreach (string file in Directory.GetFiles(saveDir))
                File.Copy(file, Path.Combine(backupDir, Path.GetFileName(file)));

            string ugcPath = Path.Combine(saveDir, "Ugc");
            if (!Directory.Exists(ugcPath))
            {
                return;
            }

            string backupUgc = Path.Combine(backupDir, "Ugc");
            Directory.CreateDirectory(backupUgc);
            foreach (string file in Directory.GetFiles(ugcPath))
                File.Copy(file, Path.Combine(backupUgc, Path.GetFileName(file)));
        }

        private static List<int> DecodeBits(byte[] data)
        {
            List<int> bits = [];
            foreach (byte b in data)
            {
                for (int i = 0; i < 8; i++) bits.Add((b >> i) & 1);
            }
            return bits;
        }

        private static byte[] EncodeBits(List<int> bits)
        {
            byte[] bytes = new byte[27];
            for (int i = 0; i < bits.Count; i++)
            {
                if (bits[i] == 1) bytes[i / 8] |= (byte)(1 << (i % 8));
            }
            return bytes;
        }

        private static int FindLastMarker(byte[] data, byte[] marker)
        {
            for (int i = data.Length - marker.Length; i >= 0; i--)
            {
                if (data.Skip(i).Take(marker.Length).SequenceEqual(marker)) return i;
            }
            return -1;
        }

        private static int FindMarker(byte[] data, byte[] marker)
        {
            for (int i = 0; i <= data.Length - marker.Length; i++)
            {
                if (data.Skip(i).Take(marker.Length).SequenceEqual(marker)) return i;
            }
            return -1;
        }

        private static void UpdatePlayerRegistry(byte[] pBytes, int faceID)
        {
            int fpP = SaveEngine.GetDataOffset(pBytes, "4C9819E4") + 4;
            int fpT = SaveEngine.GetDataOffset(pBytes, "DECC8954") + 4;
            int fpS = SaveEngine.GetDataOffset(pBytes, "23135BC5") + 4;
            int fpU = SaveEngine.GetDataOffset(pBytes, "FFC750B6") + 4;
            int fpH = SaveEngine.GetDataOffset(pBytes, "A56E42EC") + 4;
            Array.Copy(new byte[] { 0xF4, 0x01, 0x00, 0x00 }, 0, pBytes, fpP + (faceID * 4), 4);
            Array.Copy(new byte[] { 0x41, 0x49, 0x93, 0x56 }, 0, pBytes, fpT + (faceID * 4), 4);
            Array.Copy(new byte[] { 0xF4, 0xAD, 0x7F, 0x1D }, 0, pBytes, fpS + (faceID * 4), 4);
            Array.Copy(new byte[] { 0x00, 0x80, 0x00, 0x00 }, 0, pBytes, fpU + (faceID * 4), 4);
            Array.Copy(new byte[] { (byte)faceID, 0, 8, 0 }, 0, pBytes, fpH + (faceID * 4), 4);
        }

        private static void ClearPlayerRegistry(byte[] pBytes, int faceID)
        {
            int fpP = SaveEngine.GetDataOffset(pBytes, "4C9819E4") + 4;
            int fpT = SaveEngine.GetDataOffset(pBytes, "DECC8954") + 4;
            int fpS = SaveEngine.GetDataOffset(pBytes, "23135BC5") + 4;
            int fpU = SaveEngine.GetDataOffset(pBytes, "FFC750B6") + 4;
            int fpH = SaveEngine.GetDataOffset(pBytes, "A56E42EC") + 4;
            Array.Copy(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0, pBytes, fpP + (faceID * 4), 4);
            Array.Copy(new byte[] { 0x09, 0xDE, 0xEE, 0xB6 }, 0, pBytes, fpT + (faceID * 4), 4);
            Array.Copy(new byte[] { 0xA5, 0x8A, 0xFF, 0xAF }, 0, pBytes, fpS + (faceID * 4), 4);
            Array.Copy(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0, pBytes, fpU + (faceID * 4), 4);
            Array.Copy(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0, pBytes, fpH + (faceID * 4), 4);
        }
    }
}
