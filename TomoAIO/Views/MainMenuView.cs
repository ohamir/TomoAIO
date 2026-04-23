using System.Windows.Forms;

namespace TomoAIO.Views
{
    internal sealed class MainMenuView
    {
        private readonly PictureBox _background;
        private readonly PictureBox _logo;
        private readonly Button _miiButton;
        private readonly Button _ugcButton;

        public MainMenuView(PictureBox background, PictureBox logo, Button miiButton, Button ugcButton)
        {
            _background = background;
            _logo = logo;
            _miiButton = miiButton;
            _ugcButton = ugcButton;
        }

        public void Show()
        {
            _background.Visible = true;
            _background.BringToFront();
            _logo.BringToFront();
            _miiButton.BringToFront();
            _ugcButton.BringToFront();
        }
    }
}
