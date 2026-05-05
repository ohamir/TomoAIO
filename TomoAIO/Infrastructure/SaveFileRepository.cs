using System;
using System.IO;
using System.Linq;

namespace TomoAIO.Infrastructure
{
    public class SaveFileRepository
    {
        public byte[] ReadFile(string path) => File.ReadAllBytes(path);

        public void WriteFile(string path, byte[] data) => File.WriteAllBytes(path, data);

        public bool FileExists(string path) => File.Exists(path);

        public string[] ReadAllLines(string path) => File.ReadAllLines(path);

        /// <summary>
        /// Searches for a 4-byte reversed hash in the file and returns the
        /// int32 value stored immediately after it. Returns -1 if not found.
        /// </summary>
        public int FindHashOffset(byte[] data, string hashHex)
        {
            byte[] hash = Enumerable.Range(0, hashHex.Length / 2)
                .Select(x => Convert.ToByte(hashHex.Substring(x * 2, 2), 16))
                .Reverse()
                .ToArray();

            for (int i = 0; i <= data.Length - 8; i++)
                if (data.Skip(i).Take(4).SequenceEqual(hash))
                    return BitConverter.ToInt32(data, i + 4);

            return -1;
        }

        /// <summary>
        /// Searches for a 4-byte reversed hash and returns the raw byte index
        /// of the match (not the value after it). Returns -1 if not found.
        /// </summary>
        public int FindHashIndex(byte[] data, string hashHex)
        {
            byte[] hash = Enumerable.Range(0, hashHex.Length / 2)
                .Select(x => Convert.ToByte(hashHex.Substring(x * 2, 2), 16))
                .Reverse()
                .ToArray();

            for (int i = 0; i <= data.Length - 8; i++)
                if (data.Skip(i).Take(4).SequenceEqual(hash))
                    return i;

            return -1;
        }
    }
}