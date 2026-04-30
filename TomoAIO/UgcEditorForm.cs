using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TomoAIO.Infrastructure;
using TomoAIO.Models;
using TomoAIO.Services;

namespace TomoAIO
{
    public partial class UgcEditorForm : Form
    {
        private readonly AppState _state;
        private readonly UgcService _ugcService;
        private readonly FileSystemGateway _fileSystem = new();

        private readonly List<string> _allUgcFiles = new();
        private readonly Dictionary<string, string> _ugcDisplayToFile = new(StringComparer.OrdinalIgnoreCase);

        // ─── Scaling ──────────────────────────────────────────────────────────
        private SizeF _originalFormSize;
        private readonly Dictionary<Control, RectangleF> _originalBounds = new();
        private readonly Dictionary<Control, Font> _originalFonts = new();

        // ─── Paint Editor ─────────────────────────────────────────────────────
        private Bitmap? _baseImage;
        private Bitmap? _paintOverlay;
        private bool _isPainting;
        private Color _brushColor = Color.Red;
        private int _brushSize = 10;
        private int _opacity = 255; // New: Transparency support
        private bool _eraserMode = false;
        private readonly Stack<Bitmap> _undoStack = new();

        // ─── Modes & Shapes ───────────────────────────────────────────────────
        private enum EditMode { Brush, Rect, Circle, Line, EyeDropper }
        private EditMode _currentMode = EditMode.Brush;
        private Point _startPos; // Track mouse down location for shapes

        public UgcEditorForm(AppState state)
        {
            _state = state;
            _ugcService = new UgcService(_fileSystem, new ZstdCodec());
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            EnableDoubleBuffer(lstUGC);
            EnableDoubleBuffer(picPreview);

            picPreview.SizeMode = PictureBoxSizeMode.Zoom;
            picPreview.Paint += PicPreview_Paint;
            picPreview.MouseDown += PicPreview_MouseDown;
            picPreview.MouseMove += PicPreview_MouseMove;
            picPreview.MouseUp += PicPreview_MouseUp;

            lstUGC.SelectedIndexChanged += lstUGC_SelectedIndexChanged;

            ApplyStyleToButtons();
            WireUpExtraEvents();

            this.Load += (s, e) =>
            {
                InitializeUgcList();
                CaptureOriginalBounds();
            };
            this.Resize += UgcEditorForm_Resize;
        }

        private void WireUpExtraEvents()
        {
            // Mode selection
            btnBrushMode.Click += (s, e) => { _currentMode = EditMode.Brush; _eraserMode = false; btnEraser.Text = "🖊 Eraser"; };
            btnEyeDropper.Click += (s, e) => _currentMode = EditMode.EyeDropper;
            btnRect.Click += (s, e) => _currentMode = EditMode.Rect;
            btnCircle.Click += (s, e) => _currentMode = EditMode.Circle;
            btnLine.Click += (s, e) => _currentMode = EditMode.Line;

            // Opacity slider
            trkOpacity.Scroll += (s, e) => {
                _opacity = trkOpacity.Value;
                trkOpacity.Text = $"Alpha: {_opacity}";
            };
        }

        // ─── Scaling Methods ──────────────────────────────────────────────────

        private void CaptureOriginalBounds()
        {
            if (this.ClientSize.Width == 0 || this.ClientSize.Height == 0) return;
            _originalFormSize = new SizeF(this.ClientSize.Width, this.ClientSize.Height);
            CaptureControlBounds(this.Controls);
        }

        private void CaptureControlBounds(Control.ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                _originalBounds[c] = new RectangleF(c.Left, c.Top, c.Width, c.Height);
                _originalFonts[c] = c.Font;
                if (c.Controls.Count > 0) CaptureControlBounds(c.Controls);
            }
        }

