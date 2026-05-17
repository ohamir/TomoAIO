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
            btnUnlockFood.Location = new Point(209, 106);
            btnUnlockFood.Margin = new Padding(3, 4, 3, 4);
            btnUnlockFood.Name = "btnUnlockFood";
            btnUnlockFood.Size = new Size(180, 40);
            btnUnlockFood.TabIndex = 12;
            btnUnlockFood.Text = "Unlock all Food";
            btnUnlockFood.UseVisualStyleBackColor = true;
            btnUnlockFood.Click += btnUnlockFood_Click;
            // 
            // btnUnlockClothes
            // 
            btnUnlockClothes.Location = new Point(12, 106);
            btnUnlockClothes.Margin = new Padding(3, 4, 3, 4);
            btnUnlockClothes.Name = "btnUnlockClothes";
            btnUnlockClothes.Size = new Size(180, 40);
            btnUnlockClothes.TabIndex = 13;
            btnUnlockClothes.Text = "Unlock all Clothing";
            btnUnlockClothes.UseVisualStyleBackColor = true;
            btnUnlockClothes.Click += btnUnlockClothes_Click;
            // 
            // btnUnlockQBuilds
            // 
            btnUnlockQBuilds.Location = new Point(12, 167);
            btnUnlockQBuilds.Margin = new Padding(3, 4, 3, 4);
            btnUnlockQBuilds.Name = "btnUnlockQBuilds";
            btnUnlockQBuilds.Size = new Size(180, 40);
            btnUnlockQBuilds.TabIndex = 14;
            btnUnlockQBuilds.Text = "Unlock all Quik Builds";
            btnUnlockQBuilds.UseVisualStyleBackColor = true;
            btnUnlockQBuilds.Click += btnUnlockQBuilds_Click;
            // 
            // btnUnlockInteriors
            // 
            btnUnlockInteriors.Location = new Point(209, 167);
            btnUnlockInteriors.Margin = new Padding(3, 4, 3, 4);
            btnUnlockInteriors.Name = "btnUnlockInteriors";
            btnUnlockInteriors.Size = new Size(180, 40);
            btnUnlockInteriors.TabIndex = 15;
            btnUnlockInteriors.Text = "Unlock all Interiors";
            btnUnlockInteriors.UseVisualStyleBackColor = true;
            btnUnlockInteriors.Click += btnUnlockInteriors_Click;
            // 
            // btnSaveMoney
            // 
            btnSaveMoney.Location = new Point(638, 142);
            btnSaveMoney.Margin = new Padding(3, 4, 3, 4);
            btnSaveMoney.Name = "btnSaveMoney";
            btnSaveMoney.Size = new Size(140, 40);
            btnSaveMoney.TabIndex = 16;
            btnSaveMoney.Text = "Update";
            btnSaveMoney.UseVisualStyleBackColor = true;
            btnSaveMoney.Click += btnSaveMoney_Click;
            // 
            // txtCurrentMoney
            // 
            txtCurrentMoney.Location = new Point(467, 151);
            txtCurrentMoney.Margin = new Padding(3, 4, 3, 4);
            txtCurrentMoney.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            txtCurrentMoney.Name = "txtCurrentMoney";
            txtCurrentMoney.Size = new Size(137, 26);
            txtCurrentMoney.TabIndex = 17;
            // 
            // lblCurrentMoney
            // 
            lblCurrentMoney.AutoSize = true;
            lblCurrentMoney.BackColor = Color.Transparent;
            lblCurrentMoney.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCurrentMoney.Location = new Point(467, 116);
            lblCurrentMoney.Name = "lblCurrentMoney";
            lblCurrentMoney.Size = new Size(120, 20);
            lblCurrentMoney.TabIndex = 19;
            lblCurrentMoney.Text = "Current Balance";
            // 
            // lblIslandTitle
            // 
            lblIslandTitle.AutoSize = true;
            lblIslandTitle.BackColor = Color.Transparent;
            lblIslandTitle.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            lblIslandTitle.Location = new Point(337, 14);
            lblIslandTitle.Name = "lblIslandTitle";
            lblIslandTitle.Size = new Size(103, 25);
            lblIslandTitle.TabIndex = 20;
            lblIslandTitle.Text = "Island title";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(467, 56);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(181, 23);
            label1.TabIndex = 21;
            label1.Text = "Change island money";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("Segoe UI", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(12, 56);
            label3.Margin = new Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new Size(153, 23);
            label3.TabIndex = 23;
            label3.Text = "Unlockable items:";
            // 
            // panel1
            // 
            panel1.BackColor = Color.Transparent;
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Location = new Point(455, 68);
            panel1.Margin = new Padding(2);
            panel1.Name = "panel1";
            panel1.Size = new Size(331, 176);
            panel1.TabIndex = 24;
            // 
            // panel2
            // 
            panel2.BackColor = Color.Transparent;
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Location = new Point(3, 68);
            panel2.Margin = new Padding(2);
            panel2.Name = "panel2";
            panel2.Size = new Size(425, 176);
            panel2.TabIndex = 25;
            // 
            // IslandManagerForm
            // 
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.tomo1;
            ClientSize = new Size(805, 298);
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
            MinimumSize = new Size(823, 343);
            Name = "IslandManagerForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "TomoAIO! Island Manager";
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