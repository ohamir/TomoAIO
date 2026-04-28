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
            panelIsland = new Panel();
            btnUnlockClothes = new Button();
            btnUnlockFood = new Button();
            btnUnlockQBuilds = new Button();
            btnUnlockInteriors = new Button();
            lblCurrentMoney = new Label();
            btnMenuBack = new Button();
            button4 = new Button();
            btnSaveMoney = new Button();
            numMoney = new NumericUpDown();
            IslandFund = new Label();
            lblIslandTitle = new Label();
            txtMiiPath = new TextBox();
            label1 = new Label();
            btnGo = new Button();
            cmbMiiAction = new ComboBox();
            button3 = new Button();
            listBox1 = new ListBox();
            logopanel1 = new PictureBox();
            pictureBox2 = new PictureBox();
            btnIslandMgmt = new Button();
            btnDiscord = new Button();
            panelUGC = new Panel();
            pictureBox3 = new PictureBox();
            btnUgcBack = new Button();
            btnUgcExport = new Button();
            btnUgcImport = new Button();
            lblImageInfo = new Label();
            picPreview = new PictureBox();
            panelSidebar = new Panel();
            txtSearch = new TextBox();
            lstUGC = new ListBox();
            pictureBox1 = new PictureBox();
            logo = new PictureBox();
            panel1.SuspendLayout();
            panelIsland.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numMoney).BeginInit();
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
            button1.Location = new Point(88, 322);
            button1.Name = "button1";
            button1.Size = new Size(418, 361);
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
            button2.Location = new Point(594, 322);
            button2.Name = "button2";
            button2.Size = new Size(418, 361);
            button2.TabIndex = 1;
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnBrowseMii);
            panel1.Controls.Add(panelIsland);
            panel1.Controls.Add(txtMiiPath);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(btnGo);
            panel1.Controls.Add(cmbMiiAction);
            panel1.Controls.Add(button3);
            panel1.Controls.Add(listBox1);
            panel1.Controls.Add(logopanel1);
            panel1.Controls.Add(pictureBox2);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.MinimumSize = new Size(800, 476);
            panel1.Name = "panel1";
            panel1.Size = new Size(1614, 1030);
            panel1.TabIndex = 3;
            panel1.Visible = false;
            panel1.Paint += panel1_Paint;
            // 
            // btnBrowseMii
            // 
            btnBrowseMii.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseMii.Location = new Point(1114, 165);
            btnBrowseMii.Margin = new Padding(3, 4, 3, 4);
            btnBrowseMii.Name = "btnBrowseMii";
            btnBrowseMii.Size = new Size(86, 29);
            btnBrowseMii.TabIndex = 10;
            btnBrowseMii.Text = "Browse..";
            btnBrowseMii.UseVisualStyleBackColor = true;
            btnBrowseMii.Click += btnBrowseMii_Click;
            // 
            // panelIsland
            // 
            panelIsland.Controls.Add(btnUnlockClothes);
            panelIsland.Controls.Add(btnUnlockFood);
            panelIsland.Controls.Add(btnUnlockQBuilds);
            panelIsland.Controls.Add(btnUnlockInteriors);
            panelIsland.Controls.Add(lblCurrentMoney);
            panelIsland.Controls.Add(btnMenuBack);
            panelIsland.Controls.Add(button4);
            panelIsland.Controls.Add(btnSaveMoney);
            panelIsland.Controls.Add(numMoney);
            panelIsland.Controls.Add(IslandFund);
            panelIsland.Controls.Add(lblIslandTitle);
            panelIsland.Location = new Point(341, 240);
            panelIsland.Margin = new Padding(3, 4, 3, 4);
            panelIsland.Name = "panelIsland";
            panelIsland.Size = new Size(1624, 1069);
            panelIsland.TabIndex = 13;
            panelIsland.Visible = false;
            // 
            // btnUnlockClothes
            // 
            btnUnlockClothes.Location = new Point(400, 308);
            btnUnlockClothes.Margin = new Padding(3, 4, 3, 4);
            btnUnlockClothes.Name = "btnUnlockClothes";
            btnUnlockClothes.Size = new Size(173, 47);
            btnUnlockClothes.TabIndex = 12;
            btnUnlockClothes.Text = "Unlock all Clothing";
            btnUnlockClothes.UseVisualStyleBackColor = true;
            btnUnlockClothes.Click += btnUnlockClothes_Click;
            // 
            // btnUnlockFood
            // 
            btnUnlockFood.Location = new Point(155, 304);
            btnUnlockFood.Margin = new Padding(3, 4, 3, 4);
            btnUnlockFood.Name = "btnUnlockFood";
            btnUnlockFood.Size = new Size(217, 51);
            btnUnlockFood.TabIndex = 11;
            btnUnlockFood.Text = "Unlock all Food";
            btnUnlockFood.UseVisualStyleBackColor = true;
            btnUnlockFood.Click += btnUnlockFood_Click;
            // 
            // btnUnlockQBuilds
            // 
            btnUnlockQBuilds.Location = new Point(606, 307);
            btnUnlockQBuilds.Margin = new Padding(3, 4, 3, 4);
            btnUnlockQBuilds.Name = "btnUnlockQBuilds";
            btnUnlockQBuilds.Size = new Size(183, 52);
            btnUnlockQBuilds.TabIndex = 10;
            btnUnlockQBuilds.Text = "Unlock all Quik Builds";
            btnUnlockQBuilds.UseVisualStyleBackColor = true;
            btnUnlockQBuilds.Click += btnUnlockQBuilds_Click;
            // 
            // btnUnlockInteriors
            // 
            btnUnlockInteriors.Location = new Point(795, 309);
            btnUnlockInteriors.Margin = new Padding(3, 4, 3, 4);
            btnUnlockInteriors.Name = "btnUnlockInteriors";
            btnUnlockInteriors.Size = new Size(189, 46);
            btnUnlockInteriors.TabIndex = 9;
            btnUnlockInteriors.Text = "Unlock all Interiors";
            btnUnlockInteriors.UseVisualStyleBackColor = true;
            btnUnlockInteriors.Click += btnUnlockInteriors_Click;
            // 
            // lblCurrentMoney
            // 
            lblCurrentMoney.AutoSize = true;
            lblCurrentMoney.Location = new Point(1075, 142);
            lblCurrentMoney.Name = "lblCurrentMoney";
            lblCurrentMoney.Size = new Size(102, 19);
            lblCurrentMoney.TabIndex = 6;
            lblCurrentMoney.Text = "Current money";
            // 
            // btnMenuBack
            // 
            btnMenuBack.Location = new Point(125, 56);
            btnMenuBack.Margin = new Padding(3, 4, 3, 4);
            btnMenuBack.Name = "btnMenuBack";
            btnMenuBack.Size = new Size(201, 43);
            btnMenuBack.TabIndex = 5;
            btnMenuBack.Text = "Back to menu";
            btnMenuBack.UseVisualStyleBackColor = true;
            btnMenuBack.Click += btnMenuBack_Click1;
            // 
            // button4
            // 
            button4.Location = new Point(34, 21);
            button4.Name = "button4";
            button4.Size = new Size(147, 28);
            button4.TabIndex = 2;
            button4.Text = "Load Save Folder";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // btnSaveMoney
            // 
            btnSaveMoney.Location = new Point(1065, 309);
            btnSaveMoney.Margin = new Padding(3, 4, 3, 4);
            btnSaveMoney.Name = "btnSaveMoney";
            btnSaveMoney.Size = new Size(135, 46);
            btnSaveMoney.TabIndex = 3;
            btnSaveMoney.Text = "Save Money";
            btnSaveMoney.UseVisualStyleBackColor = true;
            btnSaveMoney.Click += btnSaveMoney_Click;
            // 
            // numMoney
            // 
            numMoney.Location = new Point(1065, 256);
            numMoney.Margin = new Padding(3, 4, 3, 4);
            numMoney.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            numMoney.Name = "numMoney";
            numMoney.Size = new Size(137, 26);
            numMoney.TabIndex = 2;
            numMoney.ValueChanged += numMoney_ValueChanged;
            // 
            // IslandFund
            // 
            IslandFund.AutoSize = true;
            IslandFund.Location = new Point(1086, 210);
            IslandFund.Name = "IslandFund";
            IslandFund.Size = new Size(91, 19);
            IslandFund.TabIndex = 1;
            IslandFund.Text = "Island money";
            // 
            // lblIslandTitle
            // 
            lblIslandTitle.AutoSize = true;
            lblIslandTitle.Location = new Point(755, 56);
            lblIslandTitle.Name = "lblIslandTitle";
            lblIslandTitle.Size = new Size(45, 19);
            lblIslandTitle.TabIndex = 0;
            lblIslandTitle.Text = "label3";
            lblIslandTitle.Click += label3_Click;
            // 
            // txtMiiPath
            // 
            txtMiiPath.AllowDrop = true;
            txtMiiPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtMiiPath.Location = new Point(341, 166);
            txtMiiPath.Margin = new Padding(3, 4, 3, 4);
            txtMiiPath.Name = "txtMiiPath";
            txtMiiPath.ReadOnly = true;
            txtMiiPath.Size = new Size(766, 26);
            txtMiiPath.TabIndex = 9;
            txtMiiPath.Text = "Choose a Mii file here...";
            txtMiiPath.TextChanged += txtMiiPath_TextChanged;
            txtMiiPath.DragEnter += txtMiiPath_DragEnter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(341, 142);
            label1.Name = "label1";
            label1.Size = new Size(127, 19);
            label1.TabIndex = 8;
            label1.Text = "Open / Save as Mii:";
            // 
            // btnGo
            // 
            btnGo.Location = new Point(651, 224);
            btnGo.Name = "btnGo";
            btnGo.Size = new Size(118, 42);
            btnGo.TabIndex = 5;
            btnGo.Text = "Go";
            btnGo.UseVisualStyleBackColor = true;
            btnGo.Click += button5_Click;
            // 
            // cmbMiiAction
            // 
            cmbMiiAction.FormattingEnabled = true;
            cmbMiiAction.Items.AddRange(new object[] { "Import Mii (.ltd)", "Export Mii (.ltd)" });
            cmbMiiAction.Location = new Point(178, 104);
            cmbMiiAction.Name = "cmbMiiAction";
            cmbMiiAction.Size = new Size(147, 27);
            cmbMiiAction.TabIndex = 4;
            cmbMiiAction.Text = "Select Action..";
            // 
            // button3
            // 
            button3.Location = new Point(10, 10);
            button3.Name = "button3";
            button3.Size = new Size(135, 28);
            button3.TabIndex = 1;
            button3.Text = "<- Back to menu";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // listBox1
            // 
            listBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listBox1.FormattingEnabled = true;
            listBox1.Location = new Point(174, 271);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(1267, 650);
            listBox1.TabIndex = 0;
            // 
            // logopanel1
            // 
            logopanel1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            logopanel1.BackColor = Color.Transparent;
            logopanel1.BackgroundImage = Properties.Resources.tomoaio_logo;
            logopanel1.BackgroundImageLayout = ImageLayout.Stretch;
            logopanel1.Location = new Point(1411, 4);
            logopanel1.Margin = new Padding(3, 4, 3, 4);
            logopanel1.Name = "logopanel1";
            logopanel1.Size = new Size(200, 203);
            logopanel1.TabIndex = 12;
            logopanel1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox2.Dock = DockStyle.Fill;
            pictureBox2.Image = Properties.Resources.tomo1;
            pictureBox2.Location = new Point(0, 0);
            pictureBox2.Margin = new Padding(3, 4, 3, 4);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(1614, 1030);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 7;
            pictureBox2.TabStop = false;
            // 
            // btnIslandMgmt
            // 
            btnIslandMgmt.Anchor = AnchorStyles.None;
            btnIslandMgmt.BackColor = Color.Transparent;
            btnIslandMgmt.BackgroundImage = Properties.Resources.islandmanager;
            btnIslandMgmt.BackgroundImageLayout = ImageLayout.Zoom;
            btnIslandMgmt.Cursor = Cursors.Hand;
            btnIslandMgmt.FlatAppearance.BorderSize = 0;
            btnIslandMgmt.FlatStyle = FlatStyle.Flat;
            btnIslandMgmt.Location = new Point(1086, 322);
            btnIslandMgmt.Name = "btnIslandMgmt";
            btnIslandMgmt.Size = new Size(418, 361);
            btnIslandMgmt.TabIndex = 15;
            btnIslandMgmt.UseVisualStyleBackColor = false;
            btnIslandMgmt.Click += btnIslandMgmt_Click;
            // 
            // btnDiscord
            // 
            btnDiscord.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnDiscord.BackColor = Color.Transparent;
            btnDiscord.Location = new Point(1526, 944);
            btnDiscord.Margin = new Padding(3, 4, 3, 4);
            btnDiscord.Name = "btnDiscord";
            btnDiscord.Size = new Size(86, 82);
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
            panelUGC.Margin = new Padding(3, 4, 3, 4);
            panelUGC.Name = "panelUGC";
            panelUGC.Size = new Size(1614, 1030);
            panelUGC.TabIndex = 13;
            panelUGC.Visible = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureBox3.BackColor = Color.Transparent;
            pictureBox3.BackgroundImage = Properties.Resources.tomoaio_logo;
            pictureBox3.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox3.Location = new Point(1410, 4);
            pictureBox3.Margin = new Padding(3, 4, 3, 4);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(200, 203);
            pictureBox3.TabIndex = 12;
            pictureBox3.TabStop = false;
            // 
            // btnUgcBack
            // 
            btnUgcBack.Location = new Point(293, 35);
            btnUgcBack.Margin = new Padding(3, 4, 3, 4);
            btnUgcBack.Name = "btnUgcBack";
            btnUgcBack.Size = new Size(69, 86);
            btnUgcBack.TabIndex = 5;
            btnUgcBack.Text = "Back to menu";
            btnUgcBack.UseVisualStyleBackColor = true;
            btnUgcBack.Click += btnUgcBack_Click;
            // 
            // btnUgcExport
            // 
            btnUgcExport.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnUgcExport.Location = new Point(1235, 940);
            btnUgcExport.Margin = new Padding(3, 4, 3, 4);
            btnUgcExport.Name = "btnUgcExport";
            btnUgcExport.Size = new Size(344, 82);
            btnUgcExport.TabIndex = 4;
            btnUgcExport.Text = "Export (.png / .zs)";
            btnUgcExport.UseVisualStyleBackColor = true;
            btnUgcExport.Click += btnUgcExport_Click;
            // 
            // btnUgcImport
            // 
            btnUgcImport.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnUgcImport.Location = new Point(368, 940);
            btnUgcImport.Margin = new Padding(3, 4, 3, 4);
            btnUgcImport.Name = "btnUgcImport";
            btnUgcImport.Size = new Size(344, 82);
            btnUgcImport.TabIndex = 3;
            btnUgcImport.Text = "Import (.png / .zs)";
            btnUgcImport.UseVisualStyleBackColor = true;
            btnUgcImport.Click += btnUgcImport_Click;
            // 
            // lblImageInfo
            // 
            lblImageInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblImageInfo.AutoSize = true;
            lblImageInfo.Location = new Point(739, 972);
            lblImageInfo.Name = "lblImageInfo";
            lblImageInfo.Size = new Size(45, 19);
            lblImageInfo.TabIndex = 2;
            lblImageInfo.Text = "label2";
            lblImageInfo.Click += lblImageInfo_Click;
            // 
            // picPreview
            // 
            picPreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            picPreview.BackColor = Color.White;
            picPreview.Location = new Point(368, 10);
            picPreview.Margin = new Padding(3, 4, 3, 4);
            picPreview.Name = "picPreview";
            picPreview.Size = new Size(1211, 922);
            picPreview.SizeMode = PictureBoxSizeMode.Zoom;
            picPreview.TabIndex = 1;
            picPreview.TabStop = false;
            picPreview.Click += picPreview_Click;
            // 
            // panelSidebar
            // 
            panelSidebar.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelSidebar.BackColor = Color.DarkKhaki;
            panelSidebar.Controls.Add(txtSearch);
            panelSidebar.Controls.Add(lstUGC);
            panelSidebar.Dock = DockStyle.Left;
            panelSidebar.Location = new Point(0, 0);
            panelSidebar.Margin = new Padding(3, 4, 3, 4);
            panelSidebar.Name = "panelSidebar";
            panelSidebar.Padding = new Padding(0, 35, 0, 0);
            panelSidebar.Size = new Size(286, 1030);
            panelSidebar.TabIndex = 0;
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(2, 5);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(282, 26);
            txtSearch.TabIndex = 2;
            txtSearch.Text = "Search...";
            txtSearch.TextChanged += txtSearch_TextChanged;
            txtSearch.Enter += txtSearch_Enter;
            txtSearch.Leave += txtSearch_Leave;
            // 
            // lstUGC
            // 
            lstUGC.BackColor = Color.DarkKhaki;
            lstUGC.Dock = DockStyle.Fill;
            lstUGC.ForeColor = Color.Black;
            lstUGC.FormattingEnabled = true;
            lstUGC.Location = new Point(0, 35);
            lstUGC.Margin = new Padding(3, 4, 3, 4);
            lstUGC.Name = "lstUGC";
            lstUGC.Size = new Size(286, 995);
            lstUGC.TabIndex = 0;
            lstUGC.SelectedIndexChanged += lstUGC_SelectedIndexChanged;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Image = Properties.Resources.tomo1;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Margin = new Padding(3, 4, 3, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1614, 1030);
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
            logo.Location = new Point(1411, 4);
            logo.Margin = new Padding(3, 4, 3, 4);
            logo.Name = "logo";
            logo.Size = new Size(200, 203);
            logo.TabIndex = 11;
            logo.TabStop = false;
            logo.Click += pictureBox3_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 190, 0);
            ClientSize = new Size(1614, 1030);
            Controls.Add(logo);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(btnIslandMgmt);
            Controls.Add(btnDiscord);
            Controls.Add(panel1);
            Controls.Add(pictureBox1);
            Controls.Add(panelUGC);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "TomoAIO - TLLTD Tool 2.0";
            Load += Form1_Load;
            Shown += Form1_Shown;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panelIsland.ResumeLayout(false);
            panelIsland.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numMoney).EndInit();
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
        private Label IslandFund;
        private Button btnUgcBack;
        private Button discord;
        private Button discsord2;
        private PictureBox pictureBox3;
        private Button btnDiscord;
        private TextBox txtSearch;
        private Button btnIslandMgmt;
        private Panel panelIsland;
        private Label lblIslandTitle;
        private NumericUpDown numMoney;
        private Button btnMenuBack;
        private Button btnSaveMoney;
        private Label lblCurrentMoney;
        private Button btnUnlockClothes;
        private Button btnUnlockFood;
        private Button btnUnlockQBuilds;
        private Button btnUnlockInteriors;
    }
}