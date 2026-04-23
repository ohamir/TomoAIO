using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ZstdSharp;
using System.Net.Http;
using System.Diagnostics;

namespace TomoAIO
{
    public partial class Form1 : Form
    {
        private List<string> allUgcFiles = new List<string>();
        private readonly Dictionary<string, string> ugcDisplayToFile = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        string currentMiiSavPath = "";
        string currentUgcPath = "";
        private const int LogoMargin = 12;
        private const int DiscordMargin = 12;
        private readonly Size _baseLogoSize = new Size(175, 160);
        private readonly Size _baseClientSize = new Size(1343, 745);
        private readonly Size _baseButtonSize = new Size(366, 285);
        private const int BaseButtonGap = 45;
        private const int MiiToolbarWidth = 760;
        private const int UgcSidebarWidth = 250;
        private static readonly Color ButtonPrimaryColor = Color.FromArgb(47, 61, 82);
        private static readonly Color ButtonPrimaryHoverColor = Color.FromArgb(61, 79, 106);
        private static readonly Color ButtonPrimaryPressedColor = Color.FromArgb(35, 46, 62);
        private static readonly Color ButtonSecondaryColor = Color.FromArgb(84, 96, 110);
        private static readonly Color ButtonSecondaryHoverColor = Color.FromArgb(102, 116, 132);
        private static readonly Color ButtonSecondaryPressedColor = Color.FromArgb(66, 77, 89);
        private static readonly Color InputBorderColor = Color.FromArgb(61, 79, 106);
        private static readonly Color InputSurfaceColor = Color.White;
        private Panel? _comboHost;
        private Panel? _pathHost;
        private Label? _pathDisplay;
        private Label? _actionDropdownText;
        private Button? _actionDropdownArrow;
        private ListBox? _actionDropdownList;
        private bool _actionDropdownOpen;
        private string? _selectedMiiAction;
        private readonly string[] _miiActions = { "Import Mii (.ltd)", "Export Mii (.ltd)" };
        private readonly Dictionary<Control, (Size size, int radius)> _roundedCache = new Dictionary<Control, (Size size, int radius)>();
        private string _selectedMiiPath = "Choose a Mii file here...";

        string[] persHashes = {
            "43CD364F", "CD8DBAF8", "25B48224", "607BA160", "68E1134E", // Personality P1-P5
            "4913AE1A", "141EE086", "07B9D175", "81CF470A", "4D78E262", "FBC3FFB0", // Voice V1-V6
            "236E2D73", "F3C3DE59", "660C5247", // Gender (S1), Pronoun (S2), Style (S3)
            "5D7D3F45", "AB8AE08B", "2545E583", "6CF484F4" // Birthday B1-B4
        };

        public Form1()
        {
            InitializeComponent();
            EnableSmoothRendering();
            ConfigureMenuButtons();
            ConfigureActionButtons();
            ConfigureInputStyles();
            EnsureDiscordButtonVisible();
        }

        private async void CheckForUpdates()
        {
            try
            {
                string currentVersion = "1.2"; 
                string repoOwner = "ohamir"; 
                string repoName = "TomoAIO";   

                string apiUrl = $"https://api.github.com/repos/{repoOwner}/{repoName}/releases/latest";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "TomoAIO-Updater");
                    string response = await client.GetStringAsync(apiUrl);
                    int tagIndex = response.IndexOf("\"tag_name\"");
                    if (tagIndex != -1)
                    {
                        int startQuote = response.IndexOf('"', tagIndex + 11) + 1;
                        int endQuote = response.IndexOf('"', startQuote);
                        string latestVersion = response.Substring(startQuote, endQuote - startQuote);
                        latestVersion = latestVersion.Replace("v", "").Replace("V", "");

                        if (latestVersion != currentVersion)
                        {
                            DialogResult dialog = MessageBox.Show(
                                $"A new version of TomoAIO is available! (v{latestVersion})\n\nWould you like to open the download page?",
                                "Update Available",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information);

                            if (dialog == DialogResult.Yes)
                            {
                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = $"https://github.com/{repoOwner}/{repoName}/releases/latest",
                                    UseShellExecute = true
                                });
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void EnableSmoothRendering()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            EnableDoubleBuffer(panel1);
            EnableDoubleBuffer(panelUGC);
            EnableDoubleBuffer(panelSidebar);
            EnableDoubleBuffer(listBox1);
            EnableDoubleBuffer(lstUGC);
            EnableDoubleBuffer(picPreview);
        }

        private static void EnableDoubleBuffer(Control control)
        {
            typeof(Control)
                .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)?
                .SetValue(control, true, null);
        }

        private void ConfigureMenuButtons()
        {
            button1.Parent = pictureBox1;
            button2.Parent = pictureBox1;
            ApplyLogoStyle(logo, pictureBox1);
            ApplyLogoStyle(logopanel1, pictureBox2);
            ApplyLogoStyle(pictureBox3, panelUGC);
            pictureBox1.SendToBack();
            logo.BringToFront();
            pictureBox2.SendToBack();
            logopanel1.BringToFront();

            ConfigureTransparentButton(button1);
            ConfigureTransparentButton(button2);
            MakePictureBackgroundTransparent(logo, Color.FromArgb(255, 190, 0), 38);
            MakePictureBackgroundTransparent(pictureBox3, Color.FromArgb(255, 190, 0), 38);
            MakePictureBackgroundTransparent(logopanel1, Color.FromArgb(255, 190, 0), 38);
            MakePictureBackgroundTransparent(logo, Color.White, 65);
            MakePictureBackgroundTransparent(pictureBox3, Color.White, 65);
            MakePictureBackgroundTransparent(logopanel1, Color.White, 65);
            logo.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox3.BackgroundImageLayout = ImageLayout.Zoom;
            logopanel1.BackgroundImageLayout = ImageLayout.Zoom;
            PinLogoTopRight();
            LayoutMainMenuButtons();
        }

