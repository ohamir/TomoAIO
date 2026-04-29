using System.Windows.Forms;

namespace TomoAIO.Components
{
    internal sealed class CustomActionDropdown
    {
        public Panel Host { get; }
        public Label TextLabel { get; }
        public Button ArrowButton { get; }
        public ListBox List { get; }

        public bool IsOpen { get; private set; }

        public CustomActionDropdown(Panel parent, string[] options)
        {
            Host = new Panel();
            TextLabel = new Label();
            ArrowButton = new Button();
            List = new ListBox();

            parent.Controls.Add(Host);
            parent.Controls.Add(List);

            TextLabel.Text = "Select Action...";
            TextLabel.AutoSize = false;
            TextLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            TextLabel.Cursor = Cursors.Hand;

            ArrowButton.Text = "▼";
            ArrowButton.FlatStyle = FlatStyle.Flat;
            ArrowButton.FlatAppearance.BorderSize = 0;
            ArrowButton.TabStop = false;
            ArrowButton.Cursor = Cursors.Hand;

            List.Visible = false;
            List.Items.AddRange(options);

            Host.Controls.Add(TextLabel);
            Host.Controls.Add(ArrowButton);
        }

        public void Toggle()
        {
            IsOpen = !IsOpen;
            List.Visible = IsOpen;
            ArrowButton.Text = IsOpen ? "▲" : "▼";
        }

        public void Close()
        {
            IsOpen = false;
            List.Visible = false;
            ArrowButton.Text = "▼";
        }
    }
}
