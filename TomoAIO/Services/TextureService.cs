using System.Drawing;
using System.Drawing.Imaging;

namespace TomoAIO.Services
{
    internal sealed class TextureService
    {
        public byte[] EncodeThumbnailBc3Swizzled(Bitmap sourceImage, int thumbSize = 256, bool convertSrgbToLinear = true, int swizzleBlockHeight = 8)
        {
            using Bitmap resized = new(thumbSize, thumbSize, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(resized))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawImage(sourceImage, new Rectangle(0, 0, thumbSize, thumbSize), 0, 0, sourceImage.Width, sourceImage.Height, GraphicsUnit.Pixel);
            }

            byte[] bgra = ExtractBgraBytes(resized);
            byte[] rgba = BgraToRgba(bgra);
            if (convertSrgbToLinear)
            {
                for (int i = 0; i < rgba.Length; i += 4)
                {
                    rgba[i + 0] = SrgbToLinearByte(rgba[i + 0]);
                    rgba[i + 1] = SrgbToLinearByte(rgba[i + 1]);
                    rgba[i + 2] = SrgbToLinearByte(rgba[i + 2]);
                }
            }

            byte[] linearBc3 = Bc3EncodeLikeToolkit(rgba, thumbSize, thumbSize);
            int gridW = thumbSize / 4;
            int gridH = thumbSize / 4;
            byte[] swizzled = new byte[linearBc3.Length];
            const int bytesPerBlock = 16;

            for (int y = 0; y < gridH; y++)
            {
                for (int x = 0; x < gridW; x++)
                {
                    int linearOffset = (y * gridW + x) * bytesPerBlock;
                    int swizzledOffset = GetSwizzleOffset(x, y, gridW, bytesPerBlock, swizzleBlockHeight);
                    if (swizzledOffset + (bytesPerBlock - 1) >= swizzled.Length)
                    {
                        continue;
                    }

                    Array.Copy(linearBc3, linearOffset, swizzled, swizzledOffset, bytesPerBlock);
                }
            }

            return swizzled;
        }

        public byte[] EncodeRawTexture(Bitmap bmp, bool convertSrgbToLinear = false)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int bpp = 4;
            byte[] swizzledData = new byte[width * height * bpp];

            using Bitmap clone = new(width, height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(clone))
            {
                g.DrawImage(bmp, new Rectangle(0, 0, width, height));
            }