        private void ApplyLogoStyle(PictureBox logoControl, Control parent)
        {
            logoControl.Parent = parent;
            logoControl.BackColor = Color.Transparent;
            logoControl.BackgroundImageLayout = ImageLayout.Zoom;
        }

        private void EnsureMenuImages()
        {
            if (pictureBox1.Image == null)
            {
                pictureBox1.Image = Properties.Resources._2026_04_20_17_04_43;
            }

            if (button1.BackgroundImage == null)
            {
                button1.BackgroundImage = Properties.Resources.mii_import1;
            }

            if (button2.BackgroundImage == null)
            {
                button2.BackgroundImage = Properties.Resources.UGC_CREATOR;
            }

            if (logo.BackgroundImage == null)
            {
                logo.BackgroundImage = Properties.Resources.tomoaio_logo;
            }

            if (logopanel1.BackgroundImage == null)
            {
                logopanel1.BackgroundImage = Properties.Resources.tomoaio_logo;
            }

            if (pictureBox3.BackgroundImage == null)
            {
                pictureBox3.BackgroundImage = Properties.Resources.tomoaio_logo;
            }

            ApplyLogoStyle(logo, pictureBox1);
            ApplyLogoStyle(logopanel1, pictureBox2);
            ApplyLogoStyle(pictureBox3, panelUGC);
            PinLogoTopRight();
        }

        private void ShowMainMenu()
        {
            panel1.Visible = false;
            panelUGC.Visible = false;
            CloseActionDropdown();

            EnsureMenuImages();
            pictureBox1.Visible = true;
            pictureBox1.BringToFront();
            logo.BringToFront();
            button1.BringToFront();
            button2.BringToFront();

            PinLogoTopRight();
            LayoutMainMenuButtons();
            EnsureDiscordButtonVisible();
            pictureBox1.Refresh();
        }

        private void EnsureDiscordButtonVisible()
        {
            if (btnDiscord.BackgroundImage == null)
            {
                string logoPath = Path.Combine(Application.StartupPath, "Resources", "discord-logo.png");
                if (File.Exists(logoPath))
                {
                    using (var stream = new MemoryStream(File.ReadAllBytes(logoPath)))
                    using (var image = Image.FromStream(stream))
                    {
                        btnDiscord.BackgroundImage = new Bitmap(image);
                    }
                }
                else
                {
                    btnDiscord.BackgroundImage = Properties.Resources.discord;
                }
            }

            btnDiscord.Text = string.Empty;
            btnDiscord.BackgroundImageLayout = ImageLayout.Zoom;
            btnDiscord.FlatStyle = FlatStyle.Flat;
            btnDiscord.FlatAppearance.BorderSize = 0;
            btnDiscord.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnDiscord.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnDiscord.Anchor = AnchorStyles.None;
            btnDiscord.Visible = true;
            btnDiscord.Enabled = true;
            PinDiscordBottomRight();
            btnDiscord.BringToFront();
        }

        private void PinDiscordBottomRight()
        {
            if (btnDiscord.Parent == null)
            {
                return;
            }

            btnDiscord.Location = new Point(
                Math.Max(0, btnDiscord.Parent.ClientSize.Width - btnDiscord.Width - DiscordMargin),
                Math.Max(0, btnDiscord.Parent.ClientSize.Height - btnDiscord.Height - DiscordMargin));
        }

        private void PinLogoTopRight()
        {
            float scaleX = ClientSize.Width / (float)_baseClientSize.Width;
            float scaleY = ClientSize.Height / (float)_baseClientSize.Height;
            float logoScale = Math.Max(0.6f, Math.Min(1.35f, Math.Min(scaleX, scaleY)));
            int logoWidth = Math.Max(110, (int)Math.Round(_baseLogoSize.Width * logoScale));
            int logoHeight = Math.Max(100, (int)Math.Round(_baseLogoSize.Height * logoScale));

            if (logo.Parent != null)
            {
                logo.Size = new Size(logoWidth, logoHeight);
                logo.Location = new Point(
                    Math.Max(0, logo.Parent.ClientSize.Width - logo.Width - LogoMargin),
                    LogoMargin);
            }

            if (logopanel1.Parent != null)
            {
                logopanel1.Size = new Size(logoWidth, logoHeight);
                logopanel1.Location = new Point(
                    Math.Max(0, logopanel1.Parent.ClientSize.Width - logopanel1.Width - LogoMargin),
                    LogoMargin);
            }

            if (pictureBox3.Parent != null)
            {
                pictureBox3.Size = new Size(logoWidth, logoHeight);
                pictureBox3.Location = new Point(
                    Math.Max(0, pictureBox3.Parent.ClientSize.Width - pictureBox3.Width - LogoMargin),
                    LogoMargin);
            }
        }

        private void LayoutMainMenuButtons()
        {
            int clientWidth = ClientSize.Width;
            int clientHeight = ClientSize.Height;
            if (clientWidth <= 0 || clientHeight <= 0)
            {
                return;
            }

            float scaleX = clientWidth / (float)_baseClientSize.Width;
            float scaleY = clientHeight / (float)_baseClientSize.Height;
            float scale = Math.Max(0.6f, Math.Min(scaleX, scaleY));

            int buttonWidth = (int)Math.Round(_baseButtonSize.Width * scale);
            int buttonHeight = (int)Math.Round(_baseButtonSize.Height * scale);
            int gap = (int)Math.Round(BaseButtonGap * scale);

            int totalWidth = (buttonWidth * 2) + gap;
            int reservedRight = GetRightTopReservedWidth();
            int usableWidth = Math.Max(260, clientWidth - reservedRight);
            int startX = Math.Max(20, (usableWidth - totalWidth) / 2);
            int y = Math.Max(110, (clientHeight - buttonHeight) / 2);

            button1.Size = new Size(buttonWidth, buttonHeight);
            button2.Size = new Size(buttonWidth, buttonHeight);
            button1.Location = new Point(startX, y);
            button2.Location = new Point(startX + buttonWidth + gap, y);
        }

