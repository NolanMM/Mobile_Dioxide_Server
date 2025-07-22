using System.Text.Json;

namespace Mobile_Server_Dioxide.Services.TickerServices
{
    public class TickerService : ITickerService
    {
        private const string JsonFilePath = "tickers.json";

        public async Task<List<string>?> GetTickersAsync()
        {
            if (!File.Exists(JsonFilePath))
            {
                return null;
            }

            string jsonContent = await File.ReadAllTextAsync(JsonFilePath);

            var tickers = JsonSerializer.Deserialize<List<string>>(jsonContent);

            return tickers;
        }
    }
}
