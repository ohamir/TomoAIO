using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows.Forms;
using TomoAIO.Infrastructure;
using TomoAIO.Models;
using TomoAIO.Services;

namespace TomoAIO
{
    public partial class MiiImportForm : Form
    {
        private readonly AppState _state;
        private readonly MiiService _miiService = new();


        private readonly string[] _miiActions = { "Import Mii (.ltd)", "Export Mii (.ltd)" };

        // Personality / voice / gender / birthday hashes — same order as original
        private readonly string[] persHashes =
        {
            "43CD364F", "CD8DBAF8", "25B48224", "607BA160", "68E1134E",   // Personality P1-P5
            "4913AE1A", "141EE086", "07B9D175", "81CF470A", "4D78E262", "FBC3FFB0", // Voice V1-V6
            "236E2D73", "F3C3DE59", "660C5247",                            // Gender/Pronoun/Style
            "5D7D3F45", "AB8AE08B", "2545E583", "6CF484F4"                 // Birthday B1-B4
        };

        // ─── Constructor ──────────────────────────────────────────────────────

        public MiiImportForm(AppState state)
        {
            _state = state;
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            EnableDoubleBuffer(DisplayMiiLstBox);
            ApplyStyleToButtons();
        }

        // ─── Setup ────────────────────────────────────────────────────────────

        public void ApplyStyleToButtons()
        {
            ApplyButtonStyle(LoadSaveBtn);
            ApplyButtonStyle(BrowseBtn);
            ApplyButtonStyle(ApplyChangesBtn);
        }

        public void ApplyButtonStyle(Button btn, bool isPrimary = false)
        {
            // Define our Color Palettes
            // Secondary (Load/Browse) - Dark Navy/Charcoal
            Color secMain = Color.FromArgb(44, 62, 80);
            Color secHover = Color.FromArgb(52, 73, 94);
            Color secClick = Color.FromArgb(31, 46, 61);

            // Primary (Apply) - Game-Pop Teal
            Color priMain = Color.FromArgb(19, 141, 117);
            Color priHover = Color.FromArgb(22, 160, 133);
            Color priClick = Color.FromArgb(14, 102, 85);

            // Choose the palette based on the parameter
            Color mainColor = isPrimary ? priMain : secMain;
            Color hoverColor = isPrimary ? priHover : secHover;
            Color clickColor = isPrimary ? priClick : secClick;

            // 1. Basic Visual Setup
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = mainColor;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI Semibold", 12f);

            // Create space for the "3D" movement effect
            btn.Padding = new Padding(0, 0, 0, 4);

            // 2. Hover Effect
            btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = mainColor;

            // 3. The "Push" Animation
            btn.MouseDown += (s, e) => {
                btn.BackColor = clickColor;
                // This shifts the text down by 2 pixels to simulate a physical press
                btn.Padding = new Padding(0, 2, 0, 0);
            };

            btn.MouseUp += (s, e) => {
                btn.BackColor = hoverColor;
                btn.Padding = new Padding(0, 0, 0, 4);
            };

            // Optional: Add round corners (Requires using System.Drawing.Drawing2D)
            GraphicsPath path = new GraphicsPath();
            int r = 15; 
            path.AddArc(0, 0, r, r, 180, 90);
            path.AddArc(btn.Width - r, 0, r, r, 270, 90);
            path.AddArc(btn.Width - r, btn.Height - r, r, r, 0, 90);
            path.AddArc(0, btn.Height - r, r, r, 90, 90);
            btn.Region = new Region(path);
        }


        private static void EnableDoubleBuffer(Control control)
        {
            typeof(Control)
                .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)?
                .SetValue(control, true, null);
        }
     
        private void SetSelectedMiiPath(string path)
        {
            _state.SelectedMiiPath = path;
            if (PathToSaveTxtBox != null) PathToSaveTxtBox.Text = path;
        }

        
        // ─── Core Logic ───────────────────────────────────────────────────────

        private int GetActualOffset(byte[] fileData, string hashHex)
        {
            byte[] hash = Enumerable.Range(0, hashHex.Length / 2)
                .Select(x => Convert.ToByte(hashHex.Substring(x * 2, 2), 16))
                .Reverse().ToArray();

            for (int i = 0; i <= fileData.Length - 8; i++)
                if (fileData.Skip(i).Take(4).SequenceEqual(hash))
                    return BitConverter.ToInt32(fileData, i + 4);

            return -1;
        }

        private void RefreshMiiList()
        {
            if (string.IsNullOrEmpty(_state.CurrentMiiSavPath) ||
                !File.Exists(_state.CurrentMiiSavPath)) return;

            byte[] miiBytes = File.ReadAllBytes(_state.CurrentMiiSavPath);
            int nO = GetActualOffset(miiBytes, "2499BFDA") + 4;
            int dO = GetActualOffset(miiBytes, "881CA27A") + 4;

            DisplayMiiLstBox.Items.Clear();
            List<MiiEntry> entries = _miiService.BuildMiiEntries(miiBytes, nO, dO);
            foreach (MiiEntry entry in entries)
                DisplayMiiLstBox.Items.Add(entry.ToString());
        }

