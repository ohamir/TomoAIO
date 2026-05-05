namespace TomoAIO
{
    partial class IslandManagerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IslandManagerForm));
            btnUnlockFood = new Button();
            btnUnlockClothes = new Button();
            btnUnlockQBuilds = new Button();
            btnUnlockInteriors = new Button();
            btnSaveMoney = new Button();
            txtCurrentMoney = new NumericUpDown();
            lblCurrentMoney = new Label();
            lblIslandTitle = new Label();
            label1 = new Label();
            label3 = new Label();
            panel1 = new Panel();
            panel2 = new Panel();
            ((System.ComponentModel.ISupportInitialize)txtCurrentMoney).BeginInit();
            SuspendLayout();
            // 
            // btnUnlockFood
            // 
            btnUnlockFood.Location = new Point(340, 178);
            btnUnlockFood.Margin = new Padding(5, 7, 5, 7);
            btnUnlockFood.Name = "btnUnlockFood";
            btnUnlockFood.Size = new Size(292, 67);
            btnUnlockFood.TabIndex = 12;
            btnUnlockFood.Text = "Unlock all Food";
            btnUnlockFood.UseVisualStyleBackColor = true;
            btnUnlockFood.Click += btnUnlockFood_Click;
            // 
            // btnUnlockClothes
            // 
            btnUnlockClothes.Location = new Point(20, 178);
            btnUnlockClothes.Margin = new Padding(5, 7, 5, 7);
            btnUnlockClothes.Name = "btnUnlockClothes";
            btnUnlockClothes.Size = new Size(292, 67);
            btnUnlockClothes.TabIndex = 13;
            btnUnlockClothes.Text = "Unlock all Clothing";
            btnUnlockClothes.UseVisualStyleBackColor = true;
            btnUnlockClothes.Click += btnUnlockClothes_Click;
            // 
            // btnUnlockQBuilds
            // 
            btnUnlockQBuilds.Location = new Point(20, 281);
            btnUnlockQBuilds.Margin = new Padding(5, 7, 5, 7);
            btnUnlockQBuilds.Name = "btnUnlockQBuilds";
            btnUnlockQBuilds.Size = new Size(292, 67);
            btnUnlockQBuilds.TabIndex = 14;
            btnUnlockQBuilds.Text = "Unlock all Quik Builds";
            btnUnlockQBuilds.UseVisualStyleBackColor = true;
            btnUnlockQBuilds.Click += btnUnlockQBuilds_Click;
            // 
            // btnUnlockInteriors
            // 
            btnUnlockInteriors.Location = new Point(340, 281);
            btnUnlockInteriors.Margin = new Padding(5, 7, 5, 7);
            btnUnlockInteriors.Name = "btnUnlockInteriors";
            btnUnlockInteriors.Size = new Size(292, 67);
            btnUnlockInteriors.TabIndex = 15;
            btnUnlockInteriors.Text = "Unlock all Interiors";
            btnUnlockInteriors.UseVisualStyleBackColor = true;
            btnUnlockInteriors.Click += btnUnlockInteriors_Click;
            // 
            // btnSaveMoney
            // 
            btnSaveMoney.Location = new Point(1037, 240);
            btnSaveMoney.Margin = new Padding(5, 7, 5, 7);
            btnSaveMoney.Name = "btnSaveMoney";
            btnSaveMoney.Size = new Size(227, 67);
            btnSaveMoney.TabIndex = 16;
            btnSaveMoney.Text = "Update";
            btnSaveMoney.UseVisualStyleBackColor = true;
            btnSaveMoney.Click += btnSaveMoney_Click;
            // 
            // txtCurrentMoney
            // 
            txtCurrentMoney.Location = new Point(759, 255);
            txtCurrentMoney.Margin = new Padding(5, 7, 5, 7);
            txtCurrentMoney.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            txtCurrentMoney.Name = "txtCurrentMoney";
            txtCurrentMoney.Size = new Size(223, 39);
            txtCurrentMoney.TabIndex = 17;
            // 
            // lblCurrentMoney
            // 
            lblCurrentMoney.AutoSize = true;
            lblCurrentMoney.BackColor = Color.Transparent;
            lblCurrentMoney.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCurrentMoney.Location = new Point(759, 195);
            lblCurrentMoney.Margin = new Padding(5, 0, 5, 0);
            lblCurrentMoney.Name = "lblCurrentMoney";
            lblCurrentMoney.Size = new Size(196, 32);
            lblCurrentMoney.TabIndex = 19;
            lblCurrentMoney.Text = "Current Balance";
            // 
            // lblIslandTitle
            // 
            lblIslandTitle.AutoSize = true;
            lblIslandTitle.BackColor = Color.Transparent;
            lblIslandTitle.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            lblIslandTitle.Location = new Point(548, 23);
            lblIslandTitle.Margin = new Padding(5, 0, 5, 0);
            lblIslandTitle.Name = "lblIslandTitle";
            lblIslandTitle.Size = new Size(171, 45);
            lblIslandTitle.TabIndex = 20;
            lblIslandTitle.Text = "Island title";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(759, 95);
            label1.Name = "label1";
            label1.Size = new Size(291, 37);
            label1.TabIndex = 21;
            label1.Text = "Change island money";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("Segoe UI", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(20, 95);
            label3.Name = "label3";
            label3.Size = new Size(246, 37);
            label3.TabIndex = 23;
            label3.Text = "Unlockable items:";
            // 
            // panel1
            // 
            panel1.BackColor = Color.Transparent;
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Location = new Point(739, 115);
            panel1.Name = "panel1";
            panel1.Size = new Size(537, 295);
            panel1.TabIndex = 24;
            // 
            // panel2
            // 
            panel2.BackColor = Color.Transparent;
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Location = new Point(5, 115);
            panel2.Name = "panel2";
            panel2.Size = new Size(689, 295);
            panel2.TabIndex = 25;
            // 
            // IslandManagerForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.tomo1;
            ClientSize = new Size(1300, 476);
            Controls.Add(label3);
            Controls.Add(label1);
            Controls.Add(lblIslandTitle);
            Controls.Add(lblCurrentMoney);
            Controls.Add(txtCurrentMoney);
            Controls.Add(btnSaveMoney);
            Controls.Add(btnUnlockInteriors);
            Controls.Add(btnUnlockQBuilds);
            Controls.Add(btnUnlockClothes);
            Controls.Add(btnUnlockFood);
            Controls.Add(panel1);
            Controls.Add(panel2);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(5);
            MinimumSize = new Size(1326, 547);
            Name = "IslandManagerForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "IslandManagerForm";
            ((System.ComponentModel.ISupportInitialize)txtCurrentMoney).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnUnlockFood;
        private Button btnUnlockClothes;
        private Button btnUnlockQBuilds;
        private Button btnUnlockInteriors;
        private Button btnSaveMoney;
        private NumericUpDown txtCurrentMoney;
        private Label lblCurrentMoney;
        private Label lblIslandTitle;
        private Label label1;
        private Label label3;
        private Panel panel1;
        private Panel panel2;
    }
}