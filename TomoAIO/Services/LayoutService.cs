using System.Drawing;
using TomoAIO.Models;

namespace TomoAIO.Services
{
    internal sealed class LayoutService
    {
        public (Size size, Point location) ComputeTopRightLogo(Size parentClientSize, float scaleX, float scaleY)
        {
            float logoScale = Math.Max(0.6f, Math.Min(1.35f, Math.Min(scaleX, scaleY)));
            int logoWidth = Math.Max(110, (int)Math.Round(UiConstants.BaseLogoSize.Width * logoScale));
            int logoHeight = Math.Max(100, (int)Math.Round(UiConstants.BaseLogoSize.Height * logoScale));
            Point location = new(
                Math.Max(0, parentClientSize.Width - logoWidth - UiConstants.LogoMargin),
                UiConstants.LogoMargin);
            return (new Size(logoWidth, logoHeight), location);
        }

        public Point ComputeBottomRight(Size parentClientSize, Size controlSize, int margin)
        {
            return new Point(
                Math.Max(0, parentClientSize.Width - controlSize.Width - margin),
                Math.Max(0, parentClientSize.Height - controlSize.Height - margin));
        }
    }
}
