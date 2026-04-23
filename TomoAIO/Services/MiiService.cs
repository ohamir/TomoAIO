using System.Linq;
using TomoAIO.Models;

namespace TomoAIO.Services
{
    internal sealed class MiiService
    {
        public List<MiiEntry> BuildMiiEntries(byte[] miiBytes, int nameOffset, int dnaOffset)
        {
            List<MiiEntry> entries = new();
            for (int i = 0; i < 70; i++)
            {
                if (miiBytes.Skip(dnaOffset + (i * 156)).Take(156).Sum(b => (int)b) == 152)
                {
                    continue;
                }

                byte[] nameBytes = new byte[64];
                Array.Copy(miiBytes, nameOffset + (i * 64), nameBytes, 0, 64);
                entries.Add(new MiiEntry
                {
                    Slot = i,
                    Name = System.Text.Encoding.Unicode.GetString(nameBytes).Replace("\0", "")
                });
            }

            return entries;
        }
    }
}
