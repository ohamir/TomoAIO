using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;
using TomoAIO.Infrastructure;
using TomoAIO.Models;
using TomoAIO.Services;

namespace TomoAIO
{
    public partial class MainForm : Form
    {
        private readonly AppState _state = new();
        private readonly UpdateService _updateService = new();

        // Store original form size and control bounds for scaling
        private SizeF _originalFormSize;
        private readonly System.Collections.Generic.Dictionary<Control, RectangleF> _originalBounds = new();
        private readonly System.Collections.Generic.Dictionary<Control, float> _originalFonts = new();

        // Child form instances (modeless)
        private MiiImportForm? _miiForm;
        private IslandManagerForm? _islandForm;

        public MainForm()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
            CheckForUpdates();

            // Capture original sizes after layout is done
            this.Load += (s, e) =>
            {
                InitializeSavePath();
                CaptureOriginalBounds();
            };
            this.Resize += MainForm_Resize;
        }

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
                _originalFonts[c] = c.Font.Size;
                if (c.Controls.Count > 0)
                    CaptureControlBounds(c.Controls);
            }
        }

        private void MainForm_Resize(object? sender, EventArgs e)
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

                // Scale font from original size (not current) to prevent drift
                if (_originalFonts.TryGetValue(c, out float origFontSize))
                {
                    float newFontSize = Math.Max(6f, origFontSize * Math.Min(scaleX, scaleY));
                    c.Font = new Font(c.Font.FontFamily, newFontSize, c.Font.Style);
                }

                if (c.Controls.Count > 0)
                    ScaleControls(c.Controls, scaleX, scaleY);
            }
        }

        private async void CheckForUpdates()
        {
            try
            {
                string currentVersion = "1.2";
                string repoOwner = "ohamir";
                string repoName = "TomoAIO";
                string? latestVersion = await _updateService.GetLatestVersionAsync(repoOwner, repoName);
                if (!string.IsNullOrWhiteSpace(latestVersion) && latestVersion != currentVersion)
                {
                    DialogResult dialog = MessageBox.Show(
                        $"A new version of TomoAIO is available! (v{latestVersion})\n\nWould you like to open the download page?",
                        "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (dialog == DialogResult.Yes)
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = $"https://github.com/{repoOwner}/{repoName}/releases/latest",
                            UseShellExecute = true
                        });
                }
            }
            catch { }
        }

        #region Save Path

        private void InitializeSavePath()
        {
            string? saved = SaveConfig.LoadSavePath();

            if (!string.IsNullOrWhiteSpace(saved) && Directory.Exists(saved))
            {
                _state.SaveFolderPath = saved;
                // Derive Player.sav path automatically
                _state.CurrentPlayerSavPath = Path.Combine(saved, "Player.sav");
                UpdateSavePathLabel();
                return;
            }

            PromptForSaveFolder();
        }

        private void PromptForSaveFolder()
        {
            using var fbd = new FolderBrowserDialog
            {
                Description = "Select your game save folder",
                UseDescriptionForTitle = true
            };

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                _state.SaveFolderPath = fbd.SelectedPath;
                _state.CurrentPlayerSavPath = Path.Combine(fbd.SelectedPath, "Player.sav");
                SaveConfig.StoreSavePath(fbd.SelectedPath);
                UpdateSavePathLabel();
            }
            else
            {
                MessageBox.Show("No save folder selected. Some features may not work.", "TomoAIO",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateSavePathLabel()
        {
            if (lblSavePath != null)
                lblSavePath.Text = $"{_state.SaveFolderPath}";
        }

        private void ChangeSaveFolderBtn_Click(object sender, EventArgs e)
        {
            PromptForSaveFolder();
        }

        #endregion


        #region Mii Import Button
        private void ImportMiiBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_state.SaveFolderPath))
            {
                MessageBox.Show("Please set your save folder first.", "TomoAIO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_miiForm == null || _miiForm.IsDisposed)
            {
                _miiForm = new MiiImportForm(_state);
                _miiForm.Show();
            }
            else
            {
                _miiForm.BringToFront();
            }
        }

        #endregion


        #region UGC Editor Button
        private void UgcEditorBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_state.SaveFolderPath))
            {
                MessageBox.Show("Please set your save folder first.", "TomoAIO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //var form = new UgcEditorForm(_state);
            //form.Show();
        }

        #endregion


        #region Island Manager Button

        private void IslandManagerBtn_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_state.SaveFolderPath))
            {
                MessageBox.Show("Please set your save folder first.", "TomoAIO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(_state.CurrentPlayerSavPath))
            {
                MessageBox.Show($"Player.sav not found at:\n{_state.CurrentPlayerSavPath}\n\nPlease check your save folder.",
                    "TomoAIO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_islandForm == null || _islandForm.IsDisposed)
            {
                _islandForm = new IslandManagerForm(_state);
                _islandForm.Show();
            }
            else
            {
                _islandForm.BringToFront();
            }
        }
        #endregion


        #region Discord Button 
        private void DiscordJoinBtn_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://discord.gg/JmEttdwE5J") { UseShellExecute = true });
        }

        #endregion

        private void lblSavePath_Click(object sender, EventArgs e)
        {
           PromptForSaveFolder();
        }
    }
}