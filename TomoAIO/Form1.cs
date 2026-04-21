using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ZstdSharp;

namespace TomoAIO
{
    public partial class Form1 : Form
    {
        string currentMiiSavPath = "";
        string currentUgcPath = "";

        // The 18 Hash Markers for Personality, Voice, Gender, and Birthday
        string[] persHashes = {
            "43CD364F", "CD8DBAF8", "25B48224", "607BA160", "68E1134E", // Personality P1-P5
            "4913AE1A", "141EE086", "07B9D175", "81CF470A", "4D78E262", "FBC3FFB0", // Voice V1-V6
            "236E2D73", "F3C3DE59", "660C5247", // Gender (S1), Pronoun (S2), Style (S3)
            "5D7D3F45", "AB8AE08B", "2545E583", "6CF484F4" // Birthday B1-B4
        };

        public Form1()
        {
            InitializeComponent();
            ConfigureMenuButtons();
        }

        private void ConfigureMenuButtons()
        {
            // In WinForms, transparent buttons must share the same parent as the background image.
            button1.Parent = pictureBox1;
            button2.Parent = pictureBox1;
            logo.Parent = pictureBox1;
            logo.BackColor = Color.Transparent;

            ConfigureTransparentButton(button1);
            ConfigureTransparentButton(button2);
            MakePictureBackgroundTransparent(logo, Color.FromArgb(255, 190, 0), 38);
        }

        private void ConfigureTransparentButton(Button button)
        {
            button.UseVisualStyleBackColor = false;
            button.BackColor = Color.Transparent;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.Transparent;
            button.FlatAppearance.MouseDownBackColor = Color.Transparent;
            button.TabStop = false;
        }

        private void MakePictureBackgroundTransparent(PictureBox pictureBox, Color targetColor, int tolerance)
        {
            if (pictureBox.BackgroundImage is not Bitmap sourceBitmap)
            {
                return;
            }

            Bitmap transparentBitmap = new Bitmap(sourceBitmap);
            RemoveEdgeBackground(transparentBitmap, targetColor, tolerance);
            pictureBox.BackgroundImage = transparentBitmap;
        }

        private void RemoveEdgeBackground(Bitmap bitmap, Color targetColor, int tolerance)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            bool[,] visited = new bool[width, height];
            Queue<Point> queue = new Queue<Point>();

            void EnqueueIfMatch(int x, int y)
            {
                if (x < 0 || y < 0 || x >= width || y >= height || visited[x, y])
                {
                    return;
                }

                Color pixel = bitmap.GetPixel(x, y);
                if (IsCloseColor(pixel, targetColor, tolerance))
                {
                    visited[x, y] = true;
                    queue.Enqueue(new Point(x, y));
                }
            }

            for (int x = 0; x < width; x++)
            {
                EnqueueIfMatch(x, 0);
                EnqueueIfMatch(x, height - 1);
            }

            for (int y = 0; y < height; y++)
            {
                EnqueueIfMatch(0, y);
                EnqueueIfMatch(width - 1, y);
            }

            while (queue.Count > 0)
            {
                Point point = queue.Dequeue();
                bitmap.SetPixel(point.X, point.Y, Color.Transparent);

                EnqueueIfMatch(point.X + 1, point.Y);
                EnqueueIfMatch(point.X - 1, point.Y);
                EnqueueIfMatch(point.X, point.Y + 1);
                EnqueueIfMatch(point.X, point.Y - 1);
            }
        }

        private bool IsCloseColor(Color pixel, Color target, int tolerance)
        {
            return Math.Abs(pixel.R - target.R) <= tolerance &&
                   Math.Abs(pixel.G - target.G) <= tolerance &&
                   Math.Abs(pixel.B - target.B) <= tolerance;
        }

        // --- UGC EDITOR ENGINE (MadMax Logic) ---

