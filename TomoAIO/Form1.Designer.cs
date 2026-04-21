namespace TomoAIO
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            button1 = new Button();
            button2 = new Button();
            panel1 = new Panel();
            btnBrowseMii = new Button();
            txtMiiPath = new TextBox();
            label1 = new Label();
            btnGo = new Button();
            cmbMiiAction = new ComboBox();
            button4 = new Button();
            button3 = new Button();
            listBox1 = new ListBox();
            logopanel1 = new PictureBox();
            pictureBox2 = new PictureBox();
            btnDiscord = new Button();
            panelUGC = new Panel();
            pictureBox3 = new PictureBox();
            btnUgcBack = new Button();
            btnUgcExport = new Button();
            btnUgcImport = new Button();
            lblImageInfo = new Label();
            picPreview = new PictureBox();
            panelSidebar = new Panel();
            label2 = new Label();
            lstUGC = new ListBox();
            pictureBox1 = new PictureBox();
            logo = new PictureBox();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)logopanel1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            panelUGC.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
            panelSidebar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)logo).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.None;
            button1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button1.BackColor = Color.Transparent;
            button1.BackgroundImage = Properties.Resources.mii_import1;
            button1.BackgroundImageLayout = ImageLayout.Zoom;
            button1.Cursor = Cursors.Hand;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Location = new Point(312, 254);
            button1.Margin = new Padding(3, 2, 3, 2);
            button1.Name = "button1";
            button1.Size = new Size(366, 285);
            button1.TabIndex = 0;
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.None;
            button2.BackColor = Color.Transparent;
            button2.BackgroundImage = Properties.Resources.UGC_CREATOR;
            button2.BackgroundImageLayout = ImageLayout.Zoom;
            button2.Cursor = Cursors.Hand;
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Location = new Point(723, 254);
            button2.Margin = new Padding(3, 2, 3, 2);
            button2.Name = "button2";
            button2.Size = new Size(366, 285);
            button2.TabIndex = 1;
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnBrowseMii);
            panel1.Controls.Add(txtMiiPath);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(btnGo);
            panel1.Controls.Add(cmbMiiAction);
            panel1.Controls.Add(button4);
            panel1.Controls.Add(button3);
            panel1.Controls.Add(listBox1);
            panel1.Controls.Add(logopanel1);
            panel1.Controls.Add(pictureBox2);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(3, 2, 3, 2);
            panel1.MinimumSize = new Size(700, 376);
            panel1.Name = "panel1";
            panel1.Size = new Size(1412, 813);
            panel1.TabIndex = 3;
            panel1.Visible = false;
            panel1.Paint += panel1_Paint;
            // 
            // btnBrowseMii
            // 
            btnBrowseMii.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseMii.Location = new Point(975, 130);
            btnBrowseMii.Name = "btnBrowseMii";
            btnBrowseMii.Size = new Size(75, 23);
            btnBrowseMii.TabIndex = 10;
            btnBrowseMii.Text = "Browse..";
            btnBrowseMii.UseVisualStyleBackColor = true;
            btnBrowseMii.Click += btnBrowseMii_Click;
            // 
            // txtMiiPath
            // 
            txtMiiPath.AllowDrop = true;
            txtMiiPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtMiiPath.Location = new Point(298, 131);
            txtMiiPath.Name = "txtMiiPath";
            txtMiiPath.ReadOnly = true;
            txtMiiPath.Size = new Size(671, 23);
            txtMiiPath.TabIndex = 9;
            txtMiiPath.Text = "Choose a Mii file here...";
            txtMiiPath.TextChanged += txtMiiPath_TextChanged;
            txtMiiPath.DragEnter += txtMiiPath_DragEnter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(298, 112);
            label1.Name = "label1";
            label1.Size = new Size(108, 15);
            label1.TabIndex = 8;
            label1.Text = "Open / Save as Mii:";
            // 
            // btnGo
            // 
            btnGo.Location = new Point(570, 177);
            btnGo.Margin = new Padding(3, 2, 3, 2);
            btnGo.Name = "btnGo";
            btnGo.Size = new Size(103, 33);
            btnGo.TabIndex = 5;
            btnGo.Text = "Go";
            btnGo.UseVisualStyleBackColor = true;
            btnGo.Click += button5_Click;
            // 
            // cmbMiiAction
            // 
            cmbMiiAction.FormattingEnabled = true;
            cmbMiiAction.Items.AddRange(new object[] { "Import Mii (.ltd)", "Export Mii (.ltd)" });
            cmbMiiAction.Location = new Point(156, 82);
            cmbMiiAction.Margin = new Padding(3, 2, 3, 2);
            cmbMiiAction.Name = "cmbMiiAction";
            cmbMiiAction.Size = new Size(129, 23);
            cmbMiiAction.TabIndex = 4;
            cmbMiiAction.Text = "Select Action..";
            // 
            // button4
            // 
            button4.Location = new Point(156, 56);
            button4.Margin = new Padding(3, 2, 3, 2);
            button4.Name = "button4";
            button4.Size = new Size(129, 22);
            button4.TabIndex = 2;
            button4.Text = "Load Save Folder";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button3
            // 
            button3.Location = new Point(9, 8);
            button3.Margin = new Padding(3, 2, 3, 2);
            button3.Name = "button3";
            button3.Size = new Size(118, 22);
            button3.TabIndex = 1;
            button3.Text = "<- Back to menu";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // listBox1
            // 
            listBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listBox1.FormattingEnabled = true;
            listBox1.Location = new Point(152, 214);
            listBox1.Margin = new Padding(3, 2, 3, 2);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(1109, 514);
            listBox1.TabIndex = 0;
            // 
            // logopanel1
            // 
            logopanel1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            logopanel1.BackColor = Color.Transparent;
            logopanel1.BackgroundImage = Properties.Resources.tomoaio_logo;
            logopanel1.BackgroundImageLayout = ImageLayout.Stretch;
            logopanel1.Location = new Point(1235, 3);
            logopanel1.Name = "logopanel1";
            logopanel1.Size = new Size(175, 160);
            logopanel1.TabIndex = 12;
            logopanel1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox2.Dock = DockStyle.Fill;
            pictureBox2.Image = Properties.Resources.tomo1;
            pictureBox2.Location = new Point(0, 0);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(1412, 813);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 7;
            pictureBox2.TabStop = false;
            // 
            // btnDiscord
            // 
            btnDiscord.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnDiscord.BackColor = Color.Transparent;
            btnDiscord.Location = new Point(1335, 745);
            btnDiscord.Name = "btnDiscord";
            btnDiscord.Size = new Size(75, 65);
            btnDiscord.TabIndex = 14;
            btnDiscord.UseVisualStyleBackColor = true;
            btnDiscord.Click += btnDiscord_Click;
            // 
            // panelUGC
            // 
            panelUGC.BackgroundImage = Properties.Resources.tomo1;
            panelUGC.Controls.Add(pictureBox3);
            panelUGC.Controls.Add(btnUgcBack);
            panelUGC.Controls.Add(btnUgcExport);
            panelUGC.Controls.Add(btnUgcImport);
            panelUGC.Controls.Add(lblImageInfo);
            panelUGC.Controls.Add(picPreview);
            panelUGC.Controls.Add(panelSidebar);
            panelUGC.Dock = DockStyle.Fill;
            panelUGC.Location = new Point(0, 0);
            panelUGC.Name = "panelUGC";
            panelUGC.Size = new Size(1412, 813);
            panelUGC.TabIndex = 13;
            panelUGC.Visible = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureBox3.BackColor = Color.Transparent;
            pictureBox3.BackgroundImage = Properties.Resources.tomoaio_logo;
            pictureBox3.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox3.Location = new Point(1234, 3);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(175, 160);
            pictureBox3.TabIndex = 12;
            pictureBox3.TabStop = false;
            // 
            // btnUgcBack
            // 
            btnUgcBack.Location = new Point(256, 28);
            btnUgcBack.Name = "btnUgcBack";
            btnUgcBack.Size = new Size(60, 68);
            btnUgcBack.TabIndex = 5;
            btnUgcBack.Text = "Back to menu";
            btnUgcBack.UseVisualStyleBackColor = true;
            btnUgcBack.Click += btnUgcBack_Click;
            // 
            // btnUgcExport
            // 
            btnUgcExport.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnUgcExport.Location = new Point(1081, 742);
            btnUgcExport.Name = "btnUgcExport";
            btnUgcExport.Size = new Size(301, 65);
            btnUgcExport.TabIndex = 4;
            btnUgcExport.Text = "Export";
            btnUgcExport.UseVisualStyleBackColor = true;
            btnUgcExport.Click += btnUgcExport_Click;
            // 
            // btnUgcImport
            // 
            btnUgcImport.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnUgcImport.Location = new Point(322, 742);
            btnUgcImport.Name = "btnUgcImport";
            btnUgcImport.Size = new Size(301, 65);
            btnUgcImport.TabIndex = 3;
            btnUgcImport.Text = "Import";
            btnUgcImport.UseVisualStyleBackColor = true;
            btnUgcImport.Click += btnUgcImport_Click;
            // 
            // lblImageInfo
            // 
            lblImageInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblImageInfo.AutoSize = true;
            lblImageInfo.Location = new Point(647, 767);
            lblImageInfo.Name = "lblImageInfo";
            lblImageInfo.Size = new Size(38, 15);
            lblImageInfo.TabIndex = 2;
            lblImageInfo.Text = "label2";
            lblImageInfo.Click += lblImageInfo_Click;
            // 
            // picPreview
            // 
            picPreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            picPreview.BackColor = Color.White;
            picPreview.Location = new Point(322, 8);
            picPreview.Name = "picPreview";
            picPreview.Size = new Size(1060, 728);
            picPreview.SizeMode = PictureBoxSizeMode.Zoom;
            picPreview.TabIndex = 1;
            picPreview.TabStop = false;
            // 
            // panelSidebar
            // 
            panelSidebar.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelSidebar.BackColor = Color.DarkKhaki;
            panelSidebar.Controls.Add(label2);
            panelSidebar.Controls.Add(lstUGC);
            panelSidebar.Dock = DockStyle.Left;
            panelSidebar.Location = new Point(0, 0);
            panelSidebar.Name = "panelSidebar";
            panelSidebar.Padding = new Padding(0, 28, 0, 0);
            panelSidebar.Size = new Size(250, 813);
            panelSidebar.TabIndex = 0;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.DarkKhaki;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(12, 7);
            label2.Name = "label2";
            label2.Size = new Size(68, 15);
            label2.TabIndex = 1;
            label2.Text = "UGC Editor";
            label2.Click += label2_Click;
            // 
            // lstUGC
            // 
            lstUGC.BackColor = Color.DarkKhaki;
            lstUGC.Dock = DockStyle.Fill;
            lstUGC.ForeColor = Color.Black;
            lstUGC.FormattingEnabled = true;
            lstUGC.Location = new Point(0, 28);
            lstUGC.Name = "lstUGC";
            lstUGC.Size = new Size(250, 785);
            lstUGC.TabIndex = 0;
            lstUGC.SelectedIndexChanged += lstUGC_SelectedIndexChanged;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Image = Properties.Resources.tomo1;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1412, 813);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // logo
            // 
            logo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            logo.BackColor = Color.Transparent;
            logo.BackgroundImage = Properties.Resources.tomoaio_logo;
            logo.BackgroundImageLayout = ImageLayout.Stretch;
            logo.Location = new Point(1235, 3);
            logo.Name = "logo";
            logo.Size = new Size(175, 160);
            logo.TabIndex = 11;
            logo.TabStop = false;
            logo.Click += pictureBox3_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 190, 0);
            ClientSize = new Size(1412, 813);
            Controls.Add(logo);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(btnDiscord);
            Controls.Add(panel1);
            Controls.Add(pictureBox1);
            Controls.Add(panelUGC);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 2, 3, 2);
            Name = "Form1";
            Text = "TomoAIO - TLLTD Tool 1.0";
            Load += Form1_Load;
            Shown += Form1_Shown;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)logopanel1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            panelUGC.ResumeLayout(false);
            panelUGC.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)picPreview).EndInit();
            panelSidebar.ResumeLayout(false);
            panelSidebar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)logo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private Button button2;
        private Panel panel1;
        private Button button3;
        private ListBox listBox1;
        private Button button4;
        private ComboBox cmbMiiAction;
        private Button btnGo;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private TextBox txtMiiPath;
        private Label label1;
        private Button btnBrowseMii;
        private PictureBox logo;
        private PictureBox logopanel1;
        private Panel panelUGC;
        private PictureBox picPreview;
        private Panel panelSidebar;
        private ListBox lstUGC;
        private Label lblImageInfo;
        private Button btnUgcExport;
        private Button btnUgcImport;
        private Label label2;
        private Button btnUgcBack;
        private Button discord;
        private Button discsord2;
        private PictureBox pictureBox3;
        private Button btnDiscord;
    }
}