        private void ExportMii(int slot, string name)
        {
            using var sfd = new SaveFileDialog { Filter = "LtD Mii (*.ltd)|*.ltd", FileName = name + ".ltd" };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            byte[] miiBytes = File.ReadAllBytes(_state.CurrentMiiSavPath);
            string? saveDir = Path.GetDirectoryName(_state.CurrentMiiSavPath);
            if (string.IsNullOrWhiteSpace(saveDir))
            {
                MessageBox.Show("Save folder path is invalid.", "TomoAIO");
                return;
            }

            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            bw.Write((byte)3);
            bw.Write(new byte[] { 1, 1, 0 });

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
            bw.Write(bits.Skip(slot * 3).Take(3).Select(b => (byte)b).ToArray());
            bw.Write((byte)0);

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
            MessageBox.Show("Mii Identity fully backed up!");
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
                        pkgList[uIdx] = 0xA4;
                        pkgList[uIdx + 1] = 0xA4;
                        pkgList[uIdx + 2] = 0xA4;
                        pkgList.Insert(uIdx + 3, 0xA4);
                    }
                }
            }

            byte[] pkg = pkgList.ToArray();
            byte[] miiBytes = File.ReadAllBytes(_state.CurrentMiiSavPath);
            string? saveDir = Path.GetDirectoryName(_state.CurrentMiiSavPath);
            if (string.IsNullOrWhiteSpace(saveDir))
            {
                MessageBox.Show("Save folder path is invalid.", "TomoAIO");
                return;
            }

            byte[] pBytes = File.ReadAllBytes(Path.Combine(saveDir, "Player.sav"));
            int dnaO = GetActualOffset(miiBytes, "881CA27A") + 4;

            using (var ms = new MemoryStream(pkg))
            using (var br = new BinaryReader(ms))
            {
                br.ReadBytes(4);
                Array.Copy(br.ReadBytes(156), 0, miiBytes, dnaO + (slot * 156), 156);

                foreach (string h in persHashes)
                    Array.Copy(br.ReadBytes(4), 0, miiBytes, GetActualOffset(miiBytes, h) + 4 + (slot * 4), 4);

                // Name
                int nameOffset = GetActualOffset(miiBytes, "2499BFDA") + 4 + (slot * 64);
                byte[] rawName = br.ReadBytes(64);
                Array.Clear(miiBytes, nameOffset, 64);
                int validNameLen = 64;
                for (int i = 0; i < 63; i += 2)
                {
                    if (rawName[i] == 0 && rawName[i + 1] == 0) { validNameLen = i + 2; break; }
                }
                Array.Copy(rawName, 0, miiBytes, nameOffset, validNameLen);

                // Creator
                int creatorOffset = GetActualOffset(miiBytes, "3A5EDA05") + 4 + (slot * 128);
                byte[] rawCreator = br.ReadBytes(128);
                Array.Clear(miiBytes, creatorOffset, 128);
                int validCreatorLen = 128;
                for (int i = 0; i < 127; i += 2)
                {
                    if (rawCreator[i] == 0 && rawCreator[i + 1] == 0) { validCreatorLen = i + 2; break; }
                }
                Array.Copy(rawCreator, 0, miiBytes, creatorOffset, validCreatorLen);

                // Sexuality bits
                byte[] mySx = br.ReadBytes(3); br.ReadByte();
                int sxO = GetActualOffset(miiBytes, "DFC82223") + 4;
                List<int> bits = DecodeSexuality(miiBytes.Skip(sxO).Take(27).ToArray());
                for (int i = 0; i < 3; i++) bits[(slot * 3) + i] = mySx[i];
                Array.Copy(EncodeSexuality(bits), 0, miiBytes, sxO, 27);

                // Face paint
                int cS = FindMarker(pkg, new byte[] { 0xA3, 0xA3, 0xA3, 0xA3 }) + 4;
                int tS = FindMarker(pkg, new byte[] { 0xA4, 0xA4, 0xA4, 0xA4 }) + 4;
                int fO = GetActualOffset(miiBytes, "5E32ADF4") + 4;
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
                            for (int s = 0; s < 70; s++)
                                if (miiBytes[fO + (s * 4)] == i) used = true;
                            if (!used) { faceID = i; break; }
                        }
                        if (faceID < 70)
                            Array.Copy(new byte[] { (byte)faceID, 0, 0, 0 }, 0, miiBytes, fO + (slot * 4), 4);
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
                    Array.Copy(new byte[] { 255, 255, 255, 255 }, 0, miiBytes, fO + (slot * 4), 4);
                    if (oldFaceID != 255 && oldFaceID < 70)
                        ClearPlayerRegistry(pBytes, oldFaceID);
                }
            }

            File.WriteAllBytes(_state.CurrentMiiSavPath, miiBytes);
            File.WriteAllBytes(Path.Combine(saveDir, "Player.sav"), pBytes);
            RefreshMiiList();
            MessageBox.Show("Mii Imported Successfully!");
        }

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

        private void ClearPlayerRegistry(byte[] pBytes, int faceID)
        {
            int fpP = GetActualOffset(pBytes, "4C9819E4") + 4;
            int fpT = GetActualOffset(pBytes, "DECC8954") + 4;
            int fpS = GetActualOffset(pBytes, "23135BC5") + 4;
            int fpU = GetActualOffset(pBytes, "FFC750B6") + 4;
            int fpH = GetActualOffset(pBytes, "A56E42EC") + 4;
            Array.Copy(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0, pBytes, fpP + (faceID * 4), 4);
            Array.Copy(new byte[] { 0x09, 0xDE, 0xEE, 0xB6 }, 0, pBytes, fpT + (faceID * 4), 4);
            Array.Copy(new byte[] { 0xA5, 0x8A, 0xFF, 0xAF }, 0, pBytes, fpS + (faceID * 4), 4);
            Array.Copy(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0, pBytes, fpU + (faceID * 4), 4);
            Array.Copy(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0, pBytes, fpH + (faceID * 4), 4);
        }

        private void CreateSaveBackup()
        {
            string? sD = Path.GetDirectoryName(_state.CurrentMiiSavPath);
            if (string.IsNullOrWhiteSpace(sD) || !Directory.Exists(sD)) return;

            string bD = Path.Combine(Application.StartupPath, "backup", DateTime.Now.ToString("dd_MM_yyyy_HHmmss"));
            Directory.CreateDirectory(bD);

            foreach (string file in new[] { "Map.sav", "Mii.sav", "Player.sav" })
            {
                string src = Path.Combine(sD, file);
                if (File.Exists(src)) File.Copy(src, Path.Combine(bD, file), true);
            }

            string srcUgc = Path.Combine(sD, "Ugc");
            if (Directory.Exists(srcUgc))
            {
                string dstUgc = Path.Combine(bD, "Ugc");
                Directory.CreateDirectory(dstUgc);
                foreach (string f in Directory.GetFiles(srcUgc))
                    File.Copy(f, Path.Combine(dstUgc, Path.GetFileName(f)), true);
            }
        }

        private List<int> DecodeSexuality(byte[] data)
        {
            var bits = new List<int>();
            foreach (byte b in data)
                for (int i = 0; i < 8; i++) bits.Add((b >> i) & 1);
            return bits;
        }

        private byte[] EncodeSexuality(List<int> bits)
        {
            byte[] b = new byte[27];
            for (int i = 0; i < bits.Count; i++)
                if (bits[i] == 1) b[i / 8] |= (byte)(1 << (i % 8));
            return b;
        }

        private int FindMarker(byte[] data, byte[] marker)
        {
            for (int i = 0; i <= data.Length - marker.Length; i++)
                if (data.Skip(i).Take(marker.Length).SequenceEqual(marker)) return i;
            return -1;
        }

        private int FindLastMarker(byte[] data, byte[] marker)
        {
            for (int i = data.Length - marker.Length; i >= 0; i--)
                if (data.Skip(i).Take(marker.Length).SequenceEqual(marker)) return i;
            return -1;
        }


        // Drag-and-drop support on the path display
        private void MiiImportForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void MiiImportForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data?.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
                SetSelectedMiiPath(files[0]);
        }


        #region Buttons

        private void BrowseBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Mii Data Files (*.mii;*.ltd;*.sav)|*.mii;*.ltd;*.sav";
                if (ofd.ShowDialog() == DialogResult.OK) SetSelectedMiiPath(ofd.FileName);
            }
        }


        private void ApplyChangesBtn_Click(object sender, EventArgs e)
        {
            string selectedAction = SelectActionComboBox.SelectedItem?.ToString() ?? "";

            if (DisplayMiiLstBox.SelectedItem == null || string.IsNullOrWhiteSpace(selectedAction))
            {
                MessageBox.Show("Please select a Mii and an action!", "Missing Information");
                return;
            }

            string? sel = DisplayMiiLstBox.SelectedItem.ToString();
            if (string.IsNullOrWhiteSpace(sel) || !sel.Contains(':'))
            {
                MessageBox.Show("Selected Mii entry is invalid.", "TomoAIO");
                return;
            }

            int slot = int.Parse(sel.Split(':')[0]) - 1;
            string name = sel.Split(':')[1].Trim();


            if (selectedAction.Contains("Export"))
                ExportMii(slot, name);
            else
                ImportMii(slot, _state.SelectedMiiPath);
        }

        private void LoadSaveBtn_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog
            {
                Description = "Select your game save folder (where Mii.sav is located)"
            };
            if (fbd.ShowDialog() != DialogResult.OK) return;

            string miiSavPath = Path.Combine(fbd.SelectedPath, "Mii.sav");
            if (File.Exists(miiSavPath))
            {
                _state.CurrentMiiSavPath = miiSavPath;
                RefreshMiiList();
                MessageBox.Show("Save file found!", "TomoAIO");
            }
            else
            {
                MessageBox.Show(
                    "Could not find Mii.sav in that folder!\n\nPlease make sure you selected the correct save folder.",
                    "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion
    }
}