        private void LoadUgcList(string exactUgcPath)
        {
            currentUgcPath = exactUgcPath;
            lstUGC.Items.Clear();

            if (Directory.Exists(exactUgcPath))
            {
                string[] files = Directory.GetFiles(exactUgcPath, "*.canvas.zs");

                if (files.Length == 0)
                {
                    // The error message you wanted
                    MessageBox.Show("Please make sure you selected the right folder", "Folder Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                foreach (string file in files)
                {
                    lstUGC.Items.Add(Path.GetFileName(file));
                }
            }
        }
        private byte[] EncodeRawTexture(Bitmap bmp)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int bpp = 4; // 4 bytes per pixel (RGBA)

            byte[] swizzledData = new byte[width * height * bpp];

            // Force the image into a 32-bit ARGB format just in case the user imports a 24-bit PNG
            Bitmap clone = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(clone))
            {
                g.DrawImage(bmp, new Rectangle(0, 0, width, height));
            }

            var rect = new Rectangle(0, 0, width, height);
            var bmpData = clone.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, clone.PixelFormat);

            byte[] linearData = new byte[width * height * bpp];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, linearData, 0, linearData.Length);
            clone.UnlockBits(bmpData);

            // Standard Tegra Block Height
            int blockHeight = 16;
            if (height <= 128) blockHeight = 8;
            if (height <= 64) blockHeight = 4;
            if (height <= 32) blockHeight = 2;
            if (height <= 16) blockHeight = 1;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int linearOffset = (y * width + x) * bpp;
                    int swizzledOffset = GetSwizzleOffset(x, y, width, bpp, blockHeight);

                    if (swizzledOffset + 3 < swizzledData.Length)
                    {
                        // Swap Windows BGRA back to Switch RGBA
                        swizzledData[swizzledOffset + 0] = linearData[linearOffset + 2]; // Red
                        swizzledData[swizzledOffset + 1] = linearData[linearOffset + 1]; // Green
                        swizzledData[swizzledOffset + 2] = linearData[linearOffset + 0]; // Blue
                        swizzledData[swizzledOffset + 3] = linearData[linearOffset + 3]; // Alpha
                    }
                }
            }

            clone.Dispose();
            return swizzledData;
        }

        private int GetSwizzleOffset(int x, int y, int width, int bpp, int blockHeight)
        {
            // 1. Calculate how many GOBs (Group of Bytes - 64 bytes wide) fit in one row
            int widthInGobs = (width * bpp + 63) / 64;

            // 2. Convert our X pixel coordinate to a Byte coordinate
            int xBytes = x * bpp;

            // 3. Find the address of the specific GOB this pixel belongs to
            int gobAddr = (y / (8 * blockHeight)) * 512 * blockHeight * widthInGobs +
                          (xBytes / 64) * 512 * blockHeight +
                          (y % (8 * blockHeight) / 8) * 512;

            // 4. Find the exact offset INSIDE the 64x8 GOB (This fixes the vertical slicing!)
            int innerGobAddr = ((xBytes % 64) / 32) * 256 +
                               ((y % 8) / 2) * 64 +
                               ((xBytes % 32) / 16) * 32 +
                               (y % 2) * 16 +
                               (xBytes % 16);

            return gobAddr + innerGobAddr;
        }

        // --- MII ENGINE (ALL ORIGINAL FEATURES) ---

        private int GetActualOffset(byte[] fileData, string hashHex)
        {
            byte[] hash = Enumerable.Range(0, hashHex.Length / 2)
                                    .Select(x => Convert.ToByte(hashHex.Substring(x * 2, 2), 16))
                                    .Reverse().ToArray(); // Little Endian

            for (int i = 0; i <= fileData.Length - 8; i++)
            {
                if (fileData.Skip(i).Take(4).SequenceEqual(hash))
                {
                    return BitConverter.ToInt32(fileData, i + 4);
                }
            }
            return -1;
        }
        private Bitmap DecodeRawTexture(byte[] rawData, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var rect = new Rectangle(0, 0, width, height);
            var bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, bmp.PixelFormat);

            int bpp = 4;
            byte[] deswizzledData = new byte[width * height * bpp];

            // Standard Nintendo Switch Tegra Block Heights for 4bpp (RGBA8)
            int blockHeight = 16;
            if (height <= 128) blockHeight = 8;
            if (height <= 64) blockHeight = 4;
            if (height <= 32) blockHeight = 2;
            if (height <= 16) blockHeight = 1;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int swizzledOffset = GetSwizzleOffset(x, y, width, bpp, blockHeight);
                    int linearOffset = (y * width + x) * bpp;

                    if (swizzledOffset + 3 < rawData.Length)
                    {
                        // Swap RGBA to BGRA
                        deswizzledData[linearOffset + 0] = rawData[swizzledOffset + 2]; // Blue
                        deswizzledData[linearOffset + 1] = rawData[swizzledOffset + 1]; // Green
                        deswizzledData[linearOffset + 2] = rawData[swizzledOffset + 0]; // Red
                        deswizzledData[linearOffset + 3] = rawData[swizzledOffset + 3]; // Alpha
                    }
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(deswizzledData, 0, bmpData.Scan0, deswizzledData.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        private void lstUGC_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstUGC.SelectedItem == null) return;
            string selectedFile = lstUGC.SelectedItem.ToString()!;
            string fullPath = Path.Combine(currentUgcPath, selectedFile);

            try
            {
                byte[] fileBytes = File.ReadAllBytes(fullPath);
                byte[] decompressed;
                using (var decompressor = new ZstdSharp.Decompressor())
                {
                    decompressed = decompressor.Unwrap(fileBytes).ToArray();
                }

                if (picPreview.Image != null) picPreview.Image.Dispose();

                // 1. Isolate the Canvas files (Raw RGBA, just swizzled)
                if (selectedFile.EndsWith(".canvas.zs"))
                {
                    int size = (int)Math.Sqrt(decompressed.Length / 4);
                    picPreview.Image = DecodeRawTexture(decompressed, size, size);
                    lblImageInfo.Text = $"{selectedFile} ({size}x{size} Decoded)";
                }
                // 2. Identify the Compressed files
                else if (selectedFile.EndsWith(".ugctex.zs"))
                {
                    // ASTC 4x4 uses ~1 byte per pixel. We check the file size to find the closest standard Switch resolution.
                    int actualWidth = 256; // Default fallback

                    if (decompressed.Length > 200000) actualWidth = 512;      // 512x512 = 262,144 bytes
                    else if (decompressed.Length > 100000) actualWidth = 384; // 384x384 = 147,456 bytes
                    else if (decompressed.Length > 40000) actualWidth = 256;  // 256x256 = 65,536 bytes
                    else if (decompressed.Length > 10000) actualWidth = 128;  // 128x128 = 16,384 bytes

                    int actualHeight = actualWidth;

                    picPreview.Image = DecodeCompressedAstc(decompressed, actualWidth, actualHeight);

                    if (picPreview.Image == null)
                    {
                        lblImageInfo.Text = $"{selectedFile} (ASTC Decode Failed - Check astcenc.exe)";
                    }
                    else
                    {
                        lblImageInfo.Text = $"{selectedFile} ({actualWidth}x{actualHeight} ASTC Decoded)";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private Bitmap DecodeCompressedAstc(byte[] rawData, int width, int height)
        {
            int blockW = 4;
            int blockH = 4;

            int gridWidth = width / blockW;
            int gridHeight = height / blockH;
            int bytesPerBlock = 16;

            byte[] unswizzledData = new byte[gridWidth * gridHeight * bytesPerBlock];

            // 1. The CORRECT Tegra X1 Block Height algorithm
            int swizzleBlockHeight = 1;
            while (swizzleBlockHeight * 8 < gridHeight && swizzleBlockHeight < 16)
            {
                swizzleBlockHeight *= 2;
            }

            // 2. Safety Offset: If the file is larger than the exact grid data, 
            // it means there's a header. The actual texture data is at the end.
            int dataOffset = rawData.Length - unswizzledData.Length;
            if (dataOffset < 0) dataOffset = 0;

            // 3. Unswizzle the Grid
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    int swizzledOffset = GetSwizzleOffset(x, y, gridWidth, bytesPerBlock, swizzleBlockHeight);
                    int linearOffset = (y * gridWidth + x) * bytesPerBlock;

                    if (swizzledOffset + dataOffset + 15 < rawData.Length)
                    {
                        Array.Copy(rawData, swizzledOffset + dataOffset, unswizzledData, linearOffset, bytesPerBlock);
                    }
                }
            }

            // 4. Build the official .astc file header
            byte[] astcFile = new byte[16 + unswizzledData.Length];
            astcFile[0] = 0x13; astcFile[1] = 0xAB; astcFile[2] = 0xA1; astcFile[3] = 0x5C;
            astcFile[4] = (byte)blockW; astcFile[5] = (byte)blockH; astcFile[6] = 1;
            astcFile[7] = (byte)(width & 0xFF); astcFile[8] = (byte)((width >> 8) & 0xFF); astcFile[9] = 0;
            astcFile[10] = (byte)(height & 0xFF); astcFile[11] = (byte)((height >> 8) & 0xFF); astcFile[12] = 0;
            astcFile[13] = 1; astcFile[14] = 0; astcFile[15] = 0;

            Array.Copy(unswizzledData, 0, astcFile, 16, unswizzledData.Length);

            // 5. Save temp files and run ARM's tool to convert to PNG!
            File.WriteAllBytes("temp.astc", astcFile);

            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "astcenc.exe";
            // Changed output to PNG to handle the transparent background properly
            process.StartInfo.Arguments = "-dl temp.astc temp.png";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();

            // 6. Load the image and clean up
            Bitmap finalBmp = null;
            if (File.Exists("temp.png"))
            {
                using (var tempImage = new Bitmap("temp.png"))
                {
                    finalBmp = new Bitmap(tempImage);
                }
                File.Delete("temp.png");
            }

            if (File.Exists("temp.astc")) File.Delete("temp.astc");

            return finalBmp;
        }
        private void ExportMii(int slot, string name)
        {
            SaveFileDialog sfd = new SaveFileDialog { Filter = "LtD Mii (*.ltd)|*.ltd", FileName = name + ".ltd" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                byte[] miiBytes = File.ReadAllBytes(currentMiiSavPath);
                string saveDir = Path.GetDirectoryName(currentMiiSavPath);

                using (MemoryStream ms = new MemoryStream())
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write((byte)3); bw.Write(new byte[] { 1, 1, 0 });
                    int dO = GetActualOffset(miiBytes, "881CA27A") + 4;
                    bw.Write(miiBytes, dO + (slot * 156), 156);

                    foreach (string hash in persHashes)
                    {
                        int offset = GetActualOffset(miiBytes, hash) + 4;
                        bw.Write(miiBytes, offset + (slot * 4), 4);
                    }

                    int nO = GetActualOffset(miiBytes, "2499BFDA") + 4;
                    int prO = GetActualOffset(miiBytes, "3A5EDA05") + 4;
                    bw.Write(miiBytes, nO + (slot * 64), 64);
                    bw.Write(miiBytes, prO + (slot * 128), 128);

                    int sxO = GetActualOffset(miiBytes, "DFC82223") + 4;
                    List<int> bits = DecodeSexuality(miiBytes.Skip(sxO).Take(27).ToArray());
                    bw.Write(bits.Skip(slot * 3).Take(3).Select(b => (byte)b).ToArray()); bw.Write((byte)0);

                    int fO = GetActualOffset(miiBytes, "5E32ADF4") + 4;
                    int faceID = miiBytes[fO + (slot * 4)];
                    bw.Write(new byte[] { 0xA3, 0xA3, 0xA3, 0xA3 });
                    if (faceID != 255)
                    {
                        string cP = Path.Combine(saveDir, "Ugc", $"UgcFacePaint{faceID:D3}.canvas.zs");
                        if (File.Exists(cP)) bw.Write(File.ReadAllBytes(cP));
                    }
                    bw.Write(new byte[] { 0xA4, 0xA4, 0xA4, 0xA4 });
                    if (faceID != 255)
                    {
                        string tP = Path.Combine(saveDir, "Ugc", $"UgcFacePaint{faceID:D3}.ugctex.zs");
                        if (File.Exists(tP)) bw.Write(File.ReadAllBytes(tP));
                    }
                    File.WriteAllBytes(sfd.FileName, ms.ToArray());
                }
                MessageBox.Show("Mii Identity fully backed up!");
            }
        }

        private void ImportMii(int slot, string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Please select a valid .ltd or .mii file in the text box first!", "Hold up!");
                return;
            }

            CreateSaveBackup();

            byte[] originalPkg = File.ReadAllBytes(filePath);

            // FIX 2: Version Compatibility (Handle v1 and v2 files!)
            List<byte> pkgList = originalPkg.ToList();
            if (pkgList[0] < 3)
            {
                pkgList.RemoveAt(4); // Remove the extra header byte
                if (pkgList[0] == 2)
                {
                    pkgList.Insert(427, 0); // Padding fix for v2

                    // Convert v2 marker "A3 A3 A3" to v3 marker "A3 A3 A3 A3"
                    int cIdx = FindMarker(pkgList.ToArray(), new byte[] { 0xA3, 0xA3, 0xA3 });
                    if (cIdx != -1) pkgList.Insert(cIdx + 3, 0xA3);

                    // Convert v2 marker for UGC (which was also A3 A3 A3) to "A4 A4 A4 A4"
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
            string saveDir = Path.GetDirectoryName(currentMiiSavPath);
            byte[] pBytes = File.ReadAllBytes(Path.Combine(saveDir, "Player.sav"));

            int dnaO = GetActualOffset(miiBytes, "881CA27A") + 4;

            using (MemoryStream ms = new MemoryStream(pkg))
            using (BinaryReader br = new BinaryReader(ms))
            {
                br.ReadBytes(4);
                Array.Copy(br.ReadBytes(156), 0, miiBytes, dnaO + (slot * 156), 156);
                foreach (string h in persHashes) Array.Copy(br.ReadBytes(4), 0, miiBytes, GetActualOffset(miiBytes, h) + 4 + (slot * 4), 4);
                Array.Copy(br.ReadBytes(64), 0, miiBytes, GetActualOffset(miiBytes, "2499BFDA") + 4 + (slot * 64), 64);
                Array.Copy(br.ReadBytes(128), 0, miiBytes, GetActualOffset(miiBytes, "3A5EDA05") + 4 + (slot * 128), 128);

                byte[] mySx = br.ReadBytes(3); br.ReadByte();
                int sxO = GetActualOffset(miiBytes, "DFC82223") + 4;
                List<int> bits = DecodeSexuality(miiBytes.Skip(sxO).Take(27).ToArray());
                for (int i = 0; i < 3; i++) bits[(slot * 3) + i] = mySx[i];
                Array.Copy(EncodeSexuality(bits), 0, miiBytes, sxO, 27);

                int cS = FindMarker(pkg, new byte[] { 0xA3, 0xA3, 0xA3, 0xA3 }) + 4;
                int tS = FindMarker(pkg, new byte[] { 0xA4, 0xA4, 0xA4, 0xA4 }) + 4;
                int fO = GetActualOffset(miiBytes, "5E32ADF4") + 4;

                // Track the original faceID in case we need to clear it!
                int faceID = miiBytes[fO + (slot * 4)];
                int oldFaceID = faceID;

                int canvasSize = tS - 4 - cS;
                int texSize = pkg.Length - tS;

                if (cS > 3 && tS > cS && canvasSize > 0 && texSize > 0)
                {
                    if (faceID == 255) // This translates to FF in the first byte
                    {
                        for (int i = 0; i < 70; i++)
                        {
                            bool used = false;
                            for (int s = 0; s < 70; s++) if (miiBytes[fO + (s * 4)] == i) used = true;
                            if (!used) { faceID = i; break; }
                        }

                        if (faceID < 70)
                        {
                            Array.Copy(new byte[] { (byte)faceID, 0, 0, 0 }, 0, miiBytes, fO + (slot * 4), 4);
                        }
                    }

                    if (faceID < 70)
                    {
                        miiBytes[dnaO + (slot * 156) + 43] = 1;
                        UpdatePlayerRegistry(pBytes, faceID);
                        string uD = Path.Combine(saveDir, "Ugc");
                        Directory.CreateDirectory(uD);
                        File.WriteAllBytes(Path.Combine(uD, $"UgcFacePaint{faceID:D3}.canvas.zs"), pkg.Skip(cS).Take(canvasSize).ToArray());
                        File.WriteAllBytes(Path.Combine(uD, $"UgcFacePaint{faceID:D3}.ugctex.zs"), pkg.Skip(tS).ToArray());
                    }
                }
                else
                {
                    miiBytes[dnaO + (slot * 156) + 43] = 0;

                    // FIX 1: Use proper 32-bit -1 value (FF FF FF FF)
                    Array.Copy(new byte[] { 255, 255, 255, 255 }, 0, miiBytes, fO + (slot * 4), 4);

                    // FIX 3: Clear old facepaint registry
                    if (oldFaceID != 255 && oldFaceID < 70)
                    {
                        ClearPlayerRegistry(pBytes, oldFaceID);
                    }
                }
            }

            File.WriteAllBytes(currentMiiSavPath, miiBytes);
            File.WriteAllBytes(Path.Combine(saveDir, "Player.sav"), pBytes);
            RefreshMiiList();
            MessageBox.Show("Mii Imported Successfully!");
        }

        // --- HELPERS ---

        private void UpdatePlayerRegistry(byte[] pBytes, int faceID)
        {
            int fpP = GetActualOffset(pBytes, "4C9819E4") + 4;
            int fpT = GetActualOffset(pBytes, "DECC8954") + 4;
            int fpS = GetActualOffset(pBytes, "23135BC5") + 4;
            int fpU = GetActualOffset(pBytes, "FFC750B6") + 4;
            int fpH = GetActualOffset(pBytes, "A56E42EC") + 4;
            Array.Copy(new byte[] { 0xF4, 0x01, 0x00, 0x00 }, 0, pBytes, fpP + (faceID * 4), 4);
            Array.Copy(new byte[] { 0x41, 0x49, 0x93, 0x56 }, 0, pBytes, fpT + (faceID * 4), 4);
            Array.Copy(new byte[] { 0xF4, 0xAD, 0x7F, 0x1D }, 0, pBytes, fpS + (faceID * 4), 4);
            Array.Copy(new byte[] { 0x00, 0x80, 0x00, 0x00 }, 0, pBytes, fpU + (faceID * 4), 4);
            Array.Copy(new byte[] { (byte)faceID, 0, 8, 0 }, 0, pBytes, fpH + (faceID * 4), 4);
        }

        private void CreateSaveBackup()
        {
            string sD = Path.GetDirectoryName(currentMiiSavPath);
            string bD = Path.Combine(Application.StartupPath, "backup", DateTime.Now.ToString("dd_MM_yyyy_HHmmss"));
            Directory.CreateDirectory(bD);
            foreach (string f in Directory.GetFiles(sD)) File.Copy(f, Path.Combine(bD, Path.GetFileName(f)));
            if (Directory.Exists(Path.Combine(sD, "Ugc")))
            {
                Directory.CreateDirectory(Path.Combine(bD, "Ugc"));
                foreach (string f in Directory.GetFiles(Path.Combine(sD, "Ugc"))) File.Copy(f, Path.Combine(bD, "Ugc", Path.GetFileName(f)));
            }
        }

        private List<int> DecodeSexuality(byte[] data) { List<int> bits = new List<int>(); foreach (byte b in data) { for (int i = 0; i < 8; i++) bits.Add((b >> i) & 1); } return bits; }
        private byte[] EncodeSexuality(List<int> bits) { byte[] b = new byte[27]; for (int i = 0; i < bits.Count; i++) { if (bits[i] == 1) b[i / 8] |= (byte)(1 << (i % 8)); } return b; }
        private int FindLastMarker(byte[] data, byte[] marker)
        {
            for (int i = data.Length - marker.Length; i >= 0; i--)
            {
                if (data.Skip(i).Take(marker.Length).SequenceEqual(marker)) return i;
            }
            return -1;
        }
        private int FindMarker(byte[] data, byte[] marker)
        {
            for (int i = 0; i <= data.Length - marker.Length; i++)
            {
                if (data.Skip(i).Take(marker.Length).SequenceEqual(marker)) return i;
            }
            return -1;
        }
        private void ClearPlayerRegistry(byte[] pBytes, int faceID)
        {
            int fpP = GetActualOffset(pBytes, "4C9819E4") + 4;
            int fpT = GetActualOffset(pBytes, "DECC8954") + 4;
            int fpS = GetActualOffset(pBytes, "23135BC5") + 4;
            int fpU = GetActualOffset(pBytes, "FFC750B6") + 4;
            int fpH = GetActualOffset(pBytes, "A56E42EC") + 4;

            // Standard "empty" values used by the game 
            Array.Copy(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0, pBytes, fpP + (faceID * 4), 4);
            Array.Copy(new byte[] { 0x09, 0xDE, 0xEE, 0xB6 }, 0, pBytes, fpT + (faceID * 4), 4);
            Array.Copy(new byte[] { 0xA5, 0x8A, 0xFF, 0xAF }, 0, pBytes, fpS + (faceID * 4), 4);
            Array.Copy(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0, pBytes, fpU + (faceID * 4), 4);
            Array.Copy(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0, pBytes, fpH + (faceID * 4), 4);
        }
        private void RefreshMiiList()
        {
            if (string.IsNullOrEmpty(currentMiiSavPath) || !File.Exists(currentMiiSavPath)) return;
            byte[] miiBytes = File.ReadAllBytes(currentMiiSavPath);
            int nO = GetActualOffset(miiBytes, "2499BFDA") + 4;
            int dO = GetActualOffset(miiBytes, "881CA27A") + 4;
            listBox1.Items.Clear();
            for (int i = 0; i < 70; i++)
            {
                if (miiBytes.Skip(dO + (i * 156)).Take(156).Sum(b => (int)b) == 152) continue;
                byte[] nameB = new byte[64]; Array.Copy(miiBytes, nO + (i * 64), nameB, 0, 64);
                listBox1.Items.Add($"{i + 1}: {System.Text.Encoding.Unicode.GetString(nameB).Replace("\0", "")}");
            }
        }

        // --- NAVIGATION & DESIGNER HANDSHAKES ---

        private void button1_Click(object sender, EventArgs e) { panel1.Visible = true; panel1.BringToFront(); }
        private void button3_Click(object sender, EventArgs e) { panel1.Visible = false; }
        private void button2_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select your Living the Dream 'Ugc' folder";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    // 1. Try to load the list first
                    LoadUgcList(fbd.SelectedPath);

                    // 2. Only show the panel if the folder actually had the right files!
                    if (lstUGC.Items.Count > 0)
                    {
                        panelUGC.Visible = true;
                        panelUGC.BringToFront();
                    }
                    // If the list is empty, the panel remains hidden and 
                    // the user stays on the main menu.
                }
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fb = new FolderBrowserDialog())
            {
                if (fb.ShowDialog() == DialogResult.OK)
                {
                    currentMiiSavPath = Path.Combine(fb.SelectedPath, "Mii.sav");
                    RefreshMiiList();
                    MessageBox.Show("Save file found!", "TomoAIO");
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null || cmbMiiAction.SelectedItem == null)
            {
                MessageBox.Show("Please select a Mii and an action!", "Missing Information");
                return;
            }

            string sel = listBox1.SelectedItem.ToString();
            int slot = int.Parse(sel.Split(':')[0]) - 1;
            string name = sel.Split(':')[1].Trim();

            if (cmbMiiAction.SelectedItem.ToString().Contains("Export"))
            {
                ExportMii(slot, name);
            }
            else
            {
                // Pass the file path from the text box directly into the importer!
                ImportMii(slot, txtMiiPath.Text);
            }
        }

        private void btnBrowseMii_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Mii Data Files (*.mii;*.ltd;*.sav)|*.mii;*.ltd;*.sav";
                if (ofd.ShowDialog() == DialogResult.OK) txtMiiPath.Text = ofd.FileName;
            }
        }

        // Empty stubs for designer compatibility
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void Form1_Load(object sender, EventArgs e) { }
        private void Form1_Shown(object sender, EventArgs e) { }
        private void pictureBox3_Click(object sender, EventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void txtMiiPath_TextChanged(object sender, EventArgs e) { }
        private void txtMiiPath_DragEnter(object sender, DragEventArgs e) { if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy; }
        private void txtMiiPath_DragDrop(object sender, DragEventArgs e) { string[] f = (string[])e.Data.GetData(DataFormats.FileDrop); if (f.Length > 0) txtMiiPath.Text = f[0]; }
        private void label2_Click(object sender, EventArgs e) { }
        private void btnUgcExport_Click(object sender, EventArgs e)
        {
            if (lstUGC.SelectedItem == null) return;
            using (SaveFileDialog sfd = new SaveFileDialog() { FileName = lstUGC.SelectedItem.ToString() })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.Copy(Path.Combine(currentUgcPath, lstUGC.SelectedItem.ToString()), sfd.FileName, true);
                    MessageBox.Show("Exported Successfully!");
                }
            }
        }

        private void lblImageInfo_Click(object sender, EventArgs e) { }

        private void btnUgcImport_Click(object sender, EventArgs e)
        {
            if (lstUGC.SelectedItem == null) return;

            string selectedFile = lstUGC.SelectedItem.ToString()!;
            if (!selectedFile.EndsWith(".canvas.zs"))
            {
                MessageBox.Show("Please select a .canvas.zs file to replace.");
                return;
            }

            string fullPath = Path.Combine(currentUgcPath, selectedFile);

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "PNG Images (*.png)|*.png";
                ofd.Title = "Select custom texture to import";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // 1. Read the original file to get the exact required dimensions
                        byte[] originalFileBytes = File.ReadAllBytes(fullPath);
                        byte[] decompressedOriginal;
                        using (var decompressor = new ZstdSharp.Decompressor())
                        {
                            decompressedOriginal = decompressor.Unwrap(originalFileBytes).ToArray();
                        }

                        int expectedSize = (int)Math.Sqrt(decompressedOriginal.Length / 4);

                        // 2. Load the user's new PNG image and automatically RESIZE it!
                        Bitmap originalImport = new Bitmap(ofd.FileName);
                        Bitmap resizedImage = new Bitmap(expectedSize, expectedSize, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                        using (Graphics g = Graphics.FromImage(resizedImage))
                        {
                            // Use high-quality resizing to keep the image looking crisp
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                            // Draw the imported image onto our perfectly-sized canvas
                            g.DrawImage(originalImport, new Rectangle(0, 0, expectedSize, expectedSize));
                        }

                        // 3. Swizzle the newly resized image into a Switch-compatible byte array
                        byte[] rawSwizzled = EncodeRawTexture(resizedImage);

                        // Memory cleanup
                        originalImport.Dispose();
                        resizedImage.Dispose();

                        // 4. Compress the swizzled bytes using Zstandard (Level 9 is safe/efficient)
                        byte[] compressedData;
                        using (var compressor = new ZstdSharp.Compressor(9))
                        {
                            compressedData = compressor.Wrap(rawSwizzled).ToArray();
                        }

                        // 5. Overwrite the original .canvas.zs file!
                        File.WriteAllBytes(fullPath, compressedData);
                        MessageBox.Show("Custom texture successfully injected!", "Success");

                        // 6. Refresh the preview to show the new custom image
                        lstUGC_SelectedIndexChanged(null, EventArgs.Empty);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Import Error: " + ex.Message);
                    }
                }
            }
        }

        private void btnUgcBack_Click(object sender, EventArgs e)
        {
            // 1. Clear the image preview and memory
            if (picPreview.Image != null)
            {
                picPreview.Image.Dispose();
                picPreview.Image = null;
            }
            lblImageInfo.Text = "Waiting for selection...";

            // 2. Simply hide the UGC Editor panel to reveal the main menu underneath!
            panelUGC.Visible = false;

            // (Notice we completely removed the references to panel1 here)
        }
    }
}