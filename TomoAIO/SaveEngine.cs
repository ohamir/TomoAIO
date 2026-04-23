using System;

public class SaveEngine
{
    public static int GetDataOffset(byte[] saveFileBytes, string hashStr)
    {
        // Parse to bytes.
        byte[] hashBytes = new byte[hashStr.Length / 2];
        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashBytes[i] = Convert.ToByte(hashStr.Substring(i * 2, 2), 16);
        }

        // Hashes are stored in the table.
        Array.Reverse(hashBytes);

        for (int i = 0; i < saveFileBytes.Length - 8; i++)
        {
            if (saveFileBytes[i] == hashBytes[0] &&
                saveFileBytes[i + 1] == hashBytes[1] &&
                saveFileBytes[i + 2] == hashBytes[2] &&
                saveFileBytes[i + 3] == hashBytes[3])
            {
                return BitConverter.ToInt32(saveFileBytes, i + 4);
            }
        }

        // Not found.
        return -1;
    }
}