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
using TomoAIO.Infrastructure;
using TomoAIO.Models;
using TomoAIO.Services;
using TomoAIO.Views;

namespace TomoAIO
{
    public partial class Form1 : Form
    {
        private List<string> allUgcFiles = new List<string>();
        private readonly Dictionary<string, string> ugcDisplayToFile = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly AppState _state = new();
        private readonly ZstdCodec _zstdCodec = new();
        private readonly FileSystemGateway _fileSystem = new();
        private readonly TextureService _textureService = new();
        private readonly MiiService _miiService = new();
        private readonly UpdateService _updateService = new();
        private UgcService _ugcService;
        private readonly LayoutService _layoutService = new();
        private MainMenuView? _mainMenuView;
        private MiiEditorView? _miiEditorView;
        private UgcCreatorView? _ugcCreatorView;
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
        private readonly string[] _miiActions = { "Import Mii (.ltd)", "Export Mii (.ltd)" };
        private readonly Dictionary<Control, (Size size, int radius)> _roundedCache = new Dictionary<Control, (Size size, int radius)>();

        string[] persHashes = {
            "43CD364F", "CD8DBAF8", "25B48224", "607BA160", "68E1134E", // Personality P1-P5
            "4913AE1A", "141EE086", "07B9D175", "81CF470A", "4D78E262", "FBC3FFB0", // Voice V1-V6
            "236E2D73", "F3C3DE59", "660C5247", // Gender (S1), Pronoun (S2), Style (S3)
            "5D7D3F45", "AB8AE08B", "2545E583", "6CF484F4" // Birthday B1-B4
        };

        public Form1()
        {
            _ugcService = new UgcService(_fileSystem, _zstdCodec);
            InitializeComponent();
            StyleSpecificAssets();
            CheckFirstBootSetup();
            _mainMenuView = new MainMenuView(pictureBox1, logo, button1, button2);
            _miiEditorView = new MiiEditorView(panelMii);
            _ugcCreatorView = new UgcCreatorView(panelUGC);
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
                string currentVersion = "2.0";
                string repoOwner = "ohamir";
                string repoName = "TomoAIO";
                string? latestVersion = await _updateService.GetLatestVersionAsync(repoOwner, repoName);
                if (!string.IsNullOrWhiteSpace(latestVersion) && latestVersion != currentVersion)
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
            catch
            {
            }
        }

        private void EnableSmoothRendering()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            EnableDoubleBuffer(panelMii);
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
            btnIslandMgmt.Parent = pictureBox1;
            btnChangeFolders.Parent = pictureBox1;
            ApplyLogoStyle(logo, pictureBox1);
            ApplyLogoStyle(logopanel1, pictureBox2);
            ApplyLogoStyle(pictureBox3, panelUGC);
            ApplyLogoStyle(logoislandmgt, pictureBox4);
            pictureBox1.SendToBack();
            logo.BringToFront();
            pictureBox2.SendToBack();
            logopanel1.BringToFront();

            ConfigureTransparentButton(button1);
            ConfigureTransparentButton(button2);
            ConfigureTransparentButton(btnIslandMgmt);
            MakePictureBackgroundTransparent(logo, Color.FromArgb(255, 190, 0), 38);
            MakePictureBackgroundTransparent(pictureBox3, Color.FromArgb(255, 190, 0), 38);
            MakePictureBackgroundTransparent(logopanel1, Color.FromArgb(255, 190, 0), 38);
            MakePictureBackgroundTransparent(logoislandmgt, Color.FromArgb(255, 190, 0), 38);
            MakePictureBackgroundTransparent(logo, Color.White, 65);
            MakePictureBackgroundTransparent(logoislandmgt, Color.White, 65);
            MakePictureBackgroundTransparent(pictureBox3, Color.White, 65);
            MakePictureBackgroundTransparent(logopanel1, Color.White, 65);
            logo.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox3.BackgroundImageLayout = ImageLayout.Zoom;
            logopanel1.BackgroundImageLayout = ImageLayout.Zoom;
            logoislandmgt.BackgroundImageLayout = ImageLayout.Zoom;
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
            if (btnIslandMgmt.BackgroundImage == null)
            {
                btnIslandMgmt.BackgroundImage = Properties.Resources.islandmanager;
            }
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

