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
            panelMii = new Panel();
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
            panelIslandMGT = new Panel();
            tablelayoutpanelisland = new TableLayoutPanel();
            btnUnlockInteriors = new Button();
            btnUnlockQBuilds = new Button();
            btnUnlockClothes = new Button();
            btnUnlockFood = new Button();
            lblCurrentMoney = new Label();
            btnMenuBack = new Button();
            btnSaveMoney = new Button();
            numMoney = new NumericUpDown();
            IslandFund = new Label();
            lblIslandTitle = new Label();
            logoislandmgt = new PictureBox();
            pictureBox4 = new PictureBox();
            pictureBox1 = new PictureBox();
            logo = new PictureBox();
            btnChangeFolders = new Button();
            panelMii.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)logopanel1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            panelUGC.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
            panelSidebar.SuspendLayout();
            panelIslandMGT.SuspendLayout();
            tablelayoutpanelisland.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numMoney).BeginInit();
            ((System.ComponentModel.ISupportInitialize)logoislandmgt).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
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
            button1.Location = new Point(83, 229);
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
            button2.Location = new Point(589, 229);
            button2.Name = "button2";
            button2.Size = new Size(418, 361);
            button2.TabIndex = 1;
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // panelMii
            // 
            panelMii.Controls.Add(btnBrowseMii);
            panelMii.Controls.Add(txtMiiPath);
            panelMii.Controls.Add(label1);
            panelMii.Controls.Add(btnGo);
            panelMii.Controls.Add(cmbMiiAction);
            panelMii.Controls.Add(button4);
            panelMii.Controls.Add(button3);
            panelMii.Controls.Add(listBox1);
            panelMii.Controls.Add(logopanel1);
            panelMii.Controls.Add(pictureBox2);
            panelMii.Dock = DockStyle.Fill;
            panelMii.Location = new Point(0, 0);
            panelMii.MinimumSize = new Size(800, 476);
            panelMii.Name = "panelMii";
            panelMii.Size = new Size(1605, 845);
            panelMii.TabIndex = 3;
            panelMii.Visible = false;
            panelMii.Paint += panel1_Paint;
            // 
            // btnBrowseMii
            // 
            btnBrowseMii.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseMii.Location = new Point(1105, 165);
            btnBrowseMii.Margin = new Padding(3, 4, 3, 4);
            btnBrowseMii.Name = "btnBrowseMii";
            btnBrowseMii.Size = new Size(86, 29);
            btnBrowseMii.TabIndex = 10;
            btnBrowseMii.Text = "Browse..";
            btnBrowseMii.UseVisualStyleBackColor = true;
            btnBrowseMii.Click += btnBrowseMii_Click;
            // 
            // txtMiiPath
            // 
            txtMiiPath.AllowDrop = true;
            txtMiiPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtMiiPath.Location = new Point(341, 166);
            txtMiiPath.Margin = new Padding(3, 4, 3, 4);
            txtMiiPath.Name = "txtMiiPath";
            txtMiiPath.ReadOnly = true;
            txtMiiPath.Size = new Size(757, 26);
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
            // button4
            // 
            button4.Location = new Point(178, 71);
            button4.Name = "button4";
            button4.Size = new Size(147, 28);
            button4.TabIndex = 2;
            button4.Text = "Load Save Folder";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
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
            listBox1.Size = new Size(1258, 460);
            listBox1.TabIndex = 0;
            // 
            // logopanel1
            // 
            logopanel1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            logopanel1.BackColor = Color.Transparent;
            logopanel1.BackgroundImage = Properties.Resources.tomoaio_logo;
            logopanel1.BackgroundImageLayout = ImageLayout.Stretch;
            logopanel1.Location = new Point(1402, 4);
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
            pictureBox2.Size = new Size(1605, 845);
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
            btnIslandMgmt.Location = new Point(1081, 229);
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
            btnDiscord.Location = new Point(1517, 759);
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
            panelUGC.Size = new Size(1605, 845);
            panelUGC.TabIndex = 13;
            panelUGC.Visible = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureBox3.BackColor = Color.Transparent;
            pictureBox3.BackgroundImage = Properties.Resources.tomoaio_logo;
            pictureBox3.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox3.Location = new Point(1401, 4);
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
            btnUgcExport.Location = new Point(1226, 755);
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
            btnUgcImport.Location = new Point(368, 755);
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
            lblImageInfo.Location = new Point(739, 787);
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
            picPreview.Size = new Size(1202, 737);
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
            panelSidebar.Size = new Size(286, 845);
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
            lstUGC.Size = new Size(286, 810);
            lstUGC.TabIndex = 0;
            lstUGC.SelectedIndexChanged += lstUGC_SelectedIndexChanged;
            // 
            // panelIslandMGT
            // 
            panelIslandMGT.Controls.Add(tablelayoutpanelisland);
            panelIslandMGT.Controls.Add(lblCurrentMoney);
            panelIslandMGT.Controls.Add(btnMenuBack);
            panelIslandMGT.Controls.Add(btnSaveMoney);
            panelIslandMGT.Controls.Add(numMoney);
            panelIslandMGT.Controls.Add(IslandFund);
            panelIslandMGT.Controls.Add(lblIslandTitle);
            panelIslandMGT.Controls.Add(logoislandmgt);
            panelIslandMGT.Controls.Add(pictureBox4);
            panelIslandMGT.Location = new Point(0, 0);
            panelIslandMGT.Margin = new Padding(3, 4, 3, 4);
            panelIslandMGT.Name = "panelIslandMGT";
            panelIslandMGT.Size = new Size(1624, 1030);
            panelIslandMGT.TabIndex = 13;
            panelIslandMGT.Visible = false;
            // 
            // tablelayoutpanelisland
            // 
            tablelayoutpanelisland.Anchor = AnchorStyles.Top;
            tablelayoutpanelisland.ColumnCount = 2;
            tablelayoutpanelisland.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tablelayoutpanelisland.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tablelayoutpanelisland.Controls.Add(btnUnlockInteriors, 1, 1);
            tablelayoutpanelisland.Controls.Add(btnUnlockQBuilds, 0, 1);
            tablelayoutpanelisland.Controls.Add(btnUnlockClothes, 1, 0);
            tablelayoutpanelisland.Controls.Add(btnUnlockFood, 0, 0);
            tablelayoutpanelisland.Location = new Point(341, 512);
            tablelayoutpanelisland.Margin = new Padding(3, 4, 3, 4);
            tablelayoutpanelisland.Name = "tablelayoutpanelisland";
            tablelayoutpanelisland.RowCount = 2;
            tablelayoutpanelisland.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tablelayoutpanelisland.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tablelayoutpanelisland.Size = new Size(859, 172);
            tablelayoutpanelisland.TabIndex = 15;
            // 
            // btnUnlockInteriors
            // 
            btnUnlockInteriors.Dock = DockStyle.Fill;
            btnUnlockInteriors.Location = new Point(432, 90);
            btnUnlockInteriors.Margin = new Padding(3, 4, 3, 4);
            btnUnlockInteriors.Name = "btnUnlockInteriors";
            btnUnlockInteriors.Padding = new Padding(17, 19, 17, 19);
            btnUnlockInteriors.Size = new Size(424, 78);
            btnUnlockInteriors.TabIndex = 9;
            btnUnlockInteriors.Text = "Unlock all Interiors";
            btnUnlockInteriors.UseVisualStyleBackColor = true;
            btnUnlockInteriors.Click += btnUnlockInteriors_Click;
            // 
            // btnUnlockQBuilds
            // 
            btnUnlockQBuilds.Dock = DockStyle.Fill;
            btnUnlockQBuilds.Location = new Point(3, 90);
            btnUnlockQBuilds.Margin = new Padding(3, 4, 3, 4);
            btnUnlockQBuilds.Name = "btnUnlockQBuilds";
            btnUnlockQBuilds.Padding = new Padding(17, 19, 17, 19);
            btnUnlockQBuilds.Size = new Size(423, 78);
            btnUnlockQBuilds.TabIndex = 10;
            btnUnlockQBuilds.Text = "Unlock all Quik Builds";
            btnUnlockQBuilds.UseVisualStyleBackColor = true;
            btnUnlockQBuilds.Click += btnUnlockQBuilds_Click;
            // 
            // btnUnlockClothes
            // 
            btnUnlockClothes.Dock = DockStyle.Fill;
            btnUnlockClothes.Location = new Point(432, 4);
            btnUnlockClothes.Margin = new Padding(3, 4, 3, 4);
            btnUnlockClothes.Name = "btnUnlockClothes";
            btnUnlockClothes.Padding = new Padding(17, 19, 17, 19);
            btnUnlockClothes.Size = new Size(424, 78);
            btnUnlockClothes.TabIndex = 12;
            btnUnlockClothes.Text = "Unlock all Clothing";
            btnUnlockClothes.UseVisualStyleBackColor = true;
            btnUnlockClothes.Click += btnUnlockClothes_Click;
            // 
            // btnUnlockFood
            // 
            btnUnlockFood.Dock = DockStyle.Fill;
            btnUnlockFood.Location = new Point(3, 4);
            btnUnlockFood.Margin = new Padding(3, 4, 3, 4);
            btnUnlockFood.Name = "btnUnlockFood";
            btnUnlockFood.Padding = new Padding(17, 19, 17, 19);
            btnUnlockFood.Size = new Size(423, 78);
            btnUnlockFood.TabIndex = 11;
            btnUnlockFood.Text = "Unlock all Food";
            btnUnlockFood.UseVisualStyleBackColor = true;
            btnUnlockFood.Click += btnUnlockFood_Click;
            // 
            // lblCurrentMoney
            // 
            lblCurrentMoney.Anchor = AnchorStyles.Top;
            lblCurrentMoney.AutoSize = true;
            lblCurrentMoney.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCurrentMoney.ForeColor = Color.Black;
            lblCurrentMoney.Location = new Point(695, 284);
            lblCurrentMoney.Name = "lblCurrentMoney";
            lblCurrentMoney.Size = new Size(124, 21);
            lblCurrentMoney.TabIndex = 6;
            lblCurrentMoney.Text = "Current money";
            lblCurrentMoney.Click += lblCurrentMoney_Click;
            // 
            // btnMenuBack
            // 
            btnMenuBack.Location = new Point(3, 6);
            btnMenuBack.Margin = new Padding(3, 4, 3, 4);
            btnMenuBack.Name = "btnMenuBack";
            btnMenuBack.Size = new Size(201, 61);
            btnMenuBack.TabIndex = 5;
            btnMenuBack.Text = "Back to menu";
            btnMenuBack.UseVisualStyleBackColor = true;
            btnMenuBack.Click += btnMenuBack_Click1;
            // 
            // btnSaveMoney
            // 
            btnSaveMoney.Anchor = AnchorStyles.Top;
            btnSaveMoney.BackColor = Color.FromArgb(47, 61, 82);
            btnSaveMoney.Cursor = Cursors.Hand;
            btnSaveMoney.FlatAppearance.BorderSize = 0;
            btnSaveMoney.FlatStyle = FlatStyle.Flat;
            btnSaveMoney.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSaveMoney.ForeColor = Color.White;
            btnSaveMoney.Location = new Point(689, 398);
            btnSaveMoney.Margin = new Padding(3, 4, 3, 4);
            btnSaveMoney.Name = "btnSaveMoney";
            btnSaveMoney.Size = new Size(163, 49);
            btnSaveMoney.TabIndex = 3;
            btnSaveMoney.Text = "Change Funds";
            btnSaveMoney.UseVisualStyleBackColor = false;
            btnSaveMoney.Click += btnSaveMoney_Click;
            // 
            // numMoney
            // 
            numMoney.Anchor = AnchorStyles.Top;
            numMoney.Location = new Point(689, 341);
            numMoney.Margin = new Padding(3, 4, 3, 4);
            numMoney.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            numMoney.Name = "numMoney";
            numMoney.Size = new Size(163, 26);
            numMoney.TabIndex = 2;
            numMoney.ValueChanged += numMoney_ValueChanged;
            // 
            // IslandFund
            // 
            IslandFund.Anchor = AnchorStyles.Top;
            IslandFund.AutoSize = true;
            IslandFund.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            IslandFund.ForeColor = Color.Black;
            IslandFund.Location = new Point(689, 227);
            IslandFund.Name = "IslandFund";
            IslandFund.Size = new Size(167, 35);
            IslandFund.TabIndex = 1;
            IslandFund.Text = "Island Funds:";
            IslandFund.Click += IslandFund_Click;
            // 
            // lblIslandTitle
            // 
            lblIslandTitle.Anchor = AnchorStyles.Top;
            lblIslandTitle.AutoSize = true;
            lblIslandTitle.Font = new Font("Segoe UI", 26.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblIslandTitle.ForeColor = Color.Black;
            lblIslandTitle.Location = new Point(555, 10);
            lblIslandTitle.Name = "lblIslandTitle";
            lblIslandTitle.Size = new Size(452, 57);
            lblIslandTitle.TabIndex = 0;
            lblIslandTitle.Text = "Island name Manager";
            lblIslandTitle.Click += label3_Click;
            // 
            // logoislandmgt
            // 
            logoislandmgt.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            logoislandmgt.BackColor = Color.Transparent;
            logoislandmgt.BackgroundImage = Properties.Resources.tomoaio_logo;
            logoislandmgt.BackgroundImageLayout = ImageLayout.Zoom;
            logoislandmgt.Location = new Point(1400, 15);
            logoislandmgt.Margin = new Padding(3, 4, 3, 4);
            logoislandmgt.Name = "logoislandmgt";
            logoislandmgt.Size = new Size(200, 203);
            logoislandmgt.TabIndex = 16;
            logoislandmgt.TabStop = false;
            logoislandmgt.Click += logoislandmgt_Click;
            // 
            // pictureBox4
            // 
            pictureBox4.BackColor = Color.Transparent;
            pictureBox4.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox4.Dock = DockStyle.Fill;
            pictureBox4.Image = Properties.Resources.tomo1;
            pictureBox4.Location = new Point(0, 0);
            pictureBox4.Margin = new Padding(3, 4, 3, 4);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(1624, 1030);
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.TabIndex = 13;
            pictureBox4.TabStop = false;
            pictureBox4.Click += pictureBox4_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Image = Properties.Resources.tomo1;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Margin = new Padding(3, 4, 3, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1605, 845);
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
            logo.Location = new Point(1402, 4);
            logo.Margin = new Padding(3, 4, 3, 4);
            logo.Name = "logo";
            logo.Size = new Size(200, 203);
            logo.TabIndex = 11;
            logo.TabStop = false;
            logo.Click += pictureBox3_Click;
            // 
            // btnChangeFolders
            // 
            btnChangeFolders.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnChangeFolders.Location = new Point(7, 6);
            btnChangeFolders.Margin = new Padding(3, 4, 3, 4);
            btnChangeFolders.Name = "btnChangeFolders";
            btnChangeFolders.Size = new Size(221, 53);
            btnChangeFolders.TabIndex = 13;
            btnChangeFolders.Text = "Change Save Folders";
            btnChangeFolders.UseVisualStyleBackColor = true;
            btnChangeFolders.Click += btnChangeFolders_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 190, 0);
            ClientSize = new Size(1605, 845);
            Controls.Add(panelIslandMGT);
            Controls.Add(logo);
            Controls.Add(button2);
            Controls.Add(btnChangeFolders);
            Controls.Add(button1);
            Controls.Add(btnIslandMgmt);
            Controls.Add(btnDiscord);
            Controls.Add(pictureBox1);
            Controls.Add(panelMii);
            Controls.Add(panelUGC);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(1623, 890);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "TomoAIO - TLLTD Tool 2.0";
            Load += Form1_Load;
            Shown += Form1_Shown;
            panelMii.ResumeLayout(false);
            panelMii.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)logopanel1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            panelUGC.ResumeLayout(false);
            panelUGC.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)picPreview).EndInit();
            panelSidebar.ResumeLayout(false);
            panelSidebar.PerformLayout();
            panelIslandMGT.ResumeLayout(false);
            panelIslandMGT.PerformLayout();
            tablelayoutpanelisland.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numMoney).EndInit();
            ((System.ComponentModel.ISupportInitialize)logoislandmgt).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)logo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private Button button2;
        private Panel panelMii;
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
        private Panel panelIslandMGT;
        private Label lblIslandTitle;
        private NumericUpDown numMoney;
        private Button btnMenuBack;
        private Button btnSaveMoney;
        private Label lblCurrentMoney;
        private Button btnUnlockClothes;
        private Button btnUnlockFood;
        private Button btnUnlockQBuilds;
        private Button btnUnlockInteriors;
        private PictureBox pictureBox4;
        private TableLayoutPanel tablelayoutpanelisland;
        private Button btnChangeFolders;
        private PictureBox logoislandmgt;
    }
}