namespace TomoAIO
{
    partial class MainForm
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
            ImportMiiBtn = new Button();
            UgcEditorBtn = new Button();
            logo = new PictureBox();
            IslandManagerBtn = new Button();
            DiscordJoinBtn = new Button();
            ((System.ComponentModel.ISupportInitialize)logo).BeginInit();
            SuspendLayout();
            // 
            // ImportMiiBtn
            // 
            ImportMiiBtn.Anchor = AnchorStyles.None;
            ImportMiiBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ImportMiiBtn.BackColor = Color.Transparent;
            ImportMiiBtn.BackgroundImage = Properties.Resources.mii_import1;
            ImportMiiBtn.BackgroundImageLayout = ImageLayout.Zoom;
            ImportMiiBtn.Cursor = Cursors.Hand;
            ImportMiiBtn.FlatAppearance.BorderSize = 0;
            ImportMiiBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 255, 192);
            ImportMiiBtn.FlatStyle = FlatStyle.Flat;
            ImportMiiBtn.Location = new Point(20, 335);
            ImportMiiBtn.Margin = new Padding(5, 5, 5, 5);
            ImportMiiBtn.Name = "ImportMiiBtn";
            ImportMiiBtn.Size = new Size(341, 290);
            ImportMiiBtn.TabIndex = 1;
            ImportMiiBtn.UseVisualStyleBackColor = false;
            ImportMiiBtn.Click += ImportMiiBtn_Click;
            // 
            // UgcEditorBtn
            // 
            UgcEditorBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            UgcEditorBtn.BackColor = Color.Transparent;
            UgcEditorBtn.BackgroundImage = Properties.Resources.UGC_CREATOR;
            UgcEditorBtn.BackgroundImageLayout = ImageLayout.Zoom;
            UgcEditorBtn.Cursor = Cursors.Hand;
            UgcEditorBtn.FlatAppearance.BorderSize = 0;
            UgcEditorBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 255, 192);
            UgcEditorBtn.FlatStyle = FlatStyle.Flat;
            UgcEditorBtn.Location = new Point(370, 333);
            UgcEditorBtn.Margin = new Padding(5, 5, 5, 5);
            UgcEditorBtn.Name = "UgcEditorBtn";
            UgcEditorBtn.Size = new Size(341, 290);
            UgcEditorBtn.TabIndex = 2;
            UgcEditorBtn.UseVisualStyleBackColor = false;
            UgcEditorBtn.Click += UgcEditorBtn_Click;
            // 
            // logo
            // 
            logo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            logo.BackColor = Color.Transparent;
            logo.BackgroundImage = Properties.Resources.tomoaio_logo;
            logo.BackgroundImageLayout = ImageLayout.Zoom;
            logo.Location = new Point(400, -3);
            logo.Margin = new Padding(5, 7, 5, 7);
            logo.Name = "logo";
            logo.Size = new Size(301, 312);
            logo.TabIndex = 12;
            logo.TabStop = false;
            // 
            // IslandManagerBtn
            // 
            IslandManagerBtn.Anchor = AnchorStyles.None;
            IslandManagerBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            IslandManagerBtn.BackColor = Color.Transparent;
            IslandManagerBtn.BackgroundImage = Properties.Resources.islandmanager;
            IslandManagerBtn.BackgroundImageLayout = ImageLayout.Zoom;
            IslandManagerBtn.Cursor = Cursors.Hand;
            IslandManagerBtn.FlatAppearance.BorderSize = 0;
            IslandManagerBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 255, 192);
            IslandManagerBtn.FlatStyle = FlatStyle.Flat;
            IslandManagerBtn.Location = new Point(739, 335);
            IslandManagerBtn.Margin = new Padding(5, 5, 5, 5);
            IslandManagerBtn.Name = "IslandManagerBtn";
            IslandManagerBtn.Size = new Size(341, 290);
            IslandManagerBtn.TabIndex = 16;
            IslandManagerBtn.UseVisualStyleBackColor = false;
            IslandManagerBtn.Click += IslandManagerBtn_Click_1;
            // 
            // DiscordJoinBtn
            // 
            DiscordJoinBtn.BackColor = Color.Transparent;
            DiscordJoinBtn.BackgroundImage = Properties.Resources.discord;
            DiscordJoinBtn.BackgroundImageLayout = ImageLayout.Zoom;
            DiscordJoinBtn.FlatAppearance.BorderSize = 0;
            DiscordJoinBtn.FlatAppearance.MouseDownBackColor = Color.Transparent;
            DiscordJoinBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 255, 192);
            DiscordJoinBtn.FlatStyle = FlatStyle.Flat;
            DiscordJoinBtn.Location = new Point(1001, 635);
            DiscordJoinBtn.Margin = new Padding(5, 5, 5, 5);
            DiscordJoinBtn.Name = "DiscordJoinBtn";
            DiscordJoinBtn.Size = new Size(106, 72);
            DiscordJoinBtn.TabIndex = 18;
            DiscordJoinBtn.UseVisualStyleBackColor = false;
            DiscordJoinBtn.Click += DiscordJoinBtn_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.tomo1;
            ClientSize = new Size(1100, 707);
            Controls.Add(DiscordJoinBtn);
            Controls.Add(IslandManagerBtn);
            Controls.Add(logo);
            Controls.Add(UgcEditorBtn);
            Controls.Add(ImportMiiBtn);
            Margin = new Padding(5, 5, 5, 5);
            MinimumSize = new Size(1113, 735);
            Name = "MainForm";
            SizeGripStyle = SizeGripStyle.Show;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "TomoAIO - All in one multi-tool";
            ((System.ComponentModel.ISupportInitialize)logo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button ImportMiiBtn;
        private Button UgcEditorBtn;
        private PictureBox logo;
        private Button IslandManagerBtn;
        private Button DiscordJoinBtn;
    }
}