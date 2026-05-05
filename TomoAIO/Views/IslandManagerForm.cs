using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TomoAIO.Infrastructure;
using TomoAIO.Models;
using TomoAIO.Services;

namespace TomoAIO
{
    public partial class IslandManagerForm : Form
    {
        // ─── Dependencies ─────────────────────────────────────────────────────
        private readonly AppState _state;
        private readonly IslandService _islandService;

        // ─── Rounded corners cache ────────────────────────────────────────────
        private readonly Dictionary<Control, (Size size, int radius)> _roundedCache = new();

        // ─── Scaling ──────────────────────────────────────────────────────────
        private SizeF _originalFormSize;
        private readonly Dictionary<Control, RectangleF> _originalBounds = new();
        private readonly Dictionary<Control, Font> _originalFonts = new();

        // ─── Colors ───────────────────────────────────────────────────────────
        private static readonly Color ButtonPrimaryColor = Color.FromArgb(47, 61, 82);
        private static readonly Color ButtonPrimaryHoverColor = Color.FromArgb(61, 79, 106);
        private static readonly Color ButtonPrimaryPressedColor = Color.FromArgb(35, 46, 62);

        // ─── Constructor ──────────────────────────────────────────────────────
        public IslandManagerForm(AppState state)
        {
            _state = state;
            _islandService = new IslandService(new SaveFileRepository());
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

            ConfigureButtons();

            this.Load += IslandManagerForm_Load;
            this.Resize += IslandManagerForm_Resize;
        }

        // ─── Setup ────────────────────────────────────────────────────────────

        private void ConfigureButtons()
        {
            StyleActionButton(btnSaveMoney, ButtonPrimaryColor, ButtonPrimaryHoverColor, ButtonPrimaryPressedColor);
            StyleActionButton(btnUnlockInteriors, ButtonPrimaryColor, ButtonPrimaryHoverColor, ButtonPrimaryPressedColor);
            StyleActionButton(btnUnlockQBuilds, ButtonPrimaryColor, ButtonPrimaryHoverColor, ButtonPrimaryPressedColor);
            StyleActionButton(btnUnlockClothes, ButtonPrimaryColor, ButtonPrimaryHoverColor, ButtonPrimaryPressedColor);
            StyleActionButton(btnUnlockFood, ButtonPrimaryColor, ButtonPrimaryHoverColor, ButtonPrimaryPressedColor);
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

        // ─── Scaling ──────────────────────────────────────────────────────────

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

        // ─── UI Refresh ───────────────────────────────────────────────────────

        private void RefreshUI()
        {
            if (string.IsNullOrEmpty(_state.CurrentPlayerSavPath)) return;

            try
            {
                IslandData? data = _islandService.LoadIslandData(_state.CurrentPlayerSavPath);
                if (data == null) return;

                txtCurrentMoney.Text = data.BalanceDollars.ToString("F2");
                txtCurrentMoney.Value = Math.Min(txtCurrentMoney.Maximum, data.BalanceDollars);

                if (!string.IsNullOrEmpty(data.IslandName))
                    lblIslandTitle.Text = $"Managing - {data.IslandName}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading island data: " + ex.Message, "TomoAIO");
            }
        }

        // ─── Event Handlers ───────────────────────────────────────────────────

        private void IslandManagerForm_Load(object sender, EventArgs e)
        {
            CaptureOriginalBounds();
            RefreshUI();
        }

        private void btnSaveMoney_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_state.CurrentPlayerSavPath)) return;

            try
            {
                _islandService.SaveMoney(_state.CurrentPlayerSavPath, txtCurrentMoney.Value);
                MessageBox.Show($"Funds set to ${txtCurrentMoney.Value:N2}", "Bank Updated");
                RefreshUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update money: " + ex.Message, "TomoAIO");
            }
        }

        private void btnUnlockInteriors_Click(object sender, EventArgs e) =>
            RunUnlock(null, true, "Interiors");

        private void btnUnlockQBuilds_Click(object sender, EventArgs e) =>
            RunUnlock(new[] { "3AF3C005" }, false, "Quik Builds");

        private void btnUnlockClothes_Click(object sender, EventArgs e) =>
            RunUnlock(new[] { "D273AD77", "7708017B" }, false, "Clothes");

        private void btnUnlockFood_Click(object sender, EventArgs e) =>
            RunUnlock(new[] { "933DA780" }, false, "Foods");

        private void RunUnlock(string[]? hashes, bool rooms, string categoryName)
        {
            if (string.IsNullOrEmpty(_state.CurrentPlayerSavPath)) return;

            try
            {
                string playerPath = Path.Combine(
                    Path.GetDirectoryName(_state.CurrentPlayerSavPath)!, "Player.sav");

                bool ok = _islandService.UnlockCategory(playerPath, hashes, rooms);
                MessageBox.Show(
                    ok ? $"{categoryName} successfully unlocked!" : "Nothing was changed.",
                    "TomoAIO");
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Missing File");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unlock Error: " + ex.Message, "TomoAIO");
            }
        }
    }
}