            if (logoislandmgt.BackgroundImage == null)
            {
                logoislandmgt.BackgroundImage = Properties.Resources.tomoaio_logo;
            }

            if (pictureBox3.BackgroundImage == null)
            {
                pictureBox3.BackgroundImage = Properties.Resources.tomoaio_logo;
            }

            ApplyLogoStyle(logo, pictureBox1);
            ApplyLogoStyle(logopanel1, pictureBox2);
            ApplyLogoStyle(pictureBox3, panelUGC);
            ApplyLogoStyle(logoislandmgt, pictureBox4);
            PinLogoTopRight();
        }

        private void ShowMainMenu()
        {
            panelMii.Visible = false;
            panelUGC.Visible = false;
            button1.Visible = true;
            button2.Visible = true;
            btnChangeFolders.Visible = true;
            btnIslandMgmt.Visible = true;
            pictureBox1.Visible = true;
            btnChangeFolders.BringToFront();
            CloseActionDropdown();
            EnsureMenuImages();
            _mainMenuView?.Show();
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

            btnDiscord.Location = _layoutService.ComputeBottomRight(btnDiscord.Parent.ClientSize, btnDiscord.Size, DiscordMargin);
        }

        private void PinLogoTopRight()
        {
            float scaleX = ClientSize.Width / (float)_baseClientSize.Width;
            float scaleY = ClientSize.Height / (float)_baseClientSize.Height;

            if (logo.Parent != null)
            {
                (Size size, Point location) = _layoutService.ComputeTopRightLogo(logo.Parent.ClientSize, scaleX, scaleY);
                logo.Size = size;
                logo.Location = location;
            }

            if (logopanel1.Parent != null)
            {
                (Size size, Point location) = _layoutService.ComputeTopRightLogo(logopanel1.Parent.ClientSize, scaleX, scaleY);
                logopanel1.Size = size;
                logopanel1.Location = location;
            }

            if (pictureBox3.Parent != null)
            {
                (Size size, Point location) = _layoutService.ComputeTopRightLogo(pictureBox3.Parent.ClientSize, scaleX, scaleY);
                pictureBox3.Size = size;
                pictureBox3.Location = location;
            }
            if (logoislandmgt.Parent != null)
            {
                (Size size, Point location) = _layoutService.ComputeTopRightLogo(logoislandmgt.Parent.ClientSize, scaleX, scaleY);
                logoislandmgt.Size = size;
                logoislandmgt.Location = location;
            }
      
        }

        private void LayoutMainMenuButtons()
        {
            int clientWidth = ClientSize.Width;
            int clientHeight = ClientSize.Height;
            if (clientWidth <= 0 || clientHeight <= 0) return;

            float scaleX = clientWidth / (float)_baseClientSize.Width;
            float scaleY = clientHeight / (float)_baseClientSize.Height;
            float scale = Math.Max(0.5f, Math.Min(scaleX, scaleY));

            int buttonWidth = (int)Math.Round(_baseButtonSize.Width * scale);
            int buttonHeight = (int)Math.Round(_baseButtonSize.Height * scale);
            int gap = (int)Math.Round(BaseButtonGap * scale);
            int totalWidth = (buttonWidth * 3) + (gap * 2);
            int reservedRight = GetRightTopReservedWidth();
            int usableWidth = Math.Max(260, clientWidth - reservedRight);
            int startX = Math.Max(20, (usableWidth - totalWidth) / 2);
            int y = Math.Max(110, (clientHeight - buttonHeight) / 2);

            button1.Size = button2.Size = btnIslandMgmt.Size = new Size(buttonWidth, buttonHeight);

            button1.Location = new Point(startX, y);
            button2.Location = new Point(startX + buttonWidth + gap, y);
            btnIslandMgmt.Location = new Point(startX + (buttonWidth + gap) * 2, y);
        }

