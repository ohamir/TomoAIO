using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using ZstdSharp;

namespace TomoAIO.Services
{
    public sealed class UgcTextureService
    {
        public IReadOnlyList<string> GetCanvasFiles(string ugcPath)
        {
            if (!Directory.Exists(ugcPath))
            {
                return [];
            }

            return Directory.GetFiles(ugcPath, "*.canvas.zs")
                .Select(Path.GetFileName)
                .Where(static name => !string.IsNullOrWhiteSpace(name))
                .Cast<string>()
                .ToArray();
        }

        public (Bitmap? image, string infoText) BuildPreview(string filePath, string selectedFileName)
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);
            byte[] decompressed;
            using (var decompressor = new Decompressor())
            {
                decompressed = decompressor.Unwrap(fileBytes).ToArray();
            }

            if (selectedFileName.EndsWith(".canvas.zs", StringComparison.OrdinalIgnoreCase))
            {
                int size = (int)Math.Sqrt(decompressed.Length / 4d);
                return (DecodeRawTexture(decompressed, size, size), $"{selectedFileName} ({size}x{size} Decoded)");
            }

            if (selectedFileName.EndsWith(".ugctex.zs", StringComparison.OrdinalIgnoreCase))
            {
                int actualWidth = 256;
                if (decompressed.Length > 200000) actualWidth = 512;
                else if (decompressed.Length > 100000) actualWidth = 384;
                else if (decompressed.Length > 40000) actualWidth = 256;
                else if (decompressed.Length > 10000) actualWidth = 128;

                int actualHeight = actualWidth;
                Bitmap? image = DecodeCompressedAstc(decompressed, actualWidth, actualHeight);
                string info = image == null
                    ? $"{selectedFileName} (ASTC Decode Failed - Check astcenc.exe)"
                    : $"{selectedFileName} ({actualWidth}x{actualHeight} ASTC Decoded)";
                return (image, info);
            }

