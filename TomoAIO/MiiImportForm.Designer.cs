namespace TomoAIO
{
    partial class MiiImportForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MiiImportForm));
            SelectActionComboBox = new ComboBox();
            DisplayMiiLstBox = new ListBox();
            label2 = new Label();
            ApplyChangesBtn = new Button();
            LoadSaveBtn = new Button();
            SuspendLayout();
            // 
            // SelectActionComboBox
            // 
            SelectActionComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            SelectActionComboBox.FlatStyle = FlatStyle.Flat;
            SelectActionComboBox.Font = new Font("Segoe UI", 10.125F, FontStyle.Regular, GraphicsUnit.Point, 0);
            SelectActionComboBox.ForeColor = Color.Black;
            SelectActionComboBox.FormattingEnabled = true;
            SelectActionComboBox.Items.AddRange(new object[] { "Import as .ltd", "Export as .ltd" });
            SelectActionComboBox.Location = new Point(244, 18);
            SelectActionComboBox.Margin = new Padding(5);
            SelectActionComboBox.Name = "SelectActionComboBox";
            SelectActionComboBox.Size = new Size(232, 45);
            SelectActionComboBox.TabIndex = 0;
            // 
            // DisplayMiiLstBox
            // 
            DisplayMiiLstBox.BackColor = Color.FromArgb(250, 250, 250);
            DisplayMiiLstBox.BorderStyle = BorderStyle.None;
            DisplayMiiLstBox.Font = new Font("Segoe UI", 10.125F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DisplayMiiLstBox.FormattingEnabled = true;
            DisplayMiiLstBox.Location = new Point(20, 152);
            DisplayMiiLstBox.Margin = new Padding(5);
            DisplayMiiLstBox.Name = "DisplayMiiLstBox";
            DisplayMiiLstBox.Size = new Size(1108, 1036);
            DisplayMiiLstBox.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(20, 15);
            label2.Margin = new Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new Size(224, 45);
            label2.TabIndex = 5;
            label2.Text = "Select Action:";
            // 
            // ApplyChangesBtn
            // 
            ApplyChangesBtn.Location = new Point(503, 8);
            ApplyChangesBtn.Margin = new Padding(5);
            ApplyChangesBtn.Name = "ApplyChangesBtn";
            ApplyChangesBtn.Size = new Size(141, 74);
            ApplyChangesBtn.TabIndex = 6;
            ApplyChangesBtn.Text = "Go";
            ApplyChangesBtn.UseVisualStyleBackColor = true;
            ApplyChangesBtn.Click += ApplyChangesBtn_Click;
            // 
            // LoadSaveBtn
            // 
            LoadSaveBtn.BackColor = Color.FromArgb(47, 61, 82);
            LoadSaveBtn.FlatAppearance.BorderColor = Color.Black;
            LoadSaveBtn.FlatAppearance.MouseDownBackColor = Color.FromArgb(35, 46, 62);
            LoadSaveBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(61, 79, 106);
            LoadSaveBtn.Font = new Font("Segoe UI Black", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 0);
            LoadSaveBtn.ForeColor = Color.White;
            LoadSaveBtn.Location = new Point(866, 15);
            LoadSaveBtn.Margin = new Padding(0);
            LoadSaveBtn.Name = "LoadSaveBtn";
            LoadSaveBtn.Padding = new Padding(8, 3, 8, 3);
            LoadSaveBtn.Size = new Size(262, 73);
            LoadSaveBtn.TabIndex = 7;
            LoadSaveBtn.Text = "Reload Save";
            LoadSaveBtn.UseVisualStyleBackColor = false;
            LoadSaveBtn.Click += LoadSaveBtn_Click;
            // 
            // MiiImportForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.tomo1;
            ClientSize = new Size(1142, 1216);
            Controls.Add(label2);
            Controls.Add(LoadSaveBtn);
            Controls.Add(ApplyChangesBtn);
            Controls.Add(DisplayMiiLstBox);
            Controls.Add(SelectActionComboBox);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(5);
            MinimumSize = new Size(1155, 1243);
            Name = "MiiImportForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "TomoAIO! Mii Import / Export";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox SelectActionComboBox;
        private ListBox DisplayMiiLstBox;
        private Label label2;
        private Button ApplyChangesBtn;
        private Button LoadSaveBtn;
    }
}