        private void LayoutMiiEditorControls()
        {
            if (_comboHost == null || _pathHost == null || _pathDisplay == null || _actionDropdownText == null || _actionDropdownArrow == null || _actionDropdownList == null)
            {
                return;
            }

            int panelWidth = Math.Max(1, panelMii.ClientSize.Width);
            int panelHeight = Math.Max(1, panelMii.ClientSize.Height);

            float scaleX = panelWidth / 1343f;
            float scaleY = panelHeight / 745f;
            float uiScale = Math.Max(0.8f, Math.Min(1.35f, Math.Min(scaleX, scaleY)));

            int outerMargin = Math.Max(14, (int)Math.Round(24 * uiScale));
            int top = Math.Max(10, (int)Math.Round(12 * uiScale));
            int rowGap = Math.Max(8, (int)Math.Round(12 * uiScale));

            int reservedRight = panelMii.Visible ? GetRightTopReservedWidth() : 0;
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
            int listHeight = Math.Max(120, panelMii.ClientSize.Height - listBox1.Top - 20);
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

            if (logopanel1.Visible && logopanel1.Parent == panelMii)
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
            StyleActionButton(btnChangeFolders, ButtonSecondaryColor, ButtonSecondaryHoverColor, ButtonSecondaryPressedColor);
            StyleActionButton(btnMenuBack, ButtonSecondaryColor, ButtonSecondaryHoverColor, ButtonSecondaryPressedColor);
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
                panelMii.Controls.Add(_comboHost);
                _comboHost.BringToFront();
            }

            if (_pathHost == null)
            {
                _pathHost = new Panel
                {
                    BackColor = InputBorderColor,
                    Padding = new Padding(1)
                };
                panelMii.Controls.Add(_pathHost);
                _pathHost.BringToFront();
            }

