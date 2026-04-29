using System.Net.Http;

namespace TomoAIO.Services
{
    internal sealed class UpdateService
    {
        public async Task<string?> GetLatestVersionAsync(string owner, string repo)
        {
            string apiUrl = $"https://api.github.com/repos/{owner}/{repo}/releases/latest";
            using HttpClient client = new();
            client.DefaultRequestHeaders.Add("User-Agent", "TomoAIO-Updater");
            string response = await client.GetStringAsync(apiUrl);
            int tagIndex = response.IndexOf("\"tag_name\"", StringComparison.Ordinal);
            if (tagIndex == -1)
            {
                return null;
            }

            int startQuote = response.IndexOf('"', tagIndex + 11) + 1;
            int endQuote = response.IndexOf('"', startQuote);
            if (startQuote <= 0 || endQuote <= startQuote)
            {
                return null;
            }

            return response[startQuote..endQuote].Replace("v", "", StringComparison.OrdinalIgnoreCase);
        }
    }
}