            Rectangle rect = new(0, 0, width, height);
            BitmapData bmpData = clone.LockBits(rect, ImageLockMode.ReadOnly, clone.PixelFormat);
            byte[] linearData = new byte[width * height * bpp];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, linearData, 0, linearData.Length);
            clone.UnlockBits(bmpData);

            int blockHeight = GetBlockHeightForRgba(height);
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

                    byte b = linearData[linearOffset + 0];
                    byte gChan = linearData[linearOffset + 1];
                    byte r = linearData[linearOffset + 2];
                    byte a = linearData[linearOffset + 3];

                    if (convertSrgbToLinear)
                    {
                        r = SrgbToLinearByte(r);
                        gChan = SrgbToLinearByte(gChan);
                        b = SrgbToLinearByte(b);
                    }

                    swizzledData[swizzledOffset + 0] = r;
                    swizzledData[swizzledOffset + 1] = gChan;
                    swizzledData[swizzledOffset + 2] = b;
                    swizzledData[swizzledOffset + 3] = a;
                }
            }

            return swizzledData;
        }

        public Bitmap DecodeRawTexture(byte[] rawData, int width, int height)
        {
            Bitmap bmp = new(width, height, PixelFormat.Format32bppArgb);
            Rectangle rect = new(0, 0, width, height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

            int bpp = 4;
            byte[] deswizzledData = new byte[width * height * bpp];
            int blockHeight = GetBlockHeightForRgba(height);

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

        public Bitmap? DecodeBc3SwizzledTexture(byte[] rawData, int width, int height, int blockHeight)
        {
            if (width <= 0 || height <= 0 || width % 4 != 0 || height % 4 != 0)
            {
                return null;
            }

            int gridWidth = width / 4;
            int gridHeight = height / 4;
            int bytesPerBlock = 16;
            int expectedLength = gridWidth * gridHeight * bytesPerBlock;
            int dataOffset = Math.Max(0, rawData.Length - expectedLength);
            byte[] linearBc3 = new byte[expectedLength];

            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    int swizzledOffset = GetSwizzleOffset(x, y, gridWidth, bytesPerBlock, blockHeight);
                    int linearOffset = (y * gridWidth + x) * bytesPerBlock;
                    int sourceOffset = swizzledOffset + dataOffset;

                    if (sourceOffset + (bytesPerBlock - 1) < rawData.Length && linearOffset + (bytesPerBlock - 1) < linearBc3.Length)
                    {
                        Array.Copy(rawData, sourceOffset, linearBc3, linearOffset, bytesPerBlock);
                    }
                }
            }

            byte[] bgra = DecodeBc3LinearToBgra(linearBc3, width, height);
            Bitmap bmp = new(width, height, PixelFormat.Format32bppArgb);
            Rectangle rect = new(0, 0, width, height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);
            System.Runtime.InteropServices.Marshal.Copy(bgra, 0, bmpData.Scan0, bgra.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        public int GetSwizzleOffset(int x, int y, int width, int bpp, int blockHeight)
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

        private static int GetBlockHeightForRgba(int height)
        {
            int blockHeight = 16;
            if (height <= 128) blockHeight = 8;
            if (height <= 64) blockHeight = 4;
            if (height <= 32) blockHeight = 2;
            if (height <= 16) blockHeight = 1;
            return blockHeight;
        }

        private static byte SrgbToLinearByte(byte srgb)
        {
            float s = srgb / 255f;
            float linear = (s <= 0.04045f) ? (s / 12.92f) : (float)Math.Pow((s + 0.055f) / 1.055f, 2.4f);
            int value = (int)Math.Round(linear * 255f);
            return (byte)Math.Clamp(value, 0, 255);
        }

        private static byte[] ExtractBgraBytes(Bitmap bmp)
        {
            Rectangle rect = new(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);
            try
            {
                byte[] bytes = new byte[bmp.Width * bmp.Height * 4];
                System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, bytes, 0, bytes.Length);
                return bytes;
            }
            finally
            {
                bmp.UnlockBits(bmpData);
            }
        }

        private static byte[] BgraToRgba(byte[] bgra)
        {
            byte[] rgba = new byte[bgra.Length];
            for (int i = 0; i < bgra.Length; i += 4)
            {
                rgba[i + 0] = bgra[i + 2];
                rgba[i + 1] = bgra[i + 1];
                rgba[i + 2] = bgra[i + 0];
                rgba[i + 3] = bgra[i + 3];
            }

            return rgba;
        }

        private static byte[] Bc3EncodeLikeToolkit(byte[] rgba, int texWidth, int texHeight)
        {
            int blocksX = texWidth / 4;
            int blocksY = texHeight / 4;
            byte[] output = new byte[blocksX * blocksY * 16];
            byte[] block = new byte[64];

            for (int by = 0; by < blocksY; by++)
            {
                for (int bx = 0; bx < blocksX; bx++)
                {
                    for (int row = 0; row < 4; row++)
                    {
                        for (int col = 0; col < 4; col++)
                        {
                            int px = bx * 4 + col;
                            int py = by * 4 + row;
                            int src = (py * texWidth + px) * 4;
                            int dst = (row * 4 + col) * 4;
                            block[dst] = rgba[src];
                            block[dst + 1] = rgba[src + 1];
                            block[dst + 2] = rgba[src + 2];
                            block[dst + 3] = rgba[src + 3];
                        }
                    }

                    Bc3EncodeBlockLikeToolkit(block, output, (by * blocksX + bx) * 16);
                }
            }

            return output;
        }

        private static void Bc3EncodeBlockLikeToolkit(byte[] block, byte[] output, int outOffset)
        {
            int minA = 255;
            int maxA = 0;
            for (int i = 0; i < 16; i++)
            {
                int a = block[i * 4 + 3];
                if (a < minA) minA = a;
                if (a > maxA) maxA = a;
            }

            byte a0;
            byte a1;
            if (minA == maxA)
            {
                a0 = (byte)maxA;
                a1 = (byte)maxA;
            }
            else
            {
                a0 = (byte)maxA;
                a1 = (byte)minA;
            }

            output[outOffset] = a0;
            output[outOffset + 1] = a1;

            int[] alphaPal = new int[8];
            alphaPal[0] = a0;
            alphaPal[1] = a1;
            if (a0 > a1)
            {
                alphaPal[2] = (6 * a0 + 1 * a1) / 7;
                alphaPal[3] = (5 * a0 + 2 * a1) / 7;
                alphaPal[4] = (4 * a0 + 3 * a1) / 7;
                alphaPal[5] = (3 * a0 + 4 * a1) / 7;
                alphaPal[6] = (2 * a0 + 5 * a1) / 7;
                alphaPal[7] = (1 * a0 + 6 * a1) / 7;
            }
            else
            {
                alphaPal[2] = a0;
                alphaPal[3] = a0;
                alphaPal[4] = a0;
                alphaPal[5] = a0;
                alphaPal[6] = 0;
                alphaPal[7] = 255;
            }

            ulong alphaIndexBits = 0;
            for (int i = 0; i < 16; i++)
            {
                int bestIdx = 0;
                int bestDist = int.MaxValue;
                for (int k = 0; k < 8; k++)
                {
                    int d = Math.Abs(block[i * 4 + 3] - alphaPal[k]);
                    if (d < bestDist)
                    {
                        bestDist = d;
                        bestIdx = k;
                    }
                }

                alphaIndexBits |= ((ulong)bestIdx & 0x7UL) << (i * 3);
            }

            for (int i = 0; i < 6; i++)
            {
                output[outOffset + 2 + i] = (byte)((alphaIndexBits >> (8 * i)) & 0xFF);
            }

            int minR = 255, minG = 255, minB = 255;
            int maxR = 0, maxG = 0, maxB = 0;
            for (int i = 0; i < 16; i++)
            {
                int off = i * 4;
                int r = block[off], g = block[off + 1], b = block[off + 2];
                if (r < minR) minR = r;
                if (g < minG) minG = g;
                if (b < minB) minB = b;
                if (r > maxR) maxR = r;
                if (g > maxG) maxG = g;
                if (b > maxB) maxB = b;
            }

            ushort color0 = PackRgb565((byte)maxR, (byte)maxG, (byte)maxB);
            ushort color1 = PackRgb565((byte)minR, (byte)minG, (byte)minB);
            if (color0 < color1)
            {
                (color0, color1) = (color1, color0);
            }
            if (color0 == color1)
            {
                if (color0 < 0xFFFF) color0++;
                else color1--;
            }

            var c0 = DecodeRgb565(color0);
            var c1 = DecodeRgb565(color1);
            int pr2 = (2 * c0.r + c1.r) / 3;
            int pg2 = (2 * c0.g + c1.g) / 3;
            int pb2 = (2 * c0.b + c1.b) / 3;
            int pr3 = (c0.r + 2 * c1.r) / 3;
            int pg3 = (c0.g + 2 * c1.g) / 3;
            int pb3 = (c0.b + 2 * c1.b) / 3;

            uint colorIndexBits = 0;
            for (int i = 0; i < 16; i++)
            {
                int off = i * 4;
                int r = block[off];
                int g = block[off + 1];
                int b = block[off + 2];

                int bestIdx = 0;
                int bestDist = ColorDistSq(r, g, b, c0.r, c0.g, c0.b);
                int d1 = ColorDistSq(r, g, b, c1.r, c1.g, c1.b);
                int d2 = ColorDistSq(r, g, b, pr2, pg2, pb2);
                int d3 = ColorDistSq(r, g, b, pr3, pg3, pb3);
                if (d1 < bestDist) { bestDist = d1; bestIdx = 1; }
                if (d2 < bestDist) { bestDist = d2; bestIdx = 2; }
                if (d3 < bestDist) { bestIdx = 3; }

                colorIndexBits |= ((uint)bestIdx & 0x3u) << (i * 2);
            }

            output[outOffset + 8] = (byte)(color0 & 0xFF);
            output[outOffset + 9] = (byte)((color0 >> 8) & 0xFF);
            output[outOffset + 10] = (byte)(color1 & 0xFF);
            output[outOffset + 11] = (byte)((color1 >> 8) & 0xFF);
            output[outOffset + 12] = (byte)(colorIndexBits & 0xFF);
            output[outOffset + 13] = (byte)((colorIndexBits >> 8) & 0xFF);
            output[outOffset + 14] = (byte)((colorIndexBits >> 16) & 0xFF);
            output[outOffset + 15] = (byte)((colorIndexBits >> 24) & 0xFF);
        }

        private static ushort PackRgb565(byte r, byte g, byte b)
        {
            int r5 = (r * 31 + 127) / 255;
            int g6 = (g * 63 + 127) / 255;
            int b5 = (b * 31 + 127) / 255;
            return (ushort)((r5 << 11) | (g6 << 5) | b5);
        }

        private static int ColorDistSq(int r1, int g1, int b1, int r2, int g2, int b2)
        {
            int dr = r1 - r2;
            int dg = g1 - g2;
            int db = b1 - b2;
            return dr * dr + dg * dg + db * db;
        }

        private static byte[] DecodeBc3LinearToBgra(byte[] bc3Blocks, int width, int height)
        {
            byte[] output = new byte[width * height * 4];
            int blocksX = width / 4;
            int blocksY = height / 4;

            for (int by = 0; by < blocksY; by++)
            {
                for (int bx = 0; bx < blocksX; bx++)
                {
                    int blockOffset = (by * blocksX + bx) * 16;
                    if (blockOffset + 15 >= bc3Blocks.Length)
                    {
                        continue;
                    }

                    byte alpha0 = bc3Blocks[blockOffset + 0];
                    byte alpha1 = bc3Blocks[blockOffset + 1];
                    ulong alphaBits = 0;
                    for (int i = 0; i < 6; i++)
                    {
                        alphaBits |= ((ulong)bc3Blocks[blockOffset + 2 + i]) << (8 * i);
                    }

                    byte[] alphaTable = BuildBc3AlphaTable(alpha0, alpha1);

                    ushort color0 = BitConverter.ToUInt16(bc3Blocks, blockOffset + 8);
                    ushort color1 = BitConverter.ToUInt16(bc3Blocks, blockOffset + 10);
                    uint colorBits = BitConverter.ToUInt32(bc3Blocks, blockOffset + 12);

                    var c0 = DecodeRgb565(color0);
                    var c1 = DecodeRgb565(color1);
                    byte[,] colors = new byte[4, 3];
                    colors[0, 0] = c0.r; colors[0, 1] = c0.g; colors[0, 2] = c0.b;
                    colors[1, 0] = c1.r; colors[1, 1] = c1.g; colors[1, 2] = c1.b;
                    colors[2, 0] = (byte)((2 * c0.r + c1.r) / 3);
                    colors[2, 1] = (byte)((2 * c0.g + c1.g) / 3);
                    colors[2, 2] = (byte)((2 * c0.b + c1.b) / 3);
                    colors[3, 0] = (byte)((c0.r + 2 * c1.r) / 3);
                    colors[3, 1] = (byte)((c0.g + 2 * c1.g) / 3);
                    colors[3, 2] = (byte)((c0.b + 2 * c1.b) / 3);

                    for (int py = 0; py < 4; py++)
                    {
                        for (int px = 0; px < 4; px++)
                        {
                            int pixelIndex = py * 4 + px;
                            int colorIndex = (int)((colorBits >> (2 * pixelIndex)) & 0x3);
                            int alphaIndex = (int)((alphaBits >> (3 * pixelIndex)) & 0x7);

                            int x = bx * 4 + px;
                            int y = by * 4 + py;
                            int dst = (y * width + x) * 4;
                            output[dst + 0] = colors[colorIndex, 2];
                            output[dst + 1] = colors[colorIndex, 1];
                            output[dst + 2] = colors[colorIndex, 0];
                            output[dst + 3] = alphaTable[alphaIndex];
                        }
                    }
                }
            }

            return output;
        }

        private static byte[] BuildBc3AlphaTable(byte alpha0, byte alpha1)
        {
            byte[] table = new byte[8];
            table[0] = alpha0;
            table[1] = alpha1;

            if (alpha0 > alpha1)
            {
                table[2] = (byte)((6 * alpha0 + alpha1) / 7);
                table[3] = (byte)((5 * alpha0 + 2 * alpha1) / 7);
                table[4] = (byte)((4 * alpha0 + 3 * alpha1) / 7);
                table[5] = (byte)((3 * alpha0 + 4 * alpha1) / 7);
                table[6] = (byte)((2 * alpha0 + 5 * alpha1) / 7);
                table[7] = (byte)((alpha0 + 6 * alpha1) / 7);
            }
            else
            {
                table[2] = (byte)((4 * alpha0 + alpha1) / 5);
                table[3] = (byte)((3 * alpha0 + 2 * alpha1) / 5);
                table[4] = (byte)((2 * alpha0 + 3 * alpha1) / 5);
                table[5] = (byte)((alpha0 + 4 * alpha1) / 5);
                table[6] = 0;
                table[7] = 255;
            }

            return table;
        }

        private static (byte r, byte g, byte b) DecodeRgb565(ushort value)
        {
            int r5 = (value >> 11) & 0x1F;
            int g6 = (value >> 5) & 0x3F;
            int b5 = value & 0x1F;

            byte r = (byte)((r5 * 255 + 15) / 31);
            byte g = (byte)((g6 * 255 + 31) / 63);
            byte b = (byte)((b5 * 255 + 15) / 31);
            return (r, g, b);
        }
    }
}