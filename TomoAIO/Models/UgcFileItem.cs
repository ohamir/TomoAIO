namespace TomoAIO.Models
{
    internal sealed class UgcFileItem
    {
        public required string FileName { get; init; }

        public required string DisplayName { get; init; }

        public bool IsCanvas => FileName.EndsWith(".canvas.zs", StringComparison.OrdinalIgnoreCase);

        public bool IsTexture => FileName.EndsWith(".ugctex.zs", StringComparison.OrdinalIgnoreCase);
    }
}
