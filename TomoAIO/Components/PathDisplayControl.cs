using System.Windows.Forms;

namespace TomoAIO.Components
{
    internal sealed class PathDisplayControl
    {
        public Panel Host { get; }
        public Label Label { get; }

        public PathDisplayControl(Panel parent, string initialText)
        {
            Host = new Panel();
            Label = new Label
            {
                Text = initialText,
                AutoSize = false,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                Padding = new Padding(8, 0, 8, 0),
                AutoEllipsis = true
            };

            Host.Padding = new Padding(1);
            Host.Controls.Add(Label);
            parent.Controls.Add(Host);
        }

        public void SetText(string text) => Label.Text = text;
    }
}