            if (_pathDisplay == null)
            {
                _pathDisplay = new Label
                {
                    Text = _state.SelectedMiiPath,
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
            _state.SelectedMiiPath = path;
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
                        _state.SelectedMiiAction = action;
                        _actionDropdownText!.Text = action;
                    }
                    CloseActionDropdown();
                };
                panelMii.Controls.Add(_actionDropdownList);
            }

            panelMii.MouseDown -= PanelMouseDownCloseDropdown;
            panelMii.MouseDown += PanelMouseDownCloseDropdown;
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
            _state.CurrentUgcPath = exactUgcPath;
            lstUGC.Items.Clear();
            allUgcFiles.Clear();
            ugcDisplayToFile.Clear();
            _state.UgcFiles.Clear();

            if (_fileSystem.DirectoryExists(exactUgcPath))
            {
                List<UgcFileItem> files = _ugcService.DiscoverUgcFiles(exactUgcPath);

                if (files.Count == 0)
                {
                    MessageBox.Show("Please make sure you selected the right folder", "Folder Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                _state.UgcFiles.AddRange(files);
                foreach (UgcFileItem file in files)
                {
                    allUgcFiles.Add(file.FileName);
                    AddUgcListEntry(file);
                }
            }
        }

        private void AddUgcListEntry(UgcFileItem item)
        {
            string displayName = item.DisplayName;
            string uniqueDisplay = displayName;
            int suffix = 2;
            while (ugcDisplayToFile.ContainsKey(uniqueDisplay))
            {
                uniqueDisplay = $"{displayName} ({suffix})";
                suffix++;
            }

            ugcDisplayToFile[uniqueDisplay] = item.FileName;
            lstUGC.Items.Add(uniqueDisplay);
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
        private void lstUGC_SelectedIndexChanged(object sender, EventArgs e)
        {
            string? selectedFile = GetSelectedUgcFileName();
            if (string.IsNullOrWhiteSpace(selectedFile)) return;
            string fullPath = Path.Combine(_state.CurrentUgcPath, selectedFile);

            try
            {
                using (var imageSharpImg = TextureProcessor.DecodeFile(fullPath))
                {
                    using (var ms = new MemoryStream())
                    {
                        imageSharpImg.Save(ms, new SixLabors.ImageSharp.Formats.Png.PngEncoder());

                        if (picPreview.Image != null) picPreview.Image.Dispose();

                        picPreview.Image = new Bitmap(ms);
                    }
                    lblImageInfo.Text = $"{selectedFile} ({imageSharpImg.Width}x{imageSharpImg.Height} Decoded)";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading preview: " + ex.Message, "TomoAIO");
            }
        }
        private void ExportMii(int slot, string name)
        {
            SaveFileDialog sfd = new SaveFileDialog { Filter = "LtD Mii (*.ltd)|*.ltd", FileName = name + ".ltd" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                byte[] miiBytes = File.ReadAllBytes(_state.CurrentMiiSavPath);
                string? saveDir = Path.GetDirectoryName(_state.CurrentMiiSavPath);
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
            byte[] miiBytes = File.ReadAllBytes(_state.CurrentMiiSavPath);
            string? saveDir = Path.GetDirectoryName(_state.CurrentMiiSavPath);
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
                int nameOffset = GetActualOffset(miiBytes, "2499BFDA") + 4 + (slot * 64);
                byte[] rawName = br.ReadBytes(64);
                Array.Clear(miiBytes, nameOffset, 64);
                int validNameLen = 64;
                for (int i = 0; i < 63; i += 2)
                {
                    if (rawName[i] == 0 && rawName[i + 1] == 0)
                    {
                        validNameLen = i + 2;
                        break;
                    }
                }
                Array.Copy(rawName, 0, miiBytes, nameOffset, validNameLen);
                int creatorOffset = GetActualOffset(miiBytes, "3A5EDA05") + 4 + (slot * 128);
                byte[] rawCreator = br.ReadBytes(128);
                Array.Clear(miiBytes, creatorOffset, 128);

                int validCreatorLen = 128;
                for (int i = 0; i < 127; i += 2)
                {
                    if (rawCreator[i] == 0 && rawCreator[i + 1] == 0)
                    {
                        validCreatorLen = i + 2;
                        break;
                    }
                }
                Array.Copy(rawCreator, 0, miiBytes, creatorOffset, validCreatorLen);
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

        private void CreateSaveBackup()
        {
            string? sD = Path.GetDirectoryName(_state.CurrentMiiSavPath);
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
            if (string.IsNullOrEmpty(_state.CurrentMiiSavPath) || !File.Exists(_state.CurrentMiiSavPath)) return;
            byte[] miiBytes = File.ReadAllBytes(_state.CurrentMiiSavPath);
            int nO = GetActualOffset(miiBytes, "2499BFDA") + 4;
            int dO = GetActualOffset(miiBytes, "881CA27A") + 4;
            listBox1.Items.Clear();
            List<MiiEntry> entries = _miiService.BuildMiiEntries(miiBytes, nO, dO);
            foreach (MiiEntry entry in entries)
            {
                listBox1.Items.Add(entry.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _miiEditorView?.Show();
            PinLogoTopRight();
            LayoutMiiEditorControls();
            EnsureDiscordButtonVisible();
            string savedFolder = Properties.Settings.Default.SaveFolder;
            if (string.IsNullOrEmpty(savedFolder))
            {
                MessageBox.Show("Save folder not set! Please restart the app to trigger the First Time Setup.", "Missing Folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string miiSavPath = Path.Combine(savedFolder, "Mii.sav");
            if (File.Exists(miiSavPath))
            {
                _state.CurrentMiiSavPath = miiSavPath;
                RefreshMiiList();
            }
            else
            {
                MessageBox.Show($"Could not find Mii.sav in your saved folder:\n{savedFolder}\n\nPlease make sure you didn't move your game files.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button3_Click(object sender, EventArgs e) { ShowMainMenu(); }
        private void button2_Click(object sender, EventArgs e)
        {
            string savedUgcFolder = Properties.Settings.Default.UGCFolder;
            if (string.IsNullOrEmpty(savedUgcFolder))
            {
                MessageBox.Show("UGC folder not set! Please restart the app to trigger the First Time Setup.", "Missing Folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (Directory.Exists(savedUgcFolder))
            {
                LoadUgcList(savedUgcFolder);

                if (lstUGC.Items.Count > 0)
                {
                    _ugcCreatorView?.Show();
                    LayoutUgcEditorControls();
                    EnsureDiscordButtonVisible();
                }
                else
                {
                    MessageBox.Show("No UGC items were found in your saved folder.", "Empty Folder", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show($"Could not find your saved UGC folder:\n{savedUgcFolder}\n\nPlease make sure you didn't move or delete the folder.", "Folder Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fb = new FolderBrowserDialog())
            {
                if (fb.ShowDialog() == DialogResult.OK)
                {
                    _state.CurrentMiiSavPath = Path.Combine(fb.SelectedPath, "Mii.sav");
                    RefreshMiiList();
                    MessageBox.Show("Save file found!", "TomoAIO");
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null || string.IsNullOrWhiteSpace(_state.SelectedMiiAction))
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

            if (_state.SelectedMiiAction!.Contains("Export"))
            {
                ExportMii(slot, name);
            }
            else
            {
                ImportMii(slot, _state.SelectedMiiPath);
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
        private void txtMiiPath_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }
        private void txtMiiPath_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data?.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
            {
                SetSelectedMiiPath(files[0]);
            }
        }
        private void label2_Click(object sender, EventArgs e) { }
        private void btnUgcExport_Click(object sender, EventArgs e)
        {
            string? selectedFile = GetSelectedUgcFileName();
            if (string.IsNullOrWhiteSpace(selectedFile)) return;
            string sourcePath = Path.Combine(_state.CurrentUgcPath, selectedFile);

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Export UGC File";
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
                            _fileSystem.CopyFile(sourcePath, sfd.FileName, true);
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
                        string fullPath = Path.Combine(_state.CurrentUgcPath, selectedFile);
                        if (ofd.FileName.EndsWith(".zs", StringComparison.OrdinalIgnoreCase))
                        {
                            _ugcService.ReplaceFromZs(ofd.FileName, fullPath);
                            MessageBox.Show("Canvas/Texture file successfully replaced!", "Success");
                            lstUGC_SelectedIndexChanged(sender, e);
                            return;
                        }
                        string destStem = fullPath.Replace(".canvas.zs", "", StringComparison.OrdinalIgnoreCase)
                                                  .Replace(".ugctex.zs", "", StringComparison.OrdinalIgnoreCase);

                        string originalUgcPath = destStem + ".ugctex.zs";
                        bool isThumb = destStem.EndsWith("_Thumb", StringComparison.OrdinalIgnoreCase);
                        TextureProcessor.ImportPng(
                            pngPath: ofd.FileName,
                            destStem: destStem,
                            writeCanvas: !isThumb,
                            writeThumb: !isThumb,
                            noSrgb: false,
                            originalUgctexPath: File.Exists(originalUgcPath) ? originalUgcPath : null
                        );
                        MessageBox.Show("Custom texture successfully converted and injected!", "Success");
                        lstUGC_SelectedIndexChanged(sender, e);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Import Error: " + ex.Message, "Error");
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
            panelMii.SuspendLayout();
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
                panelMii.ResumeLayout(false);
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
                foreach (UgcFileItem file in _state.UgcFiles)
                {
                    AddUgcListEntry(file);
                }
                return;
            }
            string searchTerm = txtSearch.Text.ToLower();
            lstUGC.Items.Clear();
            ugcDisplayToFile.Clear();

            foreach (UgcFileItem file in _state.UgcFiles)
            {
                if (file.DisplayName.ToLower().Contains(searchTerm))
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

        private void btnIslandMgmt_Click(object sender, EventArgs e)
        {
            string savedFolder = Properties.Settings.Default.SaveFolder;
            if (string.IsNullOrEmpty(savedFolder))
            {
                MessageBox.Show("Save folder not set! Please restart the app to trigger the First Time Setup.", "Missing Folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string playerSavPath = Path.Combine(savedFolder, "Player.sav");
            if (File.Exists(playerSavPath))
            {
                _state.CurrentPlayerSavPath = playerSavPath;
                button1.Visible = false;
                button2.Visible = false;
                btnIslandMgmt.Visible = false;
                pictureBox1.Visible = false;
                btnChangeFolders.Visible = false;

                panelIslandMGT.Visible = true;
                panelIslandMGT.Dock = DockStyle.Fill;
                panelIslandMGT.BringToFront();

                RefreshIslandManagementUI();
                PinLogoTopRight();
                EnsureDiscordButtonVisible();
            }
            else
            {
                MessageBox.Show($"Could not find Player.sav in your saved folder:\n{savedFolder}\n\nPlease make sure your save files are intact.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void label3_Click(object sender, EventArgs e)
        {

        }
        private void numMoney_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnSaveMoney_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_state.CurrentPlayerSavPath) || !File.Exists(_state.CurrentPlayerSavPath)) return;

            try
            {
                byte[] pBytes = File.ReadAllBytes(_state.CurrentPlayerSavPath);
                byte[] hash = Enumerable.Range(0, 4).Select(x => Convert.ToByte("365FAB1F".Substring(x * 2, 2), 16)).Reverse().ToArray();
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
                    int totalCents = (int)(numMoney.Value * 100);
                    byte[] moneyData = BitConverter.GetBytes(totalCents);
                    Array.Copy(moneyData, 0, pBytes, valueIndex, 4);
                    File.WriteAllBytes(_state.CurrentPlayerSavPath, pBytes);

                    MessageBox.Show($"Success! Your island funds have been set to ${numMoney.Value:N2}", "Island Funds Updated  ");
                    RefreshIslandManagementUI();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update money: " + ex.Message);
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
                byte[] obtainedState = new byte[] { 0x7C, 0xEF, 0x5F, 0xBC };
                bool fileWasModified = false;
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
                                {
                                    Array.Copy(obtainedState, 0, pBytes, dataStart + (j * 4), 4);
                                }
                                fileWasModified = true;
                            }
                        }
                    }
                }
                if (unlockRooms)
                {
                    string hashFilePath = Path.Combine(Application.StartupPath, "services", "room_hashes.csv");
                    if (File.Exists(hashFilePath))
                    {
                        string[] roomHashes = File.ReadAllLines(hashFilePath);
                        foreach (string line in roomHashes)
                        {
                            string hashHex = line.Trim();
                            if (hashHex.Equals("Hash", StringComparison.OrdinalIgnoreCase) || hashHex.Length != 8)
                                continue;

                            byte[] hashBytes = Enumerable.Range(0, 4).Select(x => Convert.ToByte(hashHex.Substring(x * 2, 2), 16)).Reverse().ToArray();

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
                MessageBox.Show("Unlock Error: " + ex.Message);
            }
        }

        private void RefreshIslandManagementUI()
        {
            if (string.IsNullOrEmpty(_state.CurrentPlayerSavPath) || !File.Exists(_state.CurrentPlayerSavPath)) return;

            try
            {
                byte[] pBytes = File.ReadAllBytes(_state.CurrentPlayerSavPath);
                int totalCents = GetActualOffset(pBytes, "365FAB1F");
                if (totalCents >= 0)
                {
                    decimal dollars = (decimal)totalCents / 100;
                    lblCurrentMoney.Text = $"Balance: {dollars:C2}";
                    numMoney.Value = Math.Min(numMoney.Maximum, dollars);
                }
                int nameOffset = GetActualOffset(pBytes, "D46DF986");
                if (nameOffset > 0)
                {
                    byte[] nameBytes = pBytes.Skip(nameOffset).Take(64).ToArray();
                    string islandName = System.Text.Encoding.Unicode.GetString(nameBytes);

                    int nullIndex = islandName.IndexOf('\0');
                    if (nullIndex != -1) islandName = islandName.Substring(0, nullIndex);
                    // lblCurrentIslandName.Text = $"Island: {islandName}"; <- not needed at this time or it would show the island name twice
                    if (lblIslandTitle != null) lblIslandTitle.Text = $"{islandName} Management";
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void LoadCurrentMoney()
        {
            RefreshIslandManagementUI();
        }
        private void btnMenuBack_Click1(object sender, EventArgs e)
        {
            panelIslandMGT.Visible = false;
            ShowMainMenu();
        }

        private void btnUnlockInteriors_Click(object sender, EventArgs e)
        {
            UnlockSpecificCategory(null, true, "Interiors");
        }

        private void btnUnlockQBuilds_Click(object sender, EventArgs e)
        {
            UnlockSpecificCategory(new string[] { "3AF3C005" }, false, "Quik Builds");
        }

        private void btnUnlockClothes_Click(object sender, EventArgs e)
        {
            UnlockSpecificCategory(new string[] { "D273AD77", "7708017B" }, false, "Clothes");
        }

        private void btnUnlockFood_Click(object sender, EventArgs e)
        {
            UnlockSpecificCategory(new string[] { "933DA780" }, false, "Foods");
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void IslandFund_Click(object sender, EventArgs e)
        {

        }
        private void StyleSpecificAssets()
        {
            Button[] unlockButtons = { btnUnlockClothes, btnUnlockFood, btnUnlockQBuilds, btnUnlockInteriors };
            Color exactBlue = ColorTranslator.FromHtml("#2F3D52");
            foreach (Button btn in unlockButtons)
            {
                btn.Dock = DockStyle.None;
                btn.Anchor = AnchorStyles.None;
                btn.Size = new Size(370, 61);
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.BackColor = exactBlue;
                btn.ForeColor = Color.White;
                btn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                btn.Cursor = Cursors.Hand;
                btn.Resize -= CustomButton_Resize;
                btn.Resize += CustomButton_Resize;
                CustomButton_Resize(btn, EventArgs.Empty);
            }
        }
        private void CustomButton_Resize(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                int radius = 12;
                System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(btn.Width - radius, 0, radius, radius, 270, 90);
                path.AddArc(btn.Width - radius, btn.Height - radius, radius, radius, 0, 90);
                path.AddArc(0, btn.Height - radius, radius, radius, 90, 90);
                path.CloseFigure();
                btn.Region = new Region(path);
            }
        }
        private void CheckFirstBootSetup()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.SaveFolder) ||
                string.IsNullOrEmpty(Properties.Settings.Default.UGCFolder))
            {
                MessageBox.Show("Welcome to TomoAIO! Let's set up your folders so you don't have to select them every time.\n\n(For each window, look at the top left of your Explorer window to see which Folder you are selecting)", "First Time Setup", MessageBoxButtons.OK, MessageBoxIcon.Information);

                string tempSaveFolder = "";
                string tempUgcFolder = "";
                while (true)
                {
                    using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                    {
                        folderDialog.Description = "Select your MAIN Save Folder (Must contain Mii.sav & Player.sav)";
                        folderDialog.UseDescriptionForTitle = true;

                        if (folderDialog.ShowDialog() == DialogResult.OK)
                        {
                            string miiPath = Path.Combine(folderDialog.SelectedPath, "Mii.sav");
                            string playerPath = Path.Combine(folderDialog.SelectedPath, "Player.sav");

                            if (File.Exists(miiPath) && File.Exists(playerPath))
                            {
                                tempSaveFolder = folderDialog.SelectedPath;
                                break;
                            }
                            else
                            {
                                DialogResult retry = MessageBox.Show("Validation Failed!\n\nThe selected folder does not contain both 'Mii.sav' and 'Player.sav'.\n\nWould you like to try again?", "Invalid Folder", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

                                if (retry == DialogResult.Cancel) return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                MessageBox.Show("Mii.sav and Player.sav were successfully found!\n\nNext, please select your 'Ugc' folder so we can set up the UGC creator tool", "Step 1 Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                while (true)
                {
                    using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                    {
                        folderDialog.Description = "Select your UGC Folder (Must contain at least one .zs file)";
                        folderDialog.UseDescriptionForTitle = true;

                        if (folderDialog.ShowDialog() == DialogResult.OK)
                        {
                            bool hasZsFile = Directory.EnumerateFiles(folderDialog.SelectedPath, "*.zs").Any();

                            if (hasZsFile)
                            {
                                tempUgcFolder = folderDialog.SelectedPath;
                                break;
                            }
                            else
                            {
                                DialogResult retry = MessageBox.Show("Validation Failed!\n\nNo '.zs' files were found in the selected folder.\n\nWould you like to try again?", "Invalid Folder", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

                                if (retry == DialogResult.Cancel) return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                Properties.Settings.Default.SaveFolder = tempSaveFolder;
                Properties.Settings.Default.UGCFolder = tempUgcFolder;
                Properties.Settings.Default.Save();

                MessageBox.Show("All set! Your folders have been validated and saved.", "Setup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void lblCurrentMoney_Click(object sender, EventArgs e)
        {

        }

        private void btnChangeFolders_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show("Do you want to select new save folders?", "Change Folders", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                Properties.Settings.Default.SaveFolder = "";
                Properties.Settings.Default.UGCFolder = "";
                Properties.Settings.Default.Save();
                CheckFirstBootSetup();
            }
        }

        private void logoislandmgt_Click(object sender, EventArgs e)
        {

        }
    }
} 