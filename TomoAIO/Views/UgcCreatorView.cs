using System.Windows.Forms;

namespace TomoAIO.Views
{
    internal sealed class UgcCreatorView
    {
        private readonly Panel _panel;

        public UgcCreatorView(Panel panel)
        {
            _panel = panel;
        }

        public void Show()
        {
            _panel.Visible = true;
            _panel.BringToFront();
        }
    }
}
