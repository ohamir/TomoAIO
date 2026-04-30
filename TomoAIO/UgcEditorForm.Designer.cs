namespace TomoAIO
{
    partial class UgcEditorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lstUGC = new ListBox();
            picPreview = new PictureBox();
            btnUgcImport = new Button();
            btnUgcExport = new Button();
            txtSearch = new TextBox();
            lblImageInfo = new Label();
            btnPickColor = new Button();
            btnEraser = new Button();
            btnUndo = new Button();
            btnClearPaint = new Button();
            lblBrushSize = new Label();
            trkBrushSize = new TrackBar();
            btnEyeDropper = new Button();
            btnRect = new Button();
            btnCircle = new Button();
            btnLine = new Button();
            btnBrushMode = new Button();
            trkOpacity = new TrackBar();
            labelOpac = new Label();
            ((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trkBrushSize).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trkOpacity).BeginInit();
            SuspendLayout();
            // 
            // lstUGC
            // 
            lstUGC.FormattingEnabled = true;
            lstUGC.Location = new Point(12, 56);
            lstUGC.Name = "lstUGC";
            lstUGC.Size = new Size(248, 669);
            lstUGC.TabIndex = 0;
            // 
            // picPreview
            // 
            picPreview.BackgroundImageLayout = ImageLayout.Zoom;
            picPreview.Location = new Point(312, 75);
            picPreview.Name = "picPreview";
            picPreview.Size = new Size(591, 580);
            picPreview.TabIndex = 1;
            picPreview.TabStop = false;
            // 
            // btnUgcImport
            // 
            btnUgcImport.Location = new Point(312, 680);
            btnUgcImport.Name = "btnUgcImport";
            btnUgcImport.Size = new Size(180, 40);
            btnUgcImport.TabIndex = 2;
            btnUgcImport.Text = "Import (.png/ .zs)";
            btnUgcImport.UseVisualStyleBackColor = true;
            btnUgcImport.Click += btnUgcImport_Click;
            // 
            // btnUgcExport
            // 
            btnUgcExport.Location = new Point(723, 680);
            btnUgcExport.Name = "btnUgcExport";
            btnUgcExport.Size = new Size(180, 40);
            btnUgcExport.TabIndex = 3;
            btnUgcExport.Text = "Export (.png/ .zs)";
            btnUgcExport.UseVisualStyleBackColor = true;
            btnUgcExport.Click += btnUgcExport_Click;
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(12, 12);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(119, 26);
            txtSearch.TabIndex = 4;
            // 
            // lblImageInfo
            // 
            lblImageInfo.AutoSize = true;
            lblImageInfo.Location = new Point(315, 658);
            lblImageInfo.Name = "lblImageInfo";
            lblImageInfo.Size = new Size(97, 19);
            lblImageInfo.TabIndex = 5;
            lblImageInfo.Text = "Current image";
            // 
            // btnPickColor
            // 
            btnPickColor.Location = new Point(312, 12);
            btnPickColor.Name = "btnPickColor";
            btnPickColor.Size = new Size(100, 40);
            btnPickColor.TabIndex = 6;
            btnPickColor.Text = "pick color";
            btnPickColor.UseVisualStyleBackColor = true;
            btnPickColor.Click += btnPickColor_Click;
            // 
            // btnEraser
            // 
            btnEraser.Location = new Point(430, 12);
            btnEraser.Name = "btnEraser";
            btnEraser.Size = new Size(100, 40);
            btnEraser.TabIndex = 7;
            btnEraser.Text = "Erasor";
            btnEraser.UseVisualStyleBackColor = true;
            btnEraser.Click += btnEraser_Click;
            // 
            // btnUndo
            // 
            btnUndo.Location = new Point(547, 12);
            btnUndo.Name = "btnUndo";
            btnUndo.Size = new Size(100, 40);
            btnUndo.TabIndex = 8;
            btnUndo.Text = "Undo";
            btnUndo.UseVisualStyleBackColor = true;
            btnUndo.Click += btnUndo_Click;
            // 
            // btnClearPaint
            // 
            btnClearPaint.Location = new Point(665, 12);
            btnClearPaint.Name = "btnClearPaint";
            btnClearPaint.Size = new Size(100, 40);
            btnClearPaint.TabIndex = 9;
            btnClearPaint.Text = "Clear paint";
            btnClearPaint.UseVisualStyleBackColor = true;
            btnClearPaint.Click += btnClearPaint_Click;
            // 
            // lblBrushSize
            // 
            lblBrushSize.AutoSize = true;
            lblBrushSize.Location = new Point(927, 92);
            lblBrushSize.Name = "lblBrushSize";
            lblBrushSize.Size = new Size(70, 19);
            lblBrushSize.TabIndex = 10;
            lblBrushSize.Text = "Brush size";
            // 
            // trkBrushSize
            // 
            trkBrushSize.LargeChange = 2;
            trkBrushSize.Location = new Point(927, 114);
            trkBrushSize.Maximum = 50;
            trkBrushSize.Minimum = 1;
            trkBrushSize.Name = "trkBrushSize";
            trkBrushSize.Size = new Size(154, 53);
            trkBrushSize.TabIndex = 11;
            trkBrushSize.Value = 10;
            trkBrushSize.Scroll += trkBrushSize_Scroll;
            // 
            // btnEyeDropper
            // 
            btnEyeDropper.Location = new Point(927, 182);
            btnEyeDropper.Name = "btnEyeDropper";
            btnEyeDropper.Size = new Size(154, 40);
            btnEyeDropper.TabIndex = 12;
            btnEyeDropper.Text = "Eye Dropper";
            btnEyeDropper.UseVisualStyleBackColor = true;
            // 
            // btnRect
            // 
            btnRect.Location = new Point(927, 228);
            btnRect.Name = "btnRect";
            btnRect.Size = new Size(154, 40);
            btnRect.TabIndex = 13;
            btnRect.Text = "Rectangle";
            btnRect.UseVisualStyleBackColor = true;
            // 
            // btnCircle
            // 
            btnCircle.Location = new Point(927, 274);
            btnCircle.Name = "btnCircle";
            btnCircle.Size = new Size(154, 40);
            btnCircle.TabIndex = 14;
            btnCircle.Text = "Circle";
            btnCircle.UseVisualStyleBackColor = true;
            // 
            // btnLine
            // 
            btnLine.Location = new Point(927, 320);
            btnLine.Name = "btnLine";
            btnLine.Size = new Size(154, 40);
            btnLine.TabIndex = 15;
            btnLine.Text = "Line";
            btnLine.UseVisualStyleBackColor = true;
            // 
            // btnBrushMode
            // 
            btnBrushMode.Location = new Point(927, 366);
            btnBrushMode.Name = "btnBrushMode";
            btnBrushMode.Size = new Size(154, 40);
            btnBrushMode.TabIndex = 16;
            btnBrushMode.Text = "Brush Mode";
            btnBrushMode.UseVisualStyleBackColor = true;
            // 
            // trkOpacity
            // 
            trkOpacity.Location = new Point(927, 482);
            trkOpacity.Maximum = 255;
            trkOpacity.Name = "trkOpacity";
            trkOpacity.Size = new Size(154, 53);
            trkOpacity.TabIndex = 17;
            // 
            // labelOpac
            // 
            labelOpac.AutoSize = true;
            labelOpac.Location = new Point(927, 460);
            labelOpac.Name = "labelOpac";
            labelOpac.Size = new Size(56, 19);
            labelOpac.TabIndex = 18;
            labelOpac.Text = "Opacity";
            // 
            // UgcEditorForm
            // 
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.tomo1;
            ClientSize = new Size(1093, 732);
            Controls.Add(labelOpac);
            Controls.Add(trkOpacity);
            Controls.Add(btnBrushMode);
            Controls.Add(btnLine);
            Controls.Add(btnCircle);
            Controls.Add(btnRect);
            Controls.Add(btnEyeDropper);
            Controls.Add(trkBrushSize);
            Controls.Add(lblBrushSize);
            Controls.Add(btnClearPaint);
            Controls.Add(btnUndo);
            Controls.Add(btnEraser);
            Controls.Add(btnPickColor);
            Controls.Add(lblImageInfo);
            Controls.Add(txtSearch);
            Controls.Add(btnUgcExport);
            Controls.Add(btnUgcImport);
            Controls.Add(picPreview);
            Controls.Add(lstUGC);
            MinimumSize = new Size(1111, 777);
            Name = "UgcEditorForm";
            Text = "UgcEditorForm";
            ((System.ComponentModel.ISupportInitialize)picPreview).EndInit();
            ((System.ComponentModel.ISupportInitialize)trkBrushSize).EndInit();
            ((System.ComponentModel.ISupportInitialize)trkOpacity).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox lstUGC;
        private PictureBox picPreview;
        private Button btnUgcImport;
        private Button btnUgcExport;
        private TextBox txtSearch;
        private Label lblImageInfo;
        private Button btnPickColor;
        private Button btnEraser;
        private Button btnUndo;
        private Button btnClearPaint;
        private Label lblBrushSize;
        private TrackBar trkBrushSize;
        private Button btnEyeDropper;
        private Button btnRect;
        private Button btnCircle;
        private Button btnLine;
        private Button btnBrushMode;
        private TrackBar trkOpacity;
        private Label labelOpac;
    }
}