using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TomoAIO.Models;

namespace TomoAIO
{
    public partial class IslandManagerForm : Form
    {
        private readonly AppState _state;
        private readonly Dictionary<Control, (Size size, int radius)> _roundedCache = new();

        // ─── Scaling ──────────────────────────────────────────────────────────
        private SizeF _originalFormSize;
        private readonly Dictionary<Control, RectangleF> _originalBounds = new();
        private readonly Dictionary<Control, Font> _originalFonts = new();

        private static readonly Color ButtonPrimaryColor = Color.FromArgb(47, 61, 82);
        private static readonly Color ButtonPrimaryHoverColor = Color.FromArgb(61, 79, 106);
        private static readonly Color ButtonPrimaryPressedColor = Color.FromArgb(35, 46, 62);
        private static readonly Color ButtonSecondaryColor = Color.FromArgb(84, 96, 110);
        private static readonly Color ButtonSecondaryHoverColor = Color.FromArgb(102, 116, 132);
        private static readonly Color ButtonSecondaryPressedColor = Color.FromArgb(66, 77, 89);

        public IslandManagerForm(AppState state)
        {
            _state = state;
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            ConfigureButtons();
            RefreshIslandManagementUI();

            this.Load += (s, e) => CaptureOriginalBounds();
            this.Resize += IslandManagerForm_Resize;
        }

        // ─── Setup ────────────────────────────────────────────────────────────

        private void ConfigureButtons()
        {
            //btnMenuBack.Text = "Back to Menu";

            StyleActionButton(btnSaveMoney, ButtonPrimaryColor, ButtonPrimaryHoverColor, ButtonPrimaryPressedColor);
            StyleActionButton(btnUnlockInteriors, ButtonPrimaryColor, ButtonPrimaryHoverColor, ButtonPrimaryPressedColor);
            StyleActionButton(btnUnlockQBuilds, ButtonPrimaryColor, ButtonPrimaryHoverColor, ButtonPrimaryPressedColor);
            StyleActionButton(btnUnlockClothes, ButtonPrimaryColor, ButtonPrimaryHoverColor, ButtonPrimaryPressedColor);
            StyleActionButton(btnUnlockFood, ButtonPrimaryColor, ButtonPrimaryHoverColor, ButtonPrimaryPressedColor);
            //StyleActionButton(btnMenuBack, ButtonSecondaryColor, ButtonSecondaryHoverColor, ButtonSecondaryPressedColor);
        }

        private void StyleActionButton(Button button, Color baseColor, Color hoverColor, Color pressedColor)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Color.Black;
            button.FlatAppearance.MouseOverBackColor = hoverColor;
            button.FlatAppearance.MouseDownBackColor = pressedColor;
            button.UseVisualStyleBackColor = false;
            button.BackColor = baseColor;
            button.ForeColor = Color.White;
            button.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            button.Cursor = Cursors.Hand;
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.Padding = new Padding(8, 3, 8, 3);
            ApplyRoundedCorners(button, 8);
            button.Resize += (_, _) => ApplyRoundedCorners(button, 8);
        }

        private void ApplyRoundedCorners(Control control, int radius)
        {
            if (control.Width <= 0 || control.Height <= 0) return;
            if (_roundedCache.TryGetValue(control, out var cached) &&
                cached.size == control.Size && cached.radius == radius) return;

            int maxRadius = Math.Max(2, Math.Min(control.Width, control.Height) / 2);
            int effectiveRadius = Math.Min(radius, maxRadius);
            int diameter = effectiveRadius * 2;

            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, diameter, diameter, 180, 90);
            path.AddArc(control.Width - diameter, 0, diameter, diameter, 270, 90);
            path.AddArc(control.Width - diameter, control.Height - diameter, diameter, diameter, 0, 90);
            path.AddArc(0, control.Height - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            control.Region?.Dispose();
            control.Region = new Region(path);
            path.Dispose();
            _roundedCache[control] = (control.Size, radius);
        }

        // ─── Scaling Methods ──────────────────────────────────────────────────