        private void LayoutMiiEditorControls()
        {
            if (_comboHost == null || _pathHost == null || _pathDisplay == null || _actionDropdownText == null || _actionDropdownArrow == null || _actionDropdownList == null)
            {
                return;
            }

            int panelWidth = Math.Max(1, panel1.ClientSize.Width);
            int panelHeight = Math.Max(1, panel1.ClientSize.Height);

            float scaleX = panelWidth / 1343f;
            float scaleY = panelHeight / 745f;
            float uiScale = Math.Max(0.8f, Math.Min(1.35f, Math.Min(scaleX, scaleY)));

            int outerMargin = Math.Max(14, (int)Math.Round(24 * uiScale));
            int top = Math.Max(10, (int)Math.Round(12 * uiScale));
            int rowGap = Math.Max(8, (int)Math.Round(12 * uiScale));

            int reservedRight = panel1.Visible ? GetRightTopReservedWidth() : 0;
            int maxArea = Math.Max(220, panelWidth - (outerMargin * 2) - reservedRight);
            int areaWidth = Math.Min((int)Math.Round(panelWidth * 0.72f), maxArea);
            areaWidth = Math.Max(260, areaWidth);
            areaWidth = Math.Min(areaWidth, maxArea);
            int left = Math.Max(outerMargin, (panelWidth - areaWidth) / 2);

            int buttonHeight = Math.Max(34, (int)Math.Round(36 * uiScale));
            int comboHeight = buttonHeight;
            int labelHeight = Math.Max(18, (int)Math.Round(18 * uiScale));
            int pathHeight = buttonHeight;
            int browseWidth = Math.Max(92, (int)Math.Round(116 * uiScale));
            browseWidth = Math.Min(browseWidth, Math.Max(92, (int)Math.Round(areaWidth * 0.35f)));
            int goWidth = Math.Max(118, (int)Math.Round(130 * uiScale));
            int goHeight = Math.Max(40, (int)Math.Round(42 * uiScale));
            int topButtonWidth = Math.Max(162, (int)Math.Round(170 * uiScale));

            button3.Location = new Point(left, top);
            button3.Size = new Size(topButtonWidth, buttonHeight);

            button4.Location = new Point(left + areaWidth - topButtonWidth, top);
            button4.Size = new Size(topButtonWidth, buttonHeight);

            int comboWidth = Math.Max(140, Math.Min(340, (int)Math.Round(areaWidth * 0.38f)));
            _comboHost!.Location = new Point(left, button3.Bottom + rowGap);
            _comboHost.Size = new Size(comboWidth, comboHeight);
            int arrowWidth = Math.Max(28, (int)Math.Round(30 * uiScale));
            _actionDropdownArrow.Size = new Size(arrowWidth, Math.Max(20, _comboHost.Height - 2));
            _actionDropdownArrow.Location = new Point(_comboHost.Width - arrowWidth - 1, 1);
            _actionDropdownText.Location = new Point(8, 1);
            _actionDropdownText.Size = new Size(Math.Max(70, _comboHost.Width - arrowWidth - 10), Math.Max(20, _comboHost.Height - 2));
            _actionDropdownList.Location = new Point(_comboHost.Left, _comboHost.Bottom + 2);
            _actionDropdownList.Size = new Size(_comboHost.Width, Math.Max(60, _miiActions.Length * 28));
            _actionDropdownList.BringToFront();

            label1.Location = new Point(left + 2, _comboHost.Bottom + rowGap);
            label1.Height = labelHeight;

            _pathHost!.Location = new Point(left, label1.Bottom + 4);
            _pathHost.Size = new Size(Math.Max(140, areaWidth - browseWidth - 8), pathHeight);
            PositionPathDisplay();

            bool compactMode = areaWidth < 500;
            if (compactMode)
            {
                _pathHost.Size = new Size(areaWidth, pathHeight);
                PositionPathDisplay();
                btnBrowseMii.Location = new Point(left, _pathHost.Bottom + 6);
                btnBrowseMii.Size = new Size(Math.Max(110, Math.Min(areaWidth, comboWidth)), pathHeight);
            }
            else
            {
                btnBrowseMii.Location = new Point(_pathHost.Right + 8, _pathHost.Top);
                btnBrowseMii.Size = new Size(browseWidth, pathHeight);
            }

            btnGo.Size = new Size(goWidth, goHeight);
            btnGo.Location = new Point(left + ((areaWidth - goWidth) / 2), btnBrowseMii.Bottom + 14);

            listBox1.Location = new Point(left, btnGo.Bottom + 12);
            int listHeight = Math.Max(120, panel1.ClientSize.Height - listBox1.Top - 20);
            listBox1.Size = new Size(areaWidth, listHeight);
        }

        private void PositionPathDisplay()
        {
            if (_pathHost == null || _pathDisplay == null)
            {
                return;
            }

            _pathDisplay.Dock = DockStyle.Fill;
            _pathDisplay.Padding = new Padding(8, 0, 8, 0);
        }

