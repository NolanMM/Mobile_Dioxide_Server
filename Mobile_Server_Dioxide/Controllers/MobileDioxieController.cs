using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mobile_Server_Dioxide.Context;
using Mobile_Server_Dioxide.Entities;

namespace Mobile_Server_Dioxide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileDioxieController : ControllerBase
    {
        private readonly ILogger<MobileDioxieController> _logger;
        private DioxieReadDbContext _dioxieReadDbContext;

        public MobileDioxieController(ILogger<MobileDioxieController> logger, DioxieReadDbContext dioxieReadDbContext)
        {
            _logger = logger;
            _dioxieReadDbContext = dioxieReadDbContext;
        }

        [HttpGet("Stock/Symbols/Available")]
        public async Task<IActionResult> GetAvailableStockSymbols()
        {
            var symbols = await _dioxieReadDbContext.HistoricalPricesStockWithTACompanyInformationGold
                .Select(s => s.Stock_Symbol)
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            if (symbols == null || symbols.Count == 0)
                return NotFound("No stock symbols found.");

            return Ok(symbols);
        }

        [HttpGet("Get_Stock_Price/{stockSymbol}")]
        public async Task<IActionResult> GetStockPrice(string stockSymbol)
        {
            var result = await _dioxieReadDbContext.HistoricalPricesStockWithTACompanyInformationGold
                .Where(s => s.Stock_Symbol == stockSymbol)
                .OrderByDescending(s => s.Date)
                .Take(365)
                .ToListAsync();

            if (result == null || result.Count == 0)
                return NotFound($"No data found for stock symbol: {stockSymbol}");

            return Ok(result);
        }

        [HttpGet("Get_News_Sentiment/{number_of_days}")]
        public async Task<IActionResult> GetNewsSentimentByDays(int number_of_days)
        {
            if (number_of_days <= 0)
                return BadRequest("Number of days must be greater than 0.");

            var startDate = DateTime.UtcNow.Date.AddDays(-number_of_days);

            var allData = await _dioxieReadDbContext.HistoricalStockNewsSentimentScoreGold
                .Where(s => !string.IsNullOrEmpty(s.datetime))
                .OrderByDescending(s => s.id)
                .ToListAsync();

            var filteredData = allData
                .Where(s => DateTime.TryParse(s.datetime, out var parsedDate) && parsedDate.Date >= startDate)
                .ToList();

            if (filteredData.Count == 0)
                return NotFound($"No sentiment news found in the last {number_of_days} days.");

            return Ok(filteredData);
        }

    }
}
