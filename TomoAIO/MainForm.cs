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

        public MainForm()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
            CheckForUpdates();

            // Capture original sizes after layout is done
            this.Load += (s, e) => CaptureOriginalBounds();
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

                // Scale font too
                float newFontSize = Math.Max(6f, c.Font.Size * Math.Min(scaleX, scaleY));
                c.Font = new Font(c.Font.FontFamily, newFontSize, c.Font.Style);

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

        #region Mii Import Button
        private void ImportMiiBtn_Click(object sender, EventArgs e)
        {
            var form = new MiiImportForm(_state);
            form.ShowDialog(this);
        }

        #endregion


        #region UGC Editor Button
        private void UgcEditorBtn_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog { Description = "Select your Living the Dream 'Ugc' folder" };
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                //var form = new UgcEditorForm(_state, fbd.SelectedPath);
                //form.ShowDialog(this);
            }
        }

        #endregion


        #region Island Manager Button

        private void IslandManagerBtn_Click_1(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Select your Player.sav file",
                Filter = "Tomodachi Player Save (Player.sav)|Player.sav"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _state.CurrentPlayerSavPath = ofd.FileName;
                var form = new IslandManagerForm(_state);
                form.ShowDialog(this);
            }
        }
        #endregion


        #region Discord Button 
        private void DiscordJoinBtn_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://discord.gg/JmEttdwE5J") { UseShellExecute = true });
        }

        #endregion
    }
}