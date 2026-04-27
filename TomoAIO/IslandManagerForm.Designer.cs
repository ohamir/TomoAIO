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
            button4 = new Button();
            btnUnlockFood = new Button();
            btnUnlockClothes = new Button();
            btnUnlockQBuilds = new Button();
            btnUnlockInteriors = new Button();
            btnSaveMoney = new Button();
            numMoney = new NumericUpDown();
            IslandFund = new Label();
            lblCurrentMoney = new Label();
            lblIslandTitle = new Label();
            ((System.ComponentModel.ISupportInitialize)numMoney).BeginInit();
            SuspendLayout();
            // 
            // button4
            // 
            button4.Location = new Point(12, 12);
            button4.Name = "button4";
            button4.Size = new Size(147, 28);
            button4.TabIndex = 3;
            button4.Text = "Load Save Folder";
            button4.UseVisualStyleBackColor = true;
            // 
            // btnUnlockFood
            // 
            btnUnlockFood.Location = new Point(12, 68);
            btnUnlockFood.Margin = new Padding(3, 4, 3, 4);
            btnUnlockFood.Name = "btnUnlockFood";
            btnUnlockFood.Size = new Size(217, 51);
            btnUnlockFood.TabIndex = 12;
            btnUnlockFood.Text = "Unlock all Food";
            btnUnlockFood.UseVisualStyleBackColor = true;
            // 
            // btnUnlockClothes
            // 
            btnUnlockClothes.Location = new Point(12, 141);
            btnUnlockClothes.Margin = new Padding(3, 4, 3, 4);
            btnUnlockClothes.Name = "btnUnlockClothes";
            btnUnlockClothes.Size = new Size(173, 47);
            btnUnlockClothes.TabIndex = 13;
            btnUnlockClothes.Text = "Unlock all Clothing";
            btnUnlockClothes.UseVisualStyleBackColor = true;
            // 
            // btnUnlockQBuilds
            // 
            btnUnlockQBuilds.Location = new Point(12, 213);
            btnUnlockQBuilds.Margin = new Padding(3, 4, 3, 4);
            btnUnlockQBuilds.Name = "btnUnlockQBuilds";
            btnUnlockQBuilds.Size = new Size(183, 52);
            btnUnlockQBuilds.TabIndex = 14;
            btnUnlockQBuilds.Text = "Unlock all Quik Builds";
            btnUnlockQBuilds.UseVisualStyleBackColor = true;
            // 
            // btnUnlockInteriors
            // 
            btnUnlockInteriors.Location = new Point(12, 285);
            btnUnlockInteriors.Margin = new Padding(3, 4, 3, 4);
            btnUnlockInteriors.Name = "btnUnlockInteriors";
            btnUnlockInteriors.Size = new Size(189, 46);
            btnUnlockInteriors.TabIndex = 15;
            btnUnlockInteriors.Text = "Unlock all Interiors";
            btnUnlockInteriors.UseVisualStyleBackColor = true;
            // 
            // btnSaveMoney
            // 
            btnSaveMoney.Location = new Point(441, 68);
            btnSaveMoney.Margin = new Padding(3, 4, 3, 4);
            btnSaveMoney.Name = "btnSaveMoney";
            btnSaveMoney.Size = new Size(135, 46);
            btnSaveMoney.TabIndex = 16;
            btnSaveMoney.Text = "Save Money";
            btnSaveMoney.UseVisualStyleBackColor = true;
            // 
            // numMoney
            // 
            numMoney.Location = new Point(441, 141);
            numMoney.Margin = new Padding(3, 4, 3, 4);
            numMoney.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            numMoney.Name = "numMoney";
            numMoney.Size = new Size(137, 26);
            numMoney.TabIndex = 17;
            // 
            // IslandFund
            // 
            IslandFund.AutoSize = true;
            IslandFund.Location = new Point(441, 204);
            IslandFund.Name = "IslandFund";
            IslandFund.Size = new Size(91, 19);
            IslandFund.TabIndex = 18;
            IslandFund.Text = "Island money";
            // 
            // lblCurrentMoney
            // 
            lblCurrentMoney.AutoSize = true;
            lblCurrentMoney.Location = new Point(441, 268);
            lblCurrentMoney.Name = "lblCurrentMoney";
            lblCurrentMoney.Size = new Size(102, 19);
            lblCurrentMoney.TabIndex = 19;
            lblCurrentMoney.Text = "Current money";
            // 
            // lblIslandTitle
            // 
            lblIslandTitle.AutoSize = true;
            lblIslandTitle.Location = new Point(378, 21);
            lblIslandTitle.Name = "lblIslandTitle";
            lblIslandTitle.Size = new Size(45, 19);
            lblIslandTitle.TabIndex = 20;
            lblIslandTitle.Text = "label3";
            // 
            // IslandManagerForm
            // 
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.tomo1;
            ClientSize = new Size(800, 450);
            Controls.Add(lblIslandTitle);
            Controls.Add(lblCurrentMoney);
            Controls.Add(IslandFund);
            Controls.Add(numMoney);
            Controls.Add(btnSaveMoney);
            Controls.Add(btnUnlockInteriors);
            Controls.Add(btnUnlockQBuilds);
            Controls.Add(btnUnlockClothes);
            Controls.Add(btnUnlockFood);
            Controls.Add(button4);
            Name = "IslandManagerForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "IslandManagerForm";
            ((System.ComponentModel.ISupportInitialize)numMoney).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button4;
        private Button btnUnlockFood;
        private Button btnUnlockClothes;
        private Button btnUnlockQBuilds;
        private Button btnUnlockInteriors;
        private Button btnSaveMoney;
        private NumericUpDown numMoney;
        private Label IslandFund;
        private Label lblCurrentMoney;
        private Label lblIslandTitle;
    }
}