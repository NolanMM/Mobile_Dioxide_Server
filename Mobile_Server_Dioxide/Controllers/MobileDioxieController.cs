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
    }
}
