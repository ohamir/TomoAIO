using System.Collections.Generic;

namespace TomoAIO.Models
{
    public class AppState
    {
        public string CurrentMiiSavPath { get; set; } = string.Empty;

        public string CurrentUgcPath { get; set; } = string.Empty;

        public string SelectedMiiPath { get; set; } = "Choose a Mii file here...";

        public string? SelectedMiiAction { get; set; }
        public string CurrentPlayerSavPath { get; set; } = string.Empty;

        public List<UgcFileItem> UgcFiles { get; } = new();
    }
}
