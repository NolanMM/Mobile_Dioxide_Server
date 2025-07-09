using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Mobile_Server_Dioxide.Context;
using Mobile_Server_Dioxide.DTOs;
using Mobile_Server_Dioxide.Entities;

namespace Mobile_Server_Dioxide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileDioxieController : ControllerBase
    {
        private readonly ILogger<MobileDioxieController> _logger;
        private DioxieReadDbContext _dioxieReadDbContext;
        private readonly IMemoryCache _cache;

        public MobileDioxieController(ILogger<MobileDioxieController> logger, DioxieReadDbContext dioxieReadDbContext, IMemoryCache cache)
        {
            _cache = cache;
            _logger = logger;
            _dioxieReadDbContext = dioxieReadDbContext;
        }

        [HttpGet("Stock/Symbols/Available")]
        public async Task<IActionResult> GetAvailableStockSymbols()
        {
            try{ 
                var cacheKey = "StockSymbols";

                if (!_cache.TryGetValue(cacheKey, out List<string>? symbols))
                {
                    symbols = await _dioxieReadDbContext.HistoricalPricesStockWithTACompanyInformationGold
                        .Select(s => s.Stock_Symbol)
                        .Where(s => !string.IsNullOrEmpty(s))
                        .Distinct()
                        .OrderBy(s => s)
                        .ToListAsync();

                    if (symbols == null || symbols.Count == 0)
                        return NotFound("No stock symbols found.");

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5));

                    _cache.Set(cacheKey, symbols, cacheEntryOptions);
                }

                _logger.LogInformation("Retrieved {Count} stock symbols from cache.", symbols?.Count);

                return Ok(symbols);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching stock symbols.");
                return StatusCode(500, "Internal server error while fetching stock symbols.");
            }
        }

        [HttpGet("Get_Stock_Price/{stockSymbol}")]
        public async Task<IActionResult> GetStockPrice(string stockSymbol)
        {
            try
            {
                var cacheKey = $"StockPrice_{stockSymbol}";

                if (!_cache.TryGetValue(cacheKey, out List<Historical_Prices_Stock_with_TA_Company_Information_Gold>? result))
                {
                    result = await _dioxieReadDbContext.HistoricalPricesStockWithTACompanyInformationGold
                        .Where(s => s.Stock_Symbol == stockSymbol)
                        .OrderByDescending(s => s.Date)
                        .Take(365)
                        .ToListAsync();

                    if (result == null || result.Count == 0)
                        return NotFound($"No data found for stock symbol: {stockSymbol}");

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                    _cache.Set(cacheKey, result, cacheEntryOptions);
                }

                _logger.LogInformation("Retrieved stock price for symbol: {Symbol} with {Count} records from cache.", stockSymbol, result?.Count);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching stock price for symbol: {Symbol}", stockSymbol);
                return StatusCode(500, "Internal server error while fetching stock price.");
            }
        }

        [HttpGet("Get_News_Sentiment_by_Days/{number_of_days}")]
        public async Task<IActionResult> GetNewsSentimentByDays(int number_of_days)
        {
            try
            {
                if (number_of_days <= 0)
                    return BadRequest("Number of days must be greater than 0.");

                var requestStartDate = DateTime.UtcNow.Date.AddDays(-number_of_days);
                var cacheKey = $"NewsSentimentDays_{number_of_days}";

                // Find any existing broader cache
                List<Historical_Stock_News_Sentiment_Score_Gold>? broaderCachedData = null;
                for (int i = number_of_days; i <= 60; i++) // 60 = upper bound (2 months)
                {
                    if (_cache.TryGetValue($"NewsSentimentDays_{i}", out List<Historical_Stock_News_Sentiment_Score_Gold>? cached))
                    {
                        broaderCachedData = cached
                            .Where(s => DateTime.TryParse(s.datetime, out var parsedDate) && parsedDate.Date >= requestStartDate)
                            .ToList();
                        break;
                    }
                }

                if (broaderCachedData != null && broaderCachedData.Count > 0)
                {
                    return Ok(broaderCachedData);
                }

                // Fallback to DB if no suitable cache found
                var allData = await _dioxieReadDbContext.HistoricalStockNewsSentimentScoreGold
                    .Where(s => !string.IsNullOrEmpty(s.datetime))
                    .OrderByDescending(s => s.id)
                    .ToListAsync();

                var filteredData = allData
                    .Where(s => DateTime.TryParse(s.datetime, out var parsedDate) && parsedDate.Date >= requestStartDate)
                    .ToList();

                if (filteredData.Count == 0)
                    return NotFound($"No sentiment news found in the last {number_of_days} days.");

                // Cache the full filtered result under the requested days
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                _cache.Set(cacheKey, filteredData, cacheOptions);

                _logger.LogInformation("Fetched news sentiment for the last {Days} days with {Count} records.", number_of_days, filteredData.Count);

                return Ok(filteredData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching news sentiment for the last {Days} days.", number_of_days);
                return StatusCode(500, "Internal server error while fetching news sentiment.");
            }
        }


        [HttpGet("Get_News_Sentiment_by_Symbol/{symbol}")]
        public async Task<IActionResult> GetNewsSentiment(string symbol)
        {
            try { 
                if (string.IsNullOrWhiteSpace(symbol))
                    return BadRequest("Symbol is required.");

                var cacheKey = $"NewsSentiment_Symbol_{symbol}";

                if (!_cache.TryGetValue(cacheKey, out List<Historical_Stock_News_Sentiment_Score_Gold>? result))
                {
                    result = await _dioxieReadDbContext.HistoricalStockNewsSentimentScoreGold
                        .Where(s => s.symbol == symbol)
                        .OrderByDescending(s => s.id)
                        .Take(365)
                        .ToListAsync();

                    if (result == null || result.Count == 0)
                        return NotFound($"No sentiment news data found for symbol: {symbol}");

                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                    _cache.Set(cacheKey, result, cacheOptions);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching news sentiment for symbol: {Symbol}", symbol);
                return StatusCode(500, "Internal server error while fetching news sentiment.");
            }
        }


        [HttpGet("Get_All_Users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try { 
                var cacheKey = "AllUsers";

                if (!_cache.TryGetValue(cacheKey, out List<User_DBO>? users))
                {
                    users = await _dioxieReadDbContext.UserDbos
                        .OrderBy(u => u.id)
                        .ToListAsync();

                    if (users == null || users.Count == 0)
                        return NotFound("No users found in the database.");

                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                    _cache.Set(cacheKey, users, cacheOptions);
                }
                _logger.LogInformation("Retrieved {Count} users from cache.", users?.Count);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all users.");
                return StatusCode(500, "Internal server error while fetching users.");
            }
        }

        [HttpPost("Register_User")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto newUser)
        {
            try { 
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check if username already exists
                bool usernameExists = await _dioxieReadDbContext.UserDbos
                    .AnyAsync(u => u.username == newUser.username);

                if (usernameExists)
                    return Conflict(new { message = $"Username '{newUser.username}' is already taken." });

                // Check if email already exists
                bool emailExists = await _dioxieReadDbContext.UserDbos
                    .AnyAsync(u => u.email == newUser.email);

                if (emailExists)
                    return Conflict(new { message = $"Email '{newUser.email}' is already registered." });

                var user = new User_DBO
                {
                    username = newUser.username,
                    password = newUser.password,
                    email = newUser.email,
                    first_name = newUser.first_name,
                    last_name = newUser.last_name,
                    is_superuser = false,
                    is_staff = false,
                    is_active = true,
                    date_joined = DateTimeOffset.UtcNow,
                    last_login = DateTimeOffset.UtcNow
                };

                _dioxieReadDbContext.UserDbos.Add(user);
                await _dioxieReadDbContext.SaveChangesAsync();

                _logger.LogInformation("User registered successfully: {Username}", user.username);

                return Ok(new
                {
                    message = "User registered successfully",
                    user.id,
                    user.username,
                    user.email
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while registering user.");
                return StatusCode(500, "Internal server error while registering user.");
            }
        }

        [HttpPost("Login_User")]
        public async Task<IActionResult> LoginUser([FromBody] LoginDto login)
        {
            try { 
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _dioxieReadDbContext.UserDbos
                    .FirstOrDefaultAsync(u => u.username == login.username && u.password == login.password);

                if (user == null)
                    return Unauthorized(new { message = "Invalid username or password." });

                user.last_login = DateTimeOffset.UtcNow;
                await _dioxieReadDbContext.SaveChangesAsync();

                _logger.LogInformation("User logged in successfully: {Username}", user.username);

                return Ok(new
                {
                    message = "Login successful",
                    user.id,
                    user.username,
                    user.email,
                    user.first_name,
                    user.last_name
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while logging in user: {Username}", login.username);
                return StatusCode(500, "Internal server error while logging in user.");
            }
        }
    }
}
