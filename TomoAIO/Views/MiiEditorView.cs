using System.Windows.Forms;

namespace TomoAIO.Views
{
    internal sealed class MiiEditorView
    {
        private readonly Panel _panel;

        public MiiEditorView(Panel panel)
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
