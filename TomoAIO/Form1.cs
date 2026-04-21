using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TomoAIO.Services;

namespace TomoAIO
{
    public partial class Form1 : Form
    {
        string currentMiiSavPath = "";
        string currentUgcPath = "";
        private const int LogoMargin = 12;
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
        private readonly UgcTextureService _ugcTextureService = new();
        private readonly MiiSaveService _miiSaveService = new();

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
            EnableSmoothRendering();
            ConfigureMenuButtons();
            ConfigureActionButtons();
            ConfigureInputStyles();
            EnsureDiscordButtonVisible();
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
            // In WinForms, transparent buttons must share the same parent as the background image.
            button1.Parent = pictureBox1;
            button2.Parent = pictureBox1;
            logo.Parent = pictureBox1;
            logopanel1.Parent = pictureBox2;
            logo.BackColor = Color.Transparent;
            logopanel1.BackColor = Color.Transparent;
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
            btnDiscord.Visible = true;
            btnDiscord.Enabled = true;
            btnDiscord.BringToFront();
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
            int startX = Math.Max(20, (clientWidth - totalWidth) / 2);
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

            int maxArea = Math.Max(220, panelWidth - (outerMargin * 2));
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

            // Top row: back and load buttons aligned to same baseline.
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

            panelSidebar.Width = UgcSidebarWidth;

            int contentLeft = panelSidebar.Width + 20;
            int contentRight = panelWidth - 20;
            int contentWidth = Math.Max(220, contentRight - contentLeft);
            int top = 16;
            int bottomAreaHeight = 70;
            int gap = 12;

            int availableHeight = Math.Max(200, panelHeight - top - bottomAreaHeight - (gap * 2));
            int squareSize = Math.Max(180, Math.Min(contentWidth, availableHeight));
            int squareX = contentLeft + ((contentWidth - squareSize) / 2);
            int squareY = top;

            picPreview.Location = new Point(squareX, squareY);
            picPreview.Size = new Size(squareSize, squareSize);

            int buttonY = picPreview.Bottom + gap;
            int buttonWidth = Math.Max(120, Math.Min(280, (squareSize - gap) / 2));
            int totalButtonsWidth = (buttonWidth * 2) + gap;
            int buttonsLeft = squareX + ((squareSize - totalButtonsWidth) / 2);

            btnUgcImport.Location = new Point(buttonsLeft, buttonY);
            btnUgcImport.Size = new Size(buttonWidth, 44);

            btnUgcExport.Location = new Point(btnUgcImport.Right + gap, buttonY);
            btnUgcExport.Size = new Size(buttonWidth, 44);

            lblImageInfo.AutoSize = false;
            lblImageInfo.TextAlign = ContentAlignment.MiddleCenter;
            lblImageInfo.Location = new Point(squareX, btnUgcImport.Bottom + 6);
            lblImageInfo.Size = new Size(squareSize, 20);

            btnUgcBack.Size = new Size(154, 38);
            btnUgcBack.Location = new Point(contentLeft, top);
            btnUgcBack.BringToFront();
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

        // --- UGC EDITOR ENGINE (MadMax Logic) ---

        private void LoadUgcList(string exactUgcPath)
        {
            currentUgcPath = exactUgcPath;
            lstUGC.Items.Clear();
            var files = _ugcTextureService.GetCanvasFiles(exactUgcPath);
            if (files.Count == 0)
            {
                MessageBox.Show("Please make sure you selected the right folder", "Folder Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (string file in files)
            {
                lstUGC.Items.Add(file);
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
                if (picPreview.Image != null) picPreview.Image.Dispose();
                var preview = _ugcTextureService.BuildPreview(fullPath, selectedFile);
                picPreview.Image = preview.image;
                lblImageInfo.Text = preview.infoText;
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
                string? saveDir = Path.GetDirectoryName(currentMiiSavPath);
                if (string.IsNullOrWhiteSpace(saveDir))
                {
                    MessageBox.Show("Save folder path is invalid.", "TomoAIO");
                    return;
                }

                byte[] miiBytes = File.ReadAllBytes(currentMiiSavPath);
                byte[] payload = _miiSaveService.BuildMiiPackage(miiBytes, slot, persHashes, saveDir);
                File.WriteAllBytes(sfd.FileName, payload);
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

            try
            {
                _miiSaveService.ImportMii(currentMiiSavPath, slot, filePath, persHashes);
                RefreshMiiList();
                MessageBox.Show("Mii Imported Successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "TomoAIO");
            }
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
            string? sD = Path.GetDirectoryName(currentMiiSavPath);
            if (string.IsNullOrWhiteSpace(sD) || !Directory.Exists(sD))
            {
                return;
            }
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
            listBox1.Items.Clear();
            foreach (string entry in _miiSaveService.BuildMiiEntries(currentMiiSavPath))
            {
                listBox1.Items.Add(entry);
            }
        }

        // --- NAVIGATION & DESIGNER HANDSHAKES ---

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            panel1.BringToFront();
            PinLogoTopRight();
            LayoutMiiEditorControls();
            EnsureDiscordButtonVisible();
        }
        private void button3_Click(object sender, EventArgs e) { ShowMainMenu(); }
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
                        LayoutUgcEditorControls();
                        EnsureDiscordButtonVisible();
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
                // Pass the file path from the text box directly into the importer!
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

        // Empty stubs for designer compatibility
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void Form1_Load(object sender, EventArgs e)
        {
            lblImageInfo.Text = "Waiting for selection...";
            label2.BringToFront();
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
                        byte[] compressedData = _ugcTextureService.BuildCompressedCanvasPayload(fullPath, ofd.FileName);
                        File.WriteAllBytes(fullPath, compressedData);
                        MessageBox.Show("Custom texture successfully injected!", "Success");
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
            ShowMainMenu();

            // (Notice we completely removed the references to panel1 here)
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
    }
}