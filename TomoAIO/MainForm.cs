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

        public MainForm()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
            CheckForUpdates();
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