        private void UgcEditorForm_Resize(object? sender, EventArgs e)
        {
            if (_originalFormSize.IsEmpty || this.ClientSize.Width == 0) return;
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

                if (c.Controls.Count > 0) ScaleControls(c.Controls, scaleX, scaleY);
            }
        }

        private static void EnableDoubleBuffer(Control control)
        {
            typeof(Control)
                .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)?
                .SetValue(control, true, null);
        }

        private void ApplyStyleToButtons()
        {
            ApplyButtonStyle(btnUgcImport);
            ApplyButtonStyle(btnUgcExport);
            ApplyButtonStyle(btnPickColor, isSecondary: true);
            ApplyButtonStyle(btnEraser, isSecondary: true);
            ApplyButtonStyle(btnUndo, isSecondary: true);
            ApplyButtonStyle(btnClearPaint, isSecondary: true);

            // Apply style to new buttons
            ApplyButtonStyle(btnBrushMode, isSecondary: true);
            ApplyButtonStyle(btnEyeDropper, isSecondary: true);
            ApplyButtonStyle(btnRect, isSecondary: true);
            ApplyButtonStyle(btnCircle, isSecondary: true);
            ApplyButtonStyle(btnLine, isSecondary: true);
        }

        private void ApplyButtonStyle(Button btn, bool isSecondary = false)
        {
            Color mainColor = isSecondary ? Color.FromArgb(44, 62, 80) : Color.FromArgb(19, 141, 117);
            Color hoverColor = isSecondary ? Color.FromArgb(52, 73, 94) : Color.FromArgb(22, 160, 133);
            Color clickColor = isSecondary ? Color.FromArgb(31, 46, 61) : Color.FromArgb(14, 102, 85);

            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = mainColor;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI Semibold", 11f);
            btn.Cursor = Cursors.Hand;

            btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = mainColor;
            btn.MouseDown += (s, e) => { btn.BackColor = clickColor; btn.Padding = new Padding(0, 2, 0, 0); };
            btn.MouseUp += (s, e) => { btn.BackColor = hoverColor; btn.Padding = new Padding(0, 0, 0, 4); };

            void ApplyRegion()
            {
                if (btn.Width <= 0 || btn.Height <= 0) return;
                var path = new GraphicsPath();
                int r = 12;
                path.AddArc(0, 0, r, r, 180, 90);
                path.AddArc(btn.Width - r, 0, r, r, 270, 90);
                path.AddArc(btn.Width - r, btn.Height - r, r, r, 0, 90);
                path.AddArc(0, btn.Height - r, r, r, 90, 90);
                path.CloseFigure();
                btn.Region?.Dispose();
                btn.Region = new Region(path);
            }

            ApplyRegion();
            btn.Resize += (s, e) => ApplyRegion();
        }

        // ─── UGC List Logic ───────────────────────────────────────────────────

        private void InitializeUgcList()
        {
            if (!string.IsNullOrWhiteSpace(_state.CurrentUgcPath) && Directory.Exists(_state.CurrentUgcPath))
            {
                LoadUgcList(_state.CurrentUgcPath);
                return;
            }
            if (!string.IsNullOrWhiteSpace(_state.SaveFolderPath))
            {
                string ugcPath = Path.Combine(_state.SaveFolderPath, "Ugc");
                if (Directory.Exists(ugcPath)) { LoadUgcList(ugcPath); return; }
            }
        }

        private void LoadUgcList(string ugcPath)
        {
            _state.CurrentUgcPath = ugcPath;
            lstUGC.Items.Clear();
            _allUgcFiles.Clear();
            _ugcDisplayToFile.Clear();
            _state.UgcFiles.Clear();

            List<UgcFileItem> files = _ugcService.DiscoverUgcFiles(ugcPath);
            if (files.Count == 0) return;

            _state.UgcFiles.AddRange(files);
            foreach (UgcFileItem file in files)
            {
                _allUgcFiles.Add(file.FileName);
                AddUgcListEntry(file);
            }
        }

        private void AddUgcListEntry(UgcFileItem item)
        {
            string display = item.DisplayName;
            string unique = display;
            int suffix = 2;
            while (_ugcDisplayToFile.ContainsKey(unique)) unique = $"{display} ({suffix++})";
            _ugcDisplayToFile[unique] = item.FileName;
            lstUGC.Items.Add(unique);
        }

        private string? GetSelectedUgcFileName()
        {
            if (lstUGC.SelectedItem == null) return null;
            return _ugcDisplayToFile.TryGetValue(lstUGC.SelectedItem.ToString()!, out string? real) ? real : null;
        }

        // ─── Search ───────────────────────────────────────────────────────────

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            lstUGC.Items.Clear();
            _ugcDisplayToFile.Clear();
            var term = txtSearch.Text.ToLower();
            var source = (string.IsNullOrWhiteSpace(term) || term == "search...")
                ? _state.UgcFiles
                : _state.UgcFiles.Where(f => f.DisplayName.ToLower().Contains(term));

            foreach (var file in source) AddUgcListEntry(file);
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Search...") { txtSearch.Text = ""; txtSearch.ForeColor = Color.Black; }
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text)) { txtSearch.Text = "Search..."; txtSearch.ForeColor = Color.Gray; }
        }

        // ─── Preview & Painting ───────────────────────────────────────────────

        private void lstUGC_SelectedIndexChanged(object sender, EventArgs e)
        {
            string? selectedFile = GetSelectedUgcFileName();
            if (string.IsNullOrWhiteSpace(selectedFile)) return;
            string fullPath = Path.Combine(_state.CurrentUgcPath, selectedFile);

            try
            {
                using var img = TextureProcessor.DecodeFile(fullPath);
                using var ms = new MemoryStream();
                img.Save(ms, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
                ms.Position = 0;

                using var tmp = new Bitmap(ms);
                _baseImage?.Dispose();

                _baseImage = new Bitmap(tmp.Width, tmp.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                _baseImage.SetResolution(tmp.HorizontalResolution, tmp.VerticalResolution);

                using (var g = Graphics.FromImage(_baseImage))
                {
                    g.Clear(Color.Transparent);
                    g.DrawImageUnscaled(tmp, 0, 0);
                }

                _paintOverlay?.Dispose();
                _paintOverlay = new Bitmap(_baseImage.Width, _baseImage.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                _paintOverlay.SetResolution(tmp.HorizontalResolution, tmp.VerticalResolution);
                _undoStack.Clear();

                picPreview.Image = _baseImage;
                lblImageInfo.Text = $"{selectedFile} ({img.Width}x{img.Height})";
                picPreview.Invalidate();
            }
            catch (Exception ex) { MessageBox.Show("Error loading file: " + ex.Message); }
        }

        private void PicPreview_Paint(object? sender, PaintEventArgs e)
        {
            if (_paintOverlay == null || picPreview.Image == null) return;

            Rectangle zoomRect = GetZoomRect();
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.DrawImage(_paintOverlay, zoomRect);

            // Shape Preview Logic: Draw the dashed line while dragging
            if (_isPainting && _currentMode != EditMode.Brush && _currentMode != EditMode.EyeDropper)
            {
                Point currentPos = MapToImageCoords(picPreview.PointToClient(Cursor.Position));
                DrawShape(e.Graphics, _startPos, currentPos, isPreview: true);
            }
        }

        private void PicPreview_MouseDown(object? sender, MouseEventArgs e)
        {
            if (_paintOverlay == null) return;

            Point imgPoint = MapToImageCoords(e.Location);

            if (_currentMode == EditMode.EyeDropper)
            {
                PickColorAtPoint(imgPoint);
                return;
            }

            _undoStack.Push((Bitmap)_paintOverlay.Clone());
            _isPainting = true;
            _startPos = imgPoint;

            if (_currentMode == EditMode.Brush)
                DrawOnOverlay(e.Location);
        }

        private void PicPreview_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!_isPainting) return;

            if (_currentMode == EditMode.Brush)
                DrawOnOverlay(e.Location);
            else
                picPreview.Invalidate(); // Refresh preview for shapes
        }

        private void PicPreview_MouseUp(object? sender, MouseEventArgs e)
        {
            if (!_isPainting) return;

            if (_currentMode != EditMode.Brush && _currentMode != EditMode.EyeDropper)
            {
                CommitShape(MapToImageCoords(e.Location));
            }

            _isPainting = false;
            picPreview.Invalidate();
        }

        private void DrawOnOverlay(Point screenPoint)
        {
            if (_paintOverlay == null || _baseImage == null) return;

            Point imgPoint = MapToImageCoords(screenPoint);
            if (imgPoint.X < 0 || imgPoint.X >= _baseImage.Width || imgPoint.Y < 0 || imgPoint.Y >= _baseImage.Height) return;

            using var g = Graphics.FromImage(_paintOverlay);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Color finalColor = Color.FromArgb(_opacity, _brushColor);

            if (_eraserMode)
            {
                g.CompositingMode = CompositingMode.SourceCopy;
                using var brush = new SolidBrush(Color.Transparent);
                g.FillEllipse(brush, imgPoint.X - _brushSize / 2, imgPoint.Y - _brushSize / 2, _brushSize, _brushSize);
            }
            else
            {
                using var brush = new SolidBrush(finalColor);
                g.FillEllipse(brush, imgPoint.X - _brushSize / 2, imgPoint.Y - _brushSize / 2, _brushSize, _brushSize);
            }

            picPreview.Invalidate();
        }

        private void CommitShape(Point endPoint)
        {
            if (_paintOverlay == null) return;
            using var g = Graphics.FromImage(_paintOverlay);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            DrawShape(g, _startPos, endPoint, isPreview: false);
        }

        private void DrawShape(Graphics g, Point start, Point end, bool isPreview)
        {
            Color drawColor = Color.FromArgb(_opacity, _brushColor);

            // If we are drawing a preview on the PictureBox, scale the coords to screen space
            if (isPreview)
            {
                Rectangle zoom = GetZoomRect();
                float scaleX = (float)zoom.Width / _baseImage!.Width;
                float scaleY = (float)zoom.Height / _baseImage!.Height;
                start = new Point((int)(start.X * scaleX + zoom.X), (int)(start.Y * scaleY + zoom.Y));
                end = new Point((int)(end.X * scaleX + zoom.X), (int)(end.Y * scaleY + zoom.Y));
            }

            using var pen = new Pen(drawColor, isPreview ? 1 : _brushSize);
            if (isPreview) pen.DashStyle = DashStyle.Dash;

            Rectangle rect = new Rectangle(
                Math.Min(start.X, end.X), Math.Min(start.Y, end.Y),
                Math.Abs(start.X - end.X), Math.Abs(start.Y - end.Y));

            switch (_currentMode)
            {
                case EditMode.Rect: g.DrawRectangle(pen, rect); break;
                case EditMode.Circle: g.DrawEllipse(pen, rect); break;
                case EditMode.Line: g.DrawLine(pen, start, end); break;
            }
        }

        private void PickColorAtPoint(Point p)
        {
            if (_baseImage == null) return;
            if (p.X < 0 || p.X >= _baseImage.Width || p.Y < 0 || p.Y >= _baseImage.Height) return;

            _brushColor = _baseImage.GetPixel(p.X, p.Y);
            btnPickColor.BackColor = _brushColor;
            _currentMode = EditMode.Brush; // Auto-switch back to brush
            picPreview.Invalidate();
        }

        private Point MapToImageCoords(Point p)
        {
            if (picPreview.Image == null) return p;

            Rectangle rect = GetZoomRect();

            float ratioX = (float)picPreview.Image.Width / rect.Width;
            float ratioY = (float)picPreview.Image.Height / rect.Height;

            int imgX = (int)((p.X - rect.X) * ratioX);
            int imgY = (int)((p.Y - rect.Y) * ratioY);

            return new Point(imgX, imgY);
        }

        private Rectangle GetZoomRect()
        {
            if (picPreview.Image == null) return picPreview.ClientRectangle;

            float imgAspect = (float)picPreview.Image.Width / picPreview.Image.Height;
            float pboxAspect = (float)picPreview.Width / picPreview.Height;

            int w, h, x, y;

            if (imgAspect > pboxAspect)
            {
                w = picPreview.Width;
                h = (int)(w / imgAspect);
                x = 0;
                y = (picPreview.Height - h) / 2;
            }
            else
            {
                h = picPreview.Height;
                w = (int)(h * imgAspect);
                y = 0;
                x = (picPreview.Width - w) / 2;
            }

            return new Rectangle(x, y, w, h);
        }

        // ─── Toolbar Logic ───────────────────────────────────────────────────

        private void btnPickColor_Click(object sender, EventArgs e)
        {
            using var cd = new ColorDialog { Color = _brushColor, FullOpen = true };
            if (cd.ShowDialog() == DialogResult.OK)
            {
                _brushColor = cd.Color;
                _eraserMode = false;
                btnEraser.Text = "🖊 Eraser";
                btnPickColor.BackColor = _brushColor;
            }
        }

        private void btnEraser_Click(object sender, EventArgs e)
        {
            _eraserMode = !_eraserMode;
            btnEraser.Text = _eraserMode ? "✏ Draw" : "🖊 Eraser";
            if (_eraserMode) _currentMode = EditMode.Brush;
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (_undoStack.Count > 0)
            {
                _paintOverlay?.Dispose();
                _paintOverlay = _undoStack.Pop();
                picPreview.Invalidate();
            }
        }

        private void btnClearPaint_Click(object sender, EventArgs e)
        {
            if (_paintOverlay == null) return;
            _undoStack.Push((Bitmap)_paintOverlay.Clone());
            using (var g = Graphics.FromImage(_paintOverlay)) g.Clear(Color.Transparent);
            picPreview.Invalidate();
        }

        private void trkBrushSize_Scroll(object sender, EventArgs e)
        {
            _brushSize = trkBrushSize.Value;
            lblBrushSize.Text = $"Size: {_brushSize}";
        }

        private Bitmap GetFlattenedImage()
        {
            if (_baseImage == null) throw new InvalidOperationException("No image loaded.");

            var flat = new Bitmap(_baseImage.Width, _baseImage.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            flat.SetResolution(_baseImage.HorizontalResolution, _baseImage.VerticalResolution);

            using (var g = Graphics.FromImage(flat))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImageUnscaled(_baseImage, 0, 0);
                if (_paintOverlay != null)
                    g.DrawImageUnscaled(_paintOverlay, 0, 0);
            }
            return flat;
        }

        // ─── Export & Import ──────────────────────────────────────────────────

        private void btnUgcImport_Click(object sender, EventArgs e)
        {
            string? selectedFile = GetSelectedUgcFileName();
            if (string.IsNullOrWhiteSpace(selectedFile)) return;
            using var ofd = new OpenFileDialog { Filter = "UGC Files (*.png;*.jpg;*.zs)|*.png;*.jpg;*.zs" };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            try
            {
                string fullPath = Path.Combine(_state.CurrentUgcPath, selectedFile);
                if (ofd.FileName.EndsWith(".zs", StringComparison.OrdinalIgnoreCase))
                {
                    _ugcService.ReplaceFromZs(ofd.FileName, fullPath);
                }
                else
                {
                    string stem = fullPath.Replace(".canvas.zs", "").Replace(".ugctex.zs", "");
                    TextureProcessor.ImportPng(ofd.FileName, stem, !stem.EndsWith("_Thumb"), !stem.EndsWith("_Thumb"), false, File.Exists(stem + ".ugctex.zs") ? stem + ".ugctex.zs" : null);
                }
                lstUGC_SelectedIndexChanged(sender, e);
                MessageBox.Show("Import Successful!");
            }
            catch (Exception ex) { MessageBox.Show("Import Error: " + ex.Message); }
        }

        private void btnUgcExport_Click(object sender, EventArgs e)
        {
            string? selectedFile = GetSelectedUgcFileName();
            if (string.IsNullOrWhiteSpace(selectedFile)) return;

            using var sfd = new SaveFileDialog { Filter = "PNG (*.png)|*.png|ZS (*.zs)|*.zs", FileName = selectedFile.Split('.')[0] + "_export" };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                using Bitmap flat = GetFlattenedImage();
                if (sfd.FileName.EndsWith(".png"))
                {
                    flat.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
                }
                else
                {
                    string tmp = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".png");
                    flat.Save(tmp, System.Drawing.Imaging.ImageFormat.Png);
                    string stem = sfd.FileName.Replace(".zs", "");
                    TextureProcessor.ImportPng(tmp, stem, true, true, false, null);
                    if (File.Exists(tmp)) File.Delete(tmp);
                }
                MessageBox.Show("Export Successful!");
            }
            catch (Exception ex) { MessageBox.Show("Export Error: " + ex.Message); }
        }
    }
}