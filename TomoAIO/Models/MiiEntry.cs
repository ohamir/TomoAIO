namespace TomoAIO.Models
{
    internal sealed class MiiEntry
    {
        public int Slot { get; init; }

        public required string Name { get; init; }

        public override string ToString() => $"{Slot + 1}: {Name}";
    }
}
