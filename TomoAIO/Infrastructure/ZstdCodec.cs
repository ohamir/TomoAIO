using ZstdSharp;

namespace TomoAIO.Infrastructure
{
    internal sealed class ZstdCodec
    {
        public byte[] Decompress(byte[] data)
        {
            using var decompressor = new Decompressor();
            return decompressor.Unwrap(data).ToArray();
        }

        public byte[] Compress(byte[] data, int level = 9)
        {
            using var compressor = new Compressor(level);
            return compressor.Wrap(data).ToArray();
        }
    }
}