        private void LayoutUgcEditorControls()
        {
            int panelWidth = panelUGC.ClientSize.Width;
            int panelHeight = panelUGC.ClientSize.Height;
            if (panelWidth <= 0 || panelHeight <= 0)
            {
                return;
            }

            int minSidebar = 170;
            int maxSidebar = UgcSidebarWidth;
            int responsiveSidebar = (int)Math.Round(panelWidth * 0.24f);
            panelSidebar.Width = Math.Max(minSidebar, Math.Min(maxSidebar, responsiveSidebar));

            int outerMargin = 12;
            int contentLeft = panelSidebar.Width + outerMargin;
            int reservedRight = panelUGC.Visible ? GetRightTopReservedWidth() : 0;
            int contentRight = panelWidth - outerMargin - reservedRight;
            int contentWidth = Math.Max(220, contentRight - contentLeft);
            int top = 14;
            int gap = 10;

            btnUgcBack.Size = new Size(Math.Max(130, Math.Min(180, contentWidth / 3)), 38);
            btnUgcBack.Location = new Point(contentLeft, top);
            btnUgcBack.BringToFront();

            int contentTop = btnUgcBack.Bottom + gap;
            int footerReserve = 58;
            int availableHeight = Math.Max(150, panelHeight - contentTop - footerReserve);

            bool stackButtons = contentWidth < 430 || panelHeight < 520;
            int buttonHeight = 42;
            int buttonAreaHeight = stackButtons ? (buttonHeight * 2) + gap : buttonHeight;
            int infoHeight = 20;

            int maxPreviewHeight = Math.Max(120, availableHeight - buttonAreaHeight - infoHeight - (gap * 2));
            int squareSize = Math.Max(120, Math.Min(contentWidth, maxPreviewHeight));
            int squareX = contentLeft + Math.Max(0, (contentWidth - squareSize) / 2);
            int squareY = contentTop;

            picPreview.Location = new Point(squareX, squareY);
            picPreview.Size = new Size(squareSize, squareSize);

            int buttonY = picPreview.Bottom + gap;
            if (stackButtons)
            {
                int fullButtonWidth = Math.Max(130, Math.Min(contentWidth, squareSize));
                int centeredX = contentLeft + Math.Max(0, (contentWidth - fullButtonWidth) / 2);

                btnUgcImport.Location = new Point(centeredX, buttonY);
                btnUgcImport.Size = new Size(fullButtonWidth, buttonHeight);

                btnUgcExport.Location = new Point(centeredX, btnUgcImport.Bottom + gap);
                btnUgcExport.Size = new Size(fullButtonWidth, buttonHeight);
            }
            else
            {
                int buttonWidth = Math.Max(120, Math.Min(280, (squareSize - gap) / 2));
                int totalButtonsWidth = (buttonWidth * 2) + gap;
                int buttonsLeft = squareX + ((squareSize - totalButtonsWidth) / 2);

                btnUgcImport.Location = new Point(buttonsLeft, buttonY);
                btnUgcImport.Size = new Size(buttonWidth, buttonHeight);

                btnUgcExport.Location = new Point(btnUgcImport.Right + gap, buttonY);
                btnUgcExport.Size = new Size(buttonWidth, buttonHeight);
            }

            lblImageInfo.AutoSize = false;
            lblImageInfo.TextAlign = ContentAlignment.MiddleCenter;
            int infoTop = Math.Min(panelHeight - infoHeight - 4, btnUgcExport.Bottom + 6);
            lblImageInfo.Location = new Point(contentLeft, infoTop);
            lblImageInfo.Size = new Size(contentWidth, infoHeight);
        }