            return (null, $"{selectedFileName} (Unsupported format)");
        }

        public byte[] BuildCompressedCanvasPayload(string originalCanvasPath, string importedPngPath)
        {
            byte[] originalFileBytes = File.ReadAllBytes(originalCanvasPath);
            byte[] decompressedOriginal;
            using (var decompressor = new Decompressor())
            {
                decompressedOriginal = decompressor.Unwrap(originalFileBytes).ToArray();
            }

            int expectedSize = (int)Math.Sqrt(decompressedOriginal.Length / 4d);

            using var originalImport = new Bitmap(importedPngPath);
            using var resizedImage = new Bitmap(expectedSize, expectedSize, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(resizedImage))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.DrawImage(originalImport, new Rectangle(0, 0, expectedSize, expectedSize));
            }

            byte[] rawSwizzled = EncodeRawTexture(resizedImage);
            using var compressor = new Compressor(9);
            return compressor.Wrap(rawSwizzled).ToArray();
        }

        private static byte[] EncodeRawTexture(Bitmap bmp)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int bpp = 4;
            byte[] swizzledData = new byte[width * height * bpp];

            using var clone = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(clone))
            {
                g.DrawImage(bmp, new Rectangle(0, 0, width, height));
            }

            var rect = new Rectangle(0, 0, width, height);
            var bmpData = clone.LockBits(rect, ImageLockMode.ReadOnly, clone.PixelFormat);
            byte[] linearData = new byte[width * height * bpp];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, linearData, 0, linearData.Length);
            clone.UnlockBits(bmpData);

            int blockHeight = ResolveBlockHeight(height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int linearOffset = (y * width + x) * bpp;
                    int swizzledOffset = GetSwizzleOffset(x, y, width, bpp, blockHeight);
                    if (swizzledOffset + 3 >= swizzledData.Length)
                    {
                        continue;
                    }

                    swizzledData[swizzledOffset + 0] = linearData[linearOffset + 2];
                    swizzledData[swizzledOffset + 1] = linearData[linearOffset + 1];
                    swizzledData[swizzledOffset + 2] = linearData[linearOffset + 0];
                    swizzledData[swizzledOffset + 3] = linearData[linearOffset + 3];
                }
            }

            return swizzledData;
        }

        private static Bitmap DecodeRawTexture(byte[] rawData, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var rect = new Rectangle(0, 0, width, height);
            var bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);
            int bpp = 4;
            byte[] deswizzledData = new byte[width * height * bpp];
            int blockHeight = ResolveBlockHeight(height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int swizzledOffset = GetSwizzleOffset(x, y, width, bpp, blockHeight);
                    int linearOffset = (y * width + x) * bpp;
                    if (swizzledOffset + 3 >= rawData.Length)
                    {
                        continue;
                    }

                    deswizzledData[linearOffset + 0] = rawData[swizzledOffset + 2];
                    deswizzledData[linearOffset + 1] = rawData[swizzledOffset + 1];
                    deswizzledData[linearOffset + 2] = rawData[swizzledOffset + 0];
                    deswizzledData[linearOffset + 3] = rawData[swizzledOffset + 3];
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(deswizzledData, 0, bmpData.Scan0, deswizzledData.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        private static Bitmap? DecodeCompressedAstc(byte[] rawData, int width, int height)
        {
            int blockW = 4;
            int blockH = 4;
            int gridWidth = width / blockW;
            int gridHeight = height / blockH;
            int bytesPerBlock = 16;
            byte[] unswizzledData = new byte[gridWidth * gridHeight * bytesPerBlock];

            int swizzleBlockHeight = 1;
            while (swizzleBlockHeight * 8 < gridHeight && swizzleBlockHeight < 16)
            {
                swizzleBlockHeight *= 2;
            }

            int dataOffset = Math.Max(0, rawData.Length - unswizzledData.Length);
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    int swizzledOffset = GetSwizzleOffset(x, y, gridWidth, bytesPerBlock, swizzleBlockHeight);
                    int linearOffset = (y * gridWidth + x) * bytesPerBlock;
                    if (swizzledOffset + dataOffset + 15 >= rawData.Length)
                    {
                        continue;
                    }

                    Array.Copy(rawData, swizzledOffset + dataOffset, unswizzledData, linearOffset, bytesPerBlock);
                }
            }

            byte[] astcFile = new byte[16 + unswizzledData.Length];
            astcFile[0] = 0x13; astcFile[1] = 0xAB; astcFile[2] = 0xA1; astcFile[3] = 0x5C;
            astcFile[4] = (byte)blockW; astcFile[5] = (byte)blockH; astcFile[6] = 1;
            astcFile[7] = (byte)(width & 0xFF); astcFile[8] = (byte)((width >> 8) & 0xFF); astcFile[9] = 0;
            astcFile[10] = (byte)(height & 0xFF); astcFile[11] = (byte)((height >> 8) & 0xFF); astcFile[12] = 0;
            astcFile[13] = 1; astcFile[14] = 0; astcFile[15] = 0;
            Array.Copy(unswizzledData, 0, astcFile, 16, unswizzledData.Length);

            string tempAstc = Path.Combine(Path.GetTempPath(), $"tomoaio_{Guid.NewGuid():N}.astc");
            string tempPng = Path.ChangeExtension(tempAstc, ".png");
            try
            {
                File.WriteAllBytes(tempAstc, astcFile);
                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = "astcenc.exe",
                        Arguments = $"-dl \"{tempAstc}\" \"{tempPng}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                process.WaitForExit();
                if (!File.Exists(tempPng))
                {
                    return null;
                }

                using var tempImage = new Bitmap(tempPng);
                return new Bitmap(tempImage);
            }
            finally
            {
                if (File.Exists(tempPng)) File.Delete(tempPng);
                if (File.Exists(tempAstc)) File.Delete(tempAstc);
            }
        }

        private static int ResolveBlockHeight(int height)
        {
            if (height <= 16) return 1;
            if (height <= 32) return 2;
            if (height <= 64) return 4;
            if (height <= 128) return 8;
            return 16;
        }

        private static int GetSwizzleOffset(int x, int y, int width, int bpp, int blockHeight)
        {
            int widthInGobs = (width * bpp + 63) / 64;
            int xBytes = x * bpp;
            int gobAddr = (y / (8 * blockHeight)) * 512 * blockHeight * widthInGobs +
                          (xBytes / 64) * 512 * blockHeight +
                          (y % (8 * blockHeight) / 8) * 512;
            int innerGobAddr = ((xBytes % 64) / 32) * 256 +
                               ((y % 8) / 2) * 64 +
                               ((xBytes % 32) / 16) * 32 +
                               (y % 2) * 16 +
                               (xBytes % 16);
            return gobAddr + innerGobAddr;
        }
    }
}
