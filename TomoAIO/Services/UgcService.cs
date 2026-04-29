using TomoAIO.Infrastructure;
using TomoAIO.Models;

namespace TomoAIO.Services
{
    internal sealed class UgcService
    {
        private readonly FileSystemGateway _fs;
        private readonly ZstdCodec _zstd;

        public UgcService(FileSystemGateway fs, ZstdCodec zstd)
        {
            _fs = fs;
            _zstd = zstd;
        }

        public List<UgcFileItem> DiscoverUgcFiles(string ugcPath)
        {
            List<UgcFileItem> result = new();
            if (!_fs.DirectoryExists(ugcPath))
            {
                return result;
            }

            string[] files = _fs.GetFiles(ugcPath, "*.zs")
                .Where(file =>
                    file.EndsWith(".canvas.zs", StringComparison.OrdinalIgnoreCase) ||
                    (file.EndsWith(".ugctex.zs", StringComparison.OrdinalIgnoreCase) &&
                     Path.GetFileName(file).Contains("thumb", StringComparison.OrdinalIgnoreCase)))
                .OrderBy(Path.GetFileName)
                .ToArray();

            foreach (string fullPath in files)
            {
                string fileName = Path.GetFileName(fullPath);
                result.Add(new UgcFileItem
                {
                    FileName = fileName,
                    DisplayName = BuildDisplayName(fileName)
                });
            }

            return result;
        }

        public byte[] LoadAndDecompress(string fullPath)
        {
            byte[] fileBytes = _fs.ReadAllBytes(fullPath);
            return _zstd.Decompress(fileBytes);
        }

        public void ReplaceFromZs(string sourceZs, string targetPath)
        {
            _fs.CopyFile(sourceZs, targetPath, true);
        }

        public void WriteCompressed(string fullPath, byte[] decompressedBytes, int level = 9)
        {
            _fs.WriteAllBytes(fullPath, _zstd.Compress(decompressedBytes, level));
        }

        private static string BuildDisplayName(string fileName)
        {
            string display = fileName.EndsWith(".zs", StringComparison.OrdinalIgnoreCase)
                ? fileName[..^3]
                : fileName;

            if (display.EndsWith(".ugctex", StringComparison.OrdinalIgnoreCase))
            {
                display = display[..^(".ugctex".Length)];
            }
            else if (display.EndsWith(".canvas", StringComparison.OrdinalIgnoreCase))
            {
                display = display[..^(".canvas".Length)];
            }

            return display.Replace("thumb", "thumbnail", StringComparison.OrdinalIgnoreCase);
        }
    }
}