        private int GetRightTopReservedWidth()
        {
            int reserved = 0;

            if (logo.Visible && logo.Parent == this)
            {
                reserved = Math.Max(reserved, logo.Width + (LogoMargin * 2));
            }

            if (logopanel1.Visible && logopanel1.Parent == panel1)
            {
                reserved = Math.Max(reserved, logopanel1.Width + (LogoMargin * 2));
            }

            if (pictureBox3.Visible && pictureBox3.Parent == panelUGC)
            {
                reserved = Math.Max(reserved, pictureBox3.Width + (LogoMargin * 2));
            }

            return reserved;
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

        private void ConfigureActionButtons()
        {
            button3.Text = "Back to Menu";
            btnUgcBack.Text = "Back to Menu";

            StyleActionButton(button4, ButtonPrimaryColor, ButtonPrimaryHoverColor, ButtonPrimaryPressedColor);
            StyleActionButton(btnBrowseMii, ButtonPrimaryColor, ButtonPrimaryHoverColor, ButtonPrimaryPressedColor);
            StyleActionButton(btnGo, ButtonPrimaryColor, ButtonPrimaryHoverColor, ButtonPrimaryPressedColor);
            StyleActionButton(btnUgcImport, ButtonPrimaryColor, ButtonPrimaryHoverColor, ButtonPrimaryPressedColor);
            StyleActionButton(btnUgcExport, ButtonPrimaryColor, ButtonPrimaryHoverColor, ButtonPrimaryPressedColor);
            StyleActionButton(button3, ButtonSecondaryColor, ButtonSecondaryHoverColor, ButtonSecondaryPressedColor);
            StyleActionButton(btnUgcBack, ButtonSecondaryColor, ButtonSecondaryHoverColor, ButtonSecondaryPressedColor);
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
            button.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button.Cursor = Cursors.Hand;
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.Padding = new Padding(8, 3, 8, 3);
            button.AutoEllipsis = false;
            ApplyRoundedCorners(button, 8);
            button.Resize += (_, _) => ApplyRoundedCorners(button, 8);
        }

        private void ConfigureInputStyles()
        {
            CreateInputHosts();

            cmbMiiAction.Visible = false;
            ConfigureCustomActionDropdown();

            txtMiiPath.BackColor = InputSurfaceColor;
            txtMiiPath.ForeColor = Color.FromArgb(28, 36, 52);
            txtMiiPath.BorderStyle = BorderStyle.None;
            txtMiiPath.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtMiiPath.Visible = false;

            label1.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.FromArgb(32, 39, 55);
            label1.BackColor = Color.Transparent;

            listBox1.BorderStyle = BorderStyle.FixedSingle;
            listBox1.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            listBox1.BackColor = Color.FromArgb(248, 250, 254);
            listBox1.ForeColor = Color.FromArgb(30, 38, 52);
        }

        private void CreateInputHosts()
        {
            if (_comboHost == null)
            {
                _comboHost = new Panel
                {
                    BackColor = InputBorderColor
                };
                panel1.Controls.Add(_comboHost);
                _comboHost.BringToFront();
            }

            if (_pathHost == null)
            {
                _pathHost = new Panel
                {
                    BackColor = InputBorderColor,
                    Padding = new Padding(1)
                };
                panel1.Controls.Add(_pathHost);
                _pathHost.BringToFront();
            }

            if (_pathDisplay == null)
            {
                _pathDisplay = new Label
                {
                    Text = _selectedMiiPath,
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleLeft,
                    BackColor = InputSurfaceColor,
                    ForeColor = Color.FromArgb(28, 36, 52),
                    Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    AutoEllipsis = true,
                    Dock = DockStyle.Fill,
                    Padding = new Padding(8, 0, 8, 0)
                };
                _pathHost.Controls.Add(_pathDisplay);
            }

            _comboHost.BringToFront();
            _pathHost.BringToFront();
            label1.BringToFront();
            btnBrowseMii.BringToFront();
        }

        private void SetSelectedMiiPath(string path)
        {
            _selectedMiiPath = path;
            txtMiiPath.Text = path;
            if (_pathDisplay != null)
            {
                _pathDisplay.Text = path;
            }
        }

        private void ConfigureCustomActionDropdown()
        {
            if (_comboHost == null)
            {
                return;
            }

            if (_actionDropdownText == null)
            {
                _actionDropdownText = new Label
                {
                    Text = "Select Action...",
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleLeft,
                    BackColor = InputSurfaceColor,
                    ForeColor = Color.FromArgb(28, 36, 52),
                    Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    Cursor = Cursors.Hand,
                    Padding = new Padding(8, 0, 8, 0)
                };
                _actionDropdownText.Click += (_, _) => ToggleActionDropdown();
                _comboHost.Controls.Add(_actionDropdownText);
            }

            if (_actionDropdownArrow == null)
            {
                _actionDropdownArrow = new Button
                {
                    Text = "▼",
                    FlatStyle = FlatStyle.Flat,
                    BackColor = InputSurfaceColor,
                    ForeColor = Color.FromArgb(40, 50, 68),
                    Cursor = Cursors.Hand,
                    TabStop = false
                };
                _actionDropdownArrow.FlatAppearance.BorderSize = 0;
                _actionDropdownArrow.Click += (_, _) => ToggleActionDropdown();
                _comboHost.Controls.Add(_actionDropdownArrow);
            }

            if (_actionDropdownList == null)
            {
                _actionDropdownList = new ListBox
                {
                    Visible = false,
                    BackColor = InputSurfaceColor,
                    ForeColor = Color.FromArgb(28, 36, 52),
                    BorderStyle = BorderStyle.FixedSingle,
                    Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    IntegralHeight = false
                };
                _actionDropdownList.Items.AddRange(_miiActions);
                _actionDropdownList.Click += (_, _) =>
                {
                    if (_actionDropdownList.SelectedItem is string action)
                    {
                        _selectedMiiAction = action;
                        _actionDropdownText!.Text = action;
                    }
                    CloseActionDropdown();
                };
                panel1.Controls.Add(_actionDropdownList);
            }

            panel1.MouseDown -= PanelMouseDownCloseDropdown;
            panel1.MouseDown += PanelMouseDownCloseDropdown;
        }

        private void PanelMouseDownCloseDropdown(object? sender, MouseEventArgs e)
        {
            if (_actionDropdownOpen)
            {
                CloseActionDropdown();
            }
        }

        private void ToggleActionDropdown()
        {
            if (_actionDropdownList == null)
            {
                return;
            }

            _actionDropdownOpen = !_actionDropdownOpen;
            _actionDropdownList.Visible = _actionDropdownOpen;
            _actionDropdownList.BringToFront();
            if (_actionDropdownArrow != null)
            {
                _actionDropdownArrow.Text = _actionDropdownOpen ? "▲" : "▼";
            }
        }

        private void CloseActionDropdown()
        {
            _actionDropdownOpen = false;
            if (_actionDropdownList != null)
            {
                _actionDropdownList.Visible = false;
            }
            if (_actionDropdownArrow != null)
            {
                _actionDropdownArrow.Text = "▼";
            }
        }

        private void cmbMiiAction_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }

            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color bgColor = isSelected ? Color.FromArgb(232, 238, 248) : InputSurfaceColor;
            using SolidBrush bgBrush = new SolidBrush(bgColor);
            using SolidBrush textBrush = new SolidBrush(Color.FromArgb(28, 36, 52));
            e.Graphics.FillRectangle(bgBrush, e.Bounds);
            string itemText = cmbMiiAction.Items[e.Index]?.ToString() ?? string.Empty;
            Rectangle textRect = new Rectangle(e.Bounds.X + 8, e.Bounds.Y + 1, e.Bounds.Width - 8, e.Bounds.Height - 2);
            e.Graphics.DrawString(itemText, cmbMiiAction.Font, textBrush, textRect);
            e.DrawFocusRectangle();
        }