        private void CaptureOriginalBounds()
        {
            _originalFormSize = new SizeF(this.ClientSize.Width, this.ClientSize.Height);
            CaptureControlBounds(this.Controls);
        }

        private void CaptureControlBounds(Control.ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                _originalBounds[c] = new RectangleF(c.Left, c.Top, c.Width, c.Height);
                _originalFonts[c] = c.Font;
                if (c.Controls.Count > 0)
                    CaptureControlBounds(c.Controls);
            }
        }

        private void IslandManagerForm_Resize(object? sender, EventArgs e)
        {
            if (_originalFormSize.IsEmpty) return;

            float scaleX = this.ClientSize.Width / _originalFormSize.Width;
            float scaleY = this.ClientSize.Height / _originalFormSize.Height;

            ScaleControls(this.Controls, scaleX, scaleY);
        }

        private void ScaleControls(Control.ControlCollection controls, float scaleX, float scaleY)
        {
            foreach (Control c in controls)
            {
                if (!_originalBounds.TryGetValue(c, out RectangleF orig)) continue;

                c.Left = (int)(orig.X * scaleX);
                c.Top = (int)(orig.Y * scaleY);
                c.Width = (int)(orig.Width * scaleX);
                c.Height = (int)(orig.Height * scaleY);

                if (_originalFonts.TryGetValue(c, out Font origFont))
                {
                    float newSize = Math.Max(6f, origFont.Size * Math.Min(scaleX, scaleY));
                    c.Font = new Font(origFont.FontFamily, newSize, origFont.Style);
                }

                if (c.Controls.Count > 0)
                    ScaleControls(c.Controls, scaleX, scaleY);
            }
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

        private void RefreshIslandManagementUI()
        {
            if (string.IsNullOrEmpty(_state.CurrentPlayerSavPath) ||
                !File.Exists(_state.CurrentPlayerSavPath)) return;

            try
            {
                byte[] pBytes = File.ReadAllBytes(_state.CurrentPlayerSavPath);

                // Money
                int totalCents = GetActualOffset(pBytes, "365FAB1F");
                if (totalCents >= 0)
                {
                    decimal dollars = (decimal)totalCents / 100;
                    txtCurrentMoney.Text = dollars.ToString("F2");
                    txtCurrentMoney.Value = Math.Min(txtCurrentMoney.Maximum, dollars);
                }

                // Island name → form title
                int nameOffset = GetActualOffset(pBytes, "D46DF986");
                if (nameOffset > 0)
                {
                    byte[] nameBytes = pBytes.Skip(nameOffset).Take(64).ToArray();
                    string islandName = System.Text.Encoding.Unicode.GetString(nameBytes);
                    int nullIndex = islandName.IndexOf('\0');
                    if (nullIndex != -1) islandName = islandName.Substring(0, nullIndex);
                    if (lblIslandTitle != null)
                        lblIslandTitle.Text = $"{islandName} Management";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading island data: " + ex.Message, "TomoAIO");
            }
        }

        private void UnlockSpecificCategory(string[]? arrayHashes, bool unlockRooms, string categoryName)
        {
            if (string.IsNullOrEmpty(_state.CurrentPlayerSavPath)) return;

            string? saveDir = Path.GetDirectoryName(_state.CurrentPlayerSavPath);
            if (string.IsNullOrEmpty(saveDir)) return;

            string playerPath = Path.Combine(saveDir, "Player.sav");
            if (!File.Exists(playerPath))
            {
                MessageBox.Show("Could not find Player.sav to unlock items!", "Error");
                return;
            }

            try
            {
                byte[] pBytes = File.ReadAllBytes(playerPath);
                byte[] obtainedState = { 0x7C, 0xEF, 0x5F, 0xBC };
                bool fileWasModified = false;

                // Unlock array-based items (clothes, food, quik builds, etc.)
                if (arrayHashes != null)
                {
                    foreach (string hash in arrayHashes)
                    {
                        int arrayOffset = GetActualOffset(pBytes, hash);
                        if (arrayOffset > 0 && arrayOffset < pBytes.Length - 4)
                        {
                            int itemCount = BitConverter.ToInt32(pBytes, arrayOffset);
                            int dataStart = arrayOffset + 4;

                            if (itemCount > 0 && dataStart + (itemCount * 4) <= pBytes.Length)
                            {
                                for (int j = 0; j < itemCount; j++)
                                    Array.Copy(obtainedState, 0, pBytes, dataStart + (j * 4), 4);
                                fileWasModified = true;
                            }
                        }
                    }
                }

                // Unlock rooms via CSV hash list
                if (unlockRooms)
                {
                    string hashFilePath = Path.Combine(Application.StartupPath, "services", "room_hashes.csv");
                    if (File.Exists(hashFilePath))
                    {
                        string[] roomHashes = File.ReadAllLines(hashFilePath);
                        foreach (string line in roomHashes)
                        {
                            string hashHex = line.Trim();
                            if (hashHex.Equals("Hash", StringComparison.OrdinalIgnoreCase) ||
                                hashHex.Length != 8) continue;

                            byte[] hashBytes = Enumerable.Range(0, 4)
                                .Select(x => Convert.ToByte(hashHex.Substring(x * 2, 2), 16))
                                .Reverse().ToArray();

                            for (int i = 0; i <= pBytes.Length - 8; i++)
                            {
                                if (pBytes.Skip(i).Take(4).SequenceEqual(hashBytes))
                                {
                                    Array.Copy(obtainedState, 0, pBytes, i + 4, 4);
                                    fileWasModified = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Warning: 'services\\room_hashes.csv' was not found!", "Missing File");
                    }
                }

                if (fileWasModified)
                {
                    File.WriteAllBytes(playerPath, pBytes);
                    MessageBox.Show($"{categoryName} successfully unlocked!", "TomoAIO");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unlock Error: " + ex.Message, "TomoAIO");
            }
        }

        // ─── Event Handlers ───────────────────────────────────────────────────

        private void IslandManagerForm_Load(object sender, EventArgs e)
        {
            RefreshIslandManagementUI();
        }

        private void btnSaveMoney_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_state.CurrentPlayerSavPath) ||
                !File.Exists(_state.CurrentPlayerSavPath)) return;

            try
            {
                byte[] pBytes = File.ReadAllBytes(_state.CurrentPlayerSavPath);

                byte[] hash = Enumerable.Range(0, 4)
                    .Select(x => Convert.ToByte("365FAB1F".Substring(x * 2, 2), 16))
                    .Reverse().ToArray();

                int valueIndex = -1;
                for (int i = 0; i <= pBytes.Length - 8; i++)
                {
                    if (pBytes.Skip(i).Take(4).SequenceEqual(hash))
                    {
                        valueIndex = i + 4;
                        break;
                    }
                }

                if (valueIndex != -1)
                {
                    int totalCents = (int)(txtCurrentMoney.Value * 100);
                    Array.Copy(BitConverter.GetBytes(totalCents), 0, pBytes, valueIndex, 4);
                    File.WriteAllBytes(_state.CurrentPlayerSavPath, pBytes);
                    MessageBox.Show($"Success! Your island funds have been set to ${txtCurrentMoney.Value:N2}", "Bank Updated");
                    RefreshIslandManagementUI();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update money: " + ex.Message, "TomoAIO");
            }
        }


        private void btnUnlockInteriors_Click(object sender, EventArgs e) =>
            UnlockSpecificCategory(null, true, "Interiors");

        private void btnUnlockQBuilds_Click(object sender, EventArgs e) =>
            UnlockSpecificCategory(new[] { "3AF3C005" }, false, "Quik Builds");

        private void btnUnlockClothes_Click(object sender, EventArgs e) =>
            UnlockSpecificCategory(new[] { "D273AD77", "7708017B" }, false, "Clothes");

        private void btnUnlockFood_Click(object sender, EventArgs e) =>
            UnlockSpecificCategory(new[] { "933DA780" }, false, "Foods");

        private void numMoney_ValueChanged(object sender, EventArgs e) { }


    }
}