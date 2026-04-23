namespace TomoAIO.Components
{
    internal sealed class ResponsiveLayoutManager
    {
        public bool IsCompact(int width, int height) => width < 980 || height < 620;

        public bool ShouldStackButtons(int width, int height) => width < 430 || height < 520;
    }
}
