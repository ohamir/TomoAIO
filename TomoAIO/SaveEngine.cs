using System;

public class SaveEngine
{
    // This is our C# version of the offsetLocator!
    public static int GetDataOffset(byte[] saveFileBytes, string hashStr)
    {
        // 1. Convert the hash string (like "2499BFDA") into bytes
        byte[] hashBytes = new byte[hashStr.Length / 2];
        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashBytes[i] = Convert.ToByte(hashStr.Substring(i * 2, 2), 16);
        }

        // 2. Reverse it because Switch uses "Little Endian" format
        Array.Reverse(hashBytes);

        // 3. Search the save file for this specific hash
        for (int i = 0; i < saveFileBytes.Length - 8; i++)
        {
            if (saveFileBytes[i] == hashBytes[0] &&
                saveFileBytes[i + 1] == hashBytes[1] &&
                saveFileBytes[i + 2] == hashBytes[2] &&
                saveFileBytes[i + 3] == hashBytes[3])
            {
                // 4. We found the hash! The actual offset is the next 4 bytes.
                return BitConverter.ToInt32(saveFileBytes, i + 4);
            }
        }

        // Return -1 if something went wrong and we couldn't find it
        return -1;
    }
}