        private void ApplyRoundedCorners(Control control, int radius)
        {
            if (control.Width <= 0 || control.Height <= 0)
            {
                return;
            }

            if (_roundedCache.TryGetValue(control, out var cached) &&
                cached.size == control.Size &&
                cached.radius == radius)
            {
                return;
            }

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
        private void LoadUgcList(string exactUgcPath)
        {
            currentUgcPath = exactUgcPath;
            lstUGC.Items.Clear();
            allUgcFiles.Clear();
            ugcDisplayToFile.Clear();
            if (Directory.Exists(exactUgcPath))
            {
                string[] files = Directory
                    .GetFiles(exactUgcPath, "*.zs")
                    .Where(file =>
                        file.EndsWith(".canvas.zs", StringComparison.OrdinalIgnoreCase) ||
                        (file.EndsWith(".ugctex.zs", StringComparison.OrdinalIgnoreCase) &&
                         Path.GetFileName(file).Contains("thumb", StringComparison.OrdinalIgnoreCase)))
                    .OrderBy(Path.GetFileName)
                    .ToArray();

                if (files.Length == 0)
                {
                    MessageBox.Show("Please make sure you selected the right folder", "Folder Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    allUgcFiles.Add(fileName);
                    AddUgcListEntry(fileName);
                }
            }
        }

        private void AddUgcListEntry(string fileName)
        {
            string displayName = GetUgcDisplayName(fileName);
            string uniqueDisplay = displayName;
            int suffix = 2;
            while (ugcDisplayToFile.ContainsKey(uniqueDisplay))
            {
                uniqueDisplay = $"{displayName} ({suffix})";
                suffix++;
            }

            ugcDisplayToFile[uniqueDisplay] = fileName;
            lstUGC.Items.Add(uniqueDisplay);
        }

        private static string GetUgcDisplayName(string fileName)
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

            // Friendly label in UI only; real file names stay unchanged internally.
            display = display.Replace("thumb", "thumbnail", StringComparison.OrdinalIgnoreCase);
            return display;
        }

        private string? GetSelectedUgcFileName()
        {
            if (lstUGC.SelectedItem == null)
            {
                return null;
            }

            string selectedDisplay = lstUGC.SelectedItem.ToString() ?? string.Empty;
            return ugcDisplayToFile.TryGetValue(selectedDisplay, out string? realFile)
                ? realFile
                : null;
        }
        private byte[] EncodeRawTexture(Bitmap bmp, bool convertSrgbToLinear = false)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int bpp = 4;
            byte[] swizzledData = new byte[width * height * bpp];
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
            }

            clone.Dispose();
            return swizzledData;
        }
        private byte SrgbToLinearByte(byte srgb)
        {
            float s = srgb / 255f;
            float linear = (s <= 0.04045f) ? (s / 12.92f) : (float)Math.Pow((s + 0.055f) / 1.055f, 2.4f);
            int value = (int)Math.Round(linear * 255f);
            if (value < 0) value = 0;
            if (value > 255) value = 255;
            return (byte)value;
        }
        private int GetSwizzleOffset(int x, int y, int width, int bpp, int blockHeight)
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

        // MII TOOL
        private int GetActualOffset(byte[] fileData, string hashHex)
        {
            byte[] hash = Enumerable.Range(0, hashHex.Length / 2)
                                    .Select(x => Convert.ToByte(hashHex.Substring(x * 2, 2), 16))
                                    .Reverse().ToArray();

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
                        deswizzledData[linearOffset + 0] = rawData[swizzledOffset + 2];
                        deswizzledData[linearOffset + 1] = rawData[swizzledOffset + 1];
                        deswizzledData[linearOffset + 2] = rawData[swizzledOffset + 0];
                        deswizzledData[linearOffset + 3] = rawData[swizzledOffset + 3];
                    }
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(deswizzledData, 0, bmpData.Scan0, deswizzledData.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        private void lstUGC_SelectedIndexChanged(object sender, EventArgs e)
        {
            string? selectedFile = GetSelectedUgcFileName();
            if (string.IsNullOrWhiteSpace(selectedFile)) return;
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

                if (selectedFile.EndsWith(".canvas.zs"))
                {
                    int size = (int)Math.Sqrt(decompressed.Length / 4);

                    // 1. Decode the dark, mathematically-correct image
                    Bitmap rawDecoded = DecodeRawTexture(decompressed, size, size);

                    // 2. Create the brightened Preview Filter
                    Bitmap previewImage = new Bitmap(rawDecoded.Width, rawDecoded.Height);
                    using (Graphics g = Graphics.FromImage(previewImage))
                    {
                        using (System.Drawing.Imaging.ImageAttributes attr = new System.Drawing.Imaging.ImageAttributes())
                        {
                            attr.SetGamma(0.4545f); // Reverse the Linear curve for Windows
                            g.DrawImage(rawDecoded,
                                new Rectangle(0, 0, previewImage.Width, previewImage.Height),
                                0, 0, rawDecoded.Width, rawDecoded.Height,
                                GraphicsUnit.Pixel, attr);
                        }
                    }

                    // 3. Assign the bright image to the PictureBox and clean up the dark one
                    picPreview.Image = previewImage;
                    rawDecoded.Dispose();

                    lblImageInfo.Text = $"{selectedFile} ({size}x{size} Decoded)";
                }
                else if (selectedFile.EndsWith(".ugctex.zs"))
                {
                    if (!selectedFile.Contains("thumb", StringComparison.OrdinalIgnoreCase))
                    {
                        lblImageInfo.Text = $"{selectedFile} (ignored: only thumbnail ugctex is supported)";
                        return;
                    }

                    int blockCount = decompressed.Length / 16;
                    int gridSize = (int)Math.Sqrt(blockCount);
                    bool validSquareGrid = gridSize > 0 && (gridSize * gridSize) == blockCount;

                    int actualWidth = validSquareGrid ? gridSize * 4 : 256;
                    int actualHeight = actualWidth;

                    picPreview.Image = DecodeBc3SwizzledTexture(decompressed, actualWidth, actualHeight, 8);
                    lblImageInfo.Text = picPreview.Image == null
                        ? $"{selectedFile} (BC3 Decode Failed)"
                        : $"{selectedFile} ({actualWidth}x{actualHeight} BC3 Decoded)";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private Bitmap DecodeBc3SwizzledTexture(byte[] rawData, int width, int height, int blockHeight)
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
            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Rectangle rect = new Rectangle(0, 0, width, height);
            var bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, bmp.PixelFormat);
            System.Runtime.InteropServices.Marshal.Copy(bgra, 0, bmpData.Scan0, bgra.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        private byte[] DecodeBc3LinearToBgra(byte[] bc3Blocks, int width, int height)
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

                            // Bitmap Format32bppArgb expects BGRA byte order in memory.
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
                table[2] = (byte)((6 * alpha0 + 1 * alpha1) / 7);
                table[3] = (byte)((5 * alpha0 + 2 * alpha1) / 7);
                table[4] = (byte)((4 * alpha0 + 3 * alpha1) / 7);
                table[5] = (byte)((3 * alpha0 + 4 * alpha1) / 7);
                table[6] = (byte)((2 * alpha0 + 5 * alpha1) / 7);
                table[7] = (byte)((1 * alpha0 + 6 * alpha1) / 7);
            }
            else
            {
                table[2] = (byte)((4 * alpha0 + 1 * alpha1) / 5);
                table[3] = (byte)((3 * alpha0 + 2 * alpha1) / 5);
                table[4] = (byte)((2 * alpha0 + 3 * alpha1) / 5);
                table[5] = (byte)((1 * alpha0 + 4 * alpha1) / 5);
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
        private void ExportMii(int slot, string name)
        {
            SaveFileDialog sfd = new SaveFileDialog { Filter = "LtD Mii (*.ltd)|*.ltd", FileName = name + ".ltd" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                byte[] miiBytes = File.ReadAllBytes(currentMiiSavPath);
                string? saveDir = Path.GetDirectoryName(currentMiiSavPath);
                if (string.IsNullOrWhiteSpace(saveDir))
                {
                    MessageBox.Show("Save folder path is invalid.", "TomoAIO");
                    return;
                }

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
                        pkgList[uIdx] = 0xA4; pkgList[uIdx + 1] = 0xA4; pkgList[uIdx + 2] = 0xA4;
                        pkgList.Insert(uIdx + 3, 0xA4);
                    }
                }
            }

            byte[] pkg = pkgList.ToArray();
            byte[] miiBytes = File.ReadAllBytes(currentMiiSavPath);
            string? saveDir = Path.GetDirectoryName(currentMiiSavPath);
            if (string.IsNullOrWhiteSpace(saveDir))
            {
                MessageBox.Show("Save folder path is invalid.", "TomoAIO");
                return;
            }
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

                    Array.Copy(new byte[] { 255, 255, 255, 255 }, 0, miiBytes, fO + (slot * 4), 4);

                    // Clear old facepaint 
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
            string? sD = Path.GetDirectoryName(currentMiiSavPath);
            if (string.IsNullOrWhiteSpace(sD) || !Directory.Exists(sD))
            {
                return;
            }
            string bD = Path.Combine(Application.StartupPath, "backup", DateTime.Now.ToString("dd_MM_yyyy_HHmmss"));
            Directory.CreateDirectory(bD);
            string[] safeFiles = { "Map.sav", "Mii.sav", "Player.sav" };
            foreach (string file in safeFiles)
            {
                string sourceFile = Path.Combine(sD, file);
                if (File.Exists(sourceFile))
                {
                    File.Copy(sourceFile, Path.Combine(bD, file), true);
                }
            }
            string sourceUgc = Path.Combine(sD, "Ugc");
            if (Directory.Exists(sourceUgc))
            {
                string destUgc = Path.Combine(bD, "Ugc");
                Directory.CreateDirectory(destUgc);

                foreach (string f in Directory.GetFiles(sourceUgc))
                {
                    File.Copy(f, Path.Combine(destUgc, Path.GetFileName(f)), true);
                }
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

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            panel1.BringToFront();
            PinLogoTopRight();
            LayoutMiiEditorControls();
            EnsureDiscordButtonVisible();

            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select your game save folder (where Mii.sav is located)";

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    string selectedFolder = fbd.SelectedPath;
                    string miiSavPath = Path.Combine(selectedFolder, "Mii.sav");

                    if (File.Exists(miiSavPath))
                    {
                        currentMiiSavPath = miiSavPath;
                        RefreshMiiList();
                        MessageBox.Show("Save file found!", "TomoAIO");
                    }
                    else
                    {
                        MessageBox.Show("Could not find Mii.sav in that folder!\n\nPlease make sure you selected the correct save folder.", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }
        private void button3_Click(object sender, EventArgs e) { ShowMainMenu(); }
        private void button2_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select your Living the Dream 'Ugc' folder";
                if (fbd.ShowDialog() == DialogResult.OK)
                {

                    LoadUgcList(fbd.SelectedPath);


                    if (lstUGC.Items.Count > 0)
                    {
                        panelUGC.Visible = true;
                        panelUGC.BringToFront();
                        LayoutUgcEditorControls();
                        EnsureDiscordButtonVisible();
                    }

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
            if (listBox1.SelectedItem == null || string.IsNullOrWhiteSpace(_selectedMiiAction))
            {
                MessageBox.Show("Please select a Mii and an action!", "Missing Information");
                return;
            }

            string? sel = listBox1.SelectedItem.ToString();
            if (string.IsNullOrWhiteSpace(sel) || !sel.Contains(':'))
            {
                MessageBox.Show("Selected Mii entry is invalid.", "TomoAIO");
                return;
            }
            int slot = int.Parse(sel.Split(':')[0]) - 1;
            string name = sel.Split(':')[1].Trim();

            if (_selectedMiiAction.Contains("Export"))
            {
                ExportMii(slot, name);
            }
            else
            {
                ImportMii(slot, _selectedMiiPath);
            }
        }

        private void btnBrowseMii_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Mii Data Files (*.mii;*.ltd;*.sav)|*.mii;*.ltd;*.sav";
                if (ofd.ShowDialog() == DialogResult.OK) SetSelectedMiiPath(ofd.FileName);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void Form1_Load(object sender, EventArgs e)
        {
            // Check for updates silently in the background the second the app opens!
            CheckForUpdates();

            lblImageInfo.Text = "Waiting for selection...";
            PinLogoTopRight();
            LayoutMainMenuButtons();
            LayoutMiiEditorControls();
            LayoutUgcEditorControls();
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            PinLogoTopRight();
            LayoutMainMenuButtons();
            LayoutMiiEditorControls();
            LayoutUgcEditorControls();
        }
        private void pictureBox3_Click(object sender, EventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void txtMiiPath_TextChanged(object sender, EventArgs e)
        {
            if (_pathDisplay != null && _pathDisplay.Text != txtMiiPath.Text)
            {
                _pathDisplay.Text = txtMiiPath.Text;
            }
        }
        private void txtMiiPath_DragEnter(object sender, DragEventArgs e) { if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy; }
        private void txtMiiPath_DragDrop(object sender, DragEventArgs e)
        {
            string[] f = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (f.Length > 0) SetSelectedMiiPath(f[0]);
        }
        private void label2_Click(object sender, EventArgs e) { }
        private void btnUgcExport_Click(object sender, EventArgs e)
        {
            string? selectedFile = GetSelectedUgcFileName();
            if (string.IsNullOrWhiteSpace(selectedFile)) return;
            string sourcePath = Path.Combine(currentUgcPath, selectedFile);

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Export UGC File";

                // allows png export & .zs export
                sfd.Filter = "PNG Image (*.png)|*.png|Zstandard Game File (*.zs)|*.zs";
                string cleanName = selectedFile.Replace(".canvas.zs", "").Replace(".ugctex.zs", "").Replace(".zs", "");
                sfd.FileName = cleanName + "_export";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (sfd.FileName.EndsWith(".png"))
                        {
                            if (picPreview.Image == null)
                            {
                                MessageBox.Show("There is no image loaded to export as a PNG!", "Error");
                                return;
                            }
                            picPreview.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        else if (sfd.FileName.EndsWith(".zs"))
                        {
                            File.Copy(sourcePath, sfd.FileName, true);
                        }

                        MessageBox.Show("File successfully exported to your PC!", "Success");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to export file: " + ex.Message, "Error");
                    }
                }
            }
        }
        private void lblImageInfo_Click(object sender, EventArgs e) { }
        private void btnUgcImport_Click(object sender, EventArgs e)
        {
            string? selectedFile = GetSelectedUgcFileName();
            if (string.IsNullOrWhiteSpace(selectedFile)) return;
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "UGC Files (*.png;*.jpg;*.zs)|*.png;*.jpg;*.zs" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string fullPath = Path.Combine(currentUgcPath, selectedFile);
                        if (ofd.FileName.EndsWith(".zs"))
                        {
                            File.Copy(ofd.FileName, fullPath, true);
                            MessageBox.Show("Canvas/Texture file successfully replaced!", "Success");
                            lstUGC_SelectedIndexChanged(sender, e);
                            return;
                        }
                        byte[] origBytes = File.ReadAllBytes(fullPath);
                        byte[] origDecomp;
                        using (var decompressor = new ZstdSharp.Decompressor())
                        {
                            origDecomp = decompressor.Unwrap(origBytes).ToArray();
                        }
                        int expectedSize = (int)Math.Sqrt(origDecomp.Length / 4);

                        Bitmap sourceImage = new Bitmap(ofd.FileName);
                        Bitmap processedImage = new Bitmap(expectedSize, expectedSize, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                        using (Graphics g = Graphics.FromImage(processedImage))
                        {
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            g.DrawImage(sourceImage,
                                new Rectangle(0, 0, expectedSize, expectedSize),
                                0, 0, sourceImage.Width, sourceImage.Height,
                                GraphicsUnit.Pixel);
                        }
                        byte[] rawSwizzled = EncodeRawTexture(processedImage, convertSrgbToLinear: true);
                        byte[] compressedData;
                        using (var compressor = new ZstdSharp.Compressor(9))
                        {
                            compressedData = compressor.Wrap(rawSwizzled).ToArray();
                        }
                        File.WriteAllBytes(fullPath, compressedData);
                        sourceImage.Dispose();
                        processedImage.Dispose();

                        MessageBox.Show("Custom texture successfully resized and injected!", "Success");
                        lstUGC_SelectedIndexChanged(sender, e);
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

            if (picPreview.Image != null)
            {
                picPreview.Image.Dispose();
                picPreview.Image = null;
            }
            lblImageInfo.Text = "Waiting for selection...";
            ShowMainMenu();
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CloseActionDropdown();
            SuspendLayout();
            panel1.SuspendLayout();
            panelUGC.SuspendLayout();
            try
            {
                PinLogoTopRight();
                LayoutMainMenuButtons();
                LayoutMiiEditorControls();
                LayoutUgcEditorControls();
                EnsureDiscordButtonVisible();
            }
            finally
            {
                panelUGC.ResumeLayout(false);
                panel1.ResumeLayout(false);
                ResumeLayout(false);
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {

        }

        private void btnDiscord_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://discord.gg/JmEttdwE5J") { UseShellExecute = true });
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text) || txtSearch.Text == "Search...")
            {
                lstUGC.Items.Clear();
                ugcDisplayToFile.Clear();
                foreach (string file in allUgcFiles)
                {
                    AddUgcListEntry(file);
                }
                return;
            }
            string searchTerm = txtSearch.Text.ToLower();
            lstUGC.Items.Clear();
            ugcDisplayToFile.Clear();

            foreach (string file in allUgcFiles)
            {
                string display = GetUgcDisplayName(file);
                if (display.ToLower().Contains(searchTerm))
                {
                    AddUgcListEntry(file);
                }
            }
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Search...")
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.Black;
            }
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Search...";
                txtSearch.ForeColor = Color.Gray;
            }
        }

        private void picPreview_Click(object sender, EventArgs e)
        {

        }
    }
} 