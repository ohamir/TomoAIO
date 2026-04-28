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
            SelectActionComboBox = new ComboBox();
            PathToSaveTxtBox = new TextBox();
            BrowseBtn = new Button();
            label1 = new Label();
            DisplayMiiLstBox = new ListBox();
            label2 = new Label();
            ApplyChangesBtn = new Button();
            LoadSaveBtn = new Button();
            SuspendLayout();
            // 
            // SelectActionComboBox
            // 
            SelectActionComboBox.FormattingEnabled = true;
            SelectActionComboBox.Items.AddRange(new object[] { "Import as .ltd", "Export as .ltd" });
            SelectActionComboBox.Location = new Point(12, 37);
            SelectActionComboBox.Name = "SelectActionComboBox";
            SelectActionComboBox.Size = new Size(144, 27);
            SelectActionComboBox.TabIndex = 0;
            SelectActionComboBox.Text = "Import / export .ltd";
            // 
            // PathToSaveTxtBox
            // 
            PathToSaveTxtBox.Location = new Point(12, 107);
            PathToSaveTxtBox.Name = "PathToSaveTxtBox";
            PathToSaveTxtBox.Size = new Size(355, 26);
            PathToSaveTxtBox.TabIndex = 1;
            PathToSaveTxtBox.Text = "Choose a Mii file here...";
            // 
            // BrowseBtn
            // 
            BrowseBtn.Location = new Point(373, 106);
            BrowseBtn.Name = "BrowseBtn";
            BrowseBtn.Size = new Size(89, 27);
            BrowseBtn.TabIndex = 2;
            BrowseBtn.Text = "Browse";
            BrowseBtn.UseVisualStyleBackColor = true;
            BrowseBtn.Click += BrowseBtn_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(12, 79);
            label1.Name = "label1";
            label1.Size = new Size(194, 25);
            label1.TabIndex = 3;
            label1.Text = "Save / import as Mii:";
            // 
            // DisplayMiiLstBox
            // 
            DisplayMiiLstBox.FormattingEnabled = true;
            DisplayMiiLstBox.Location = new Point(12, 156);
            DisplayMiiLstBox.Name = "DisplayMiiLstBox";
            DisplayMiiLstBox.Size = new Size(512, 384);
            DisplayMiiLstBox.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(12, 9);
            label2.Name = "label2";
            label2.Size = new Size(132, 25);
            label2.TabIndex = 5;
            label2.Text = "Select Action:";
            // 
            // ApplyChangesBtn
            // 
            ApplyChangesBtn.Location = new Point(12, 545);
            ApplyChangesBtn.Name = "ApplyChangesBtn";
            ApplyChangesBtn.Size = new Size(89, 27);
            ApplyChangesBtn.TabIndex = 6;
            ApplyChangesBtn.Text = "Apply";
            ApplyChangesBtn.UseVisualStyleBackColor = true;
            ApplyChangesBtn.Click += ApplyChangesBtn_Click;
            // 
            // LoadSaveBtn
            // 
            LoadSaveBtn.Location = new Point(393, 10);
            LoadSaveBtn.Name = "LoadSaveBtn";
            LoadSaveBtn.Size = new Size(131, 27);
            LoadSaveBtn.TabIndex = 7;
            LoadSaveBtn.Text = "Load save file";
            LoadSaveBtn.UseVisualStyleBackColor = true;
            LoadSaveBtn.Click += LoadSaveBtn_Click;
            // 
            // MiiImportForm
            // 
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.tomo1;
            ClientSize = new Size(536, 584);
            Controls.Add(LoadSaveBtn);
            Controls.Add(ApplyChangesBtn);
            Controls.Add(label2);
            Controls.Add(DisplayMiiLstBox);
            Controls.Add(label1);
            Controls.Add(BrowseBtn);
            Controls.Add(PathToSaveTxtBox);
            Controls.Add(SelectActionComboBox);
            MaximizeBox = false;
            Name = "MiiImportForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "MiiForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox SelectActionComboBox;
        private TextBox PathToSaveTxtBox;
        private Button BrowseBtn;
        private Label label1;
        private ListBox DisplayMiiLstBox;
        private Label label2;
        private Button ApplyChangesBtn;
        private Button LoadSaveBtn;
    }
}