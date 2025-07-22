using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Mobile_Server_Dioxide.Context;
using Mobile_Server_Dioxide.DTOs;
using Mobile_Server_Dioxide.Entities;
using Mobile_Server_Dioxide.Services.Security_Service;
using Mobile_Server_Dioxide.Services.OTP_Module_Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Globalization;
using Mobile_Server_Dioxide.Services.TickerServices;

namespace Mobile_Server_Dioxide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileDioxieController : ControllerBase
    {
        private readonly ILogger<MobileDioxieController> _logger;
        private DioxieReadDbContext _dioxieReadDbContext;
        private readonly IMemoryCache _cache;
        private readonly ITickerService _tickerService;

        private static Dictionary<string, Dictionary<string, (string, RegisterUserDto)>> _register_user_list_actives = new Dictionary<string, Dictionary<string, (string, RegisterUserDto)>>();

        public MobileDioxieController(ILogger<MobileDioxieController> logger, DioxieReadDbContext dioxieReadDbContext, IMemoryCache cache, ITickerService tickerService)
        {
            _cache = cache;
            _logger = logger;
            _dioxieReadDbContext = dioxieReadDbContext;
            _tickerService = tickerService;
        }

        [HttpGet("Stock/Symbols/Available")]
        public async Task<IActionResult> GetAvailableStockSymbols()
        {
            try{ 
                var cacheKey = "StockSymbols";

                if (!_cache.TryGetValue(cacheKey, out List<string>? symbols))
                {
                    symbols = await _tickerService.GetTickersAsync();

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

        [HttpGet("Get_Stock_Price_Silver/{stockSymbol}/{start_time}/{end_time}/{type}")]
        public async Task<IActionResult> GetStockPriceSilverType(string stockSymbol, string start_time, string end_time, string type)
        {
            try
            {
                if (!DateTime.TryParse(start_time, out var startDate) || !DateTime.TryParse(end_time, out var endDate))
                    return BadRequest("Invalid date format. Use yyyy-MM-dd.");

                if (startDate > endDate)
                    return BadRequest("Start date must be earlier than or equal to end date.");

                var cacheKey = $"StockPrice_Silver_{stockSymbol}";

                // Check if 365-day cached
                if (!_cache.TryGetValue(cacheKey, out List<Historical_Prices_Stock_Silver>? cachedData))
                {
                    cachedData = await _dioxieReadDbContext.HistoricalPricesStockSilver.Where(s => s.Stock_Symbol == stockSymbol).ToListAsync();

                    if (cachedData == null || cachedData.Count == 0)
                        return NotFound($"No stock price data found for symbol: {stockSymbol}");

                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                    _cache.Set(cacheKey, cachedData, cacheOptions);
                }

                var filteredData = cachedData.Where(s => s.Date != null &&
                                                DateTime.TryParseExact(s.Date, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate) &&
                                                parsedDate.Date >= startDate.Date && parsedDate.Date <= endDate.Date)
                                            .OrderBy(s => DateTime.ParseExact(s.Date!, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture))
                                            .ToList();

                if (filteredData.Count == 0)
                    return NotFound($"No stock price data found for {stockSymbol} between {startDate:yyyy-MM-dd} and {endDate:yyyy-MM-dd}.");

                if (type.ToLower() != "open" && type.ToLower() != "high" && type.ToLower() != "low" && type.ToLower() != "close" && type.ToLower() != "volume" && type.ToLower() != "dividends" && type.ToLower() != "stock_splits")
                    return BadRequest("Invalid type specified. Valid types are: Open, High, Low, Close, Volume, Dividends, Stock_Splits.");

                List<Historical_Prices_By_Type_Dto> returnedData = new List<Historical_Prices_By_Type_Dto>();

                if (type.ToLower() == "open")
                    returnedData = filteredData.Select(s => new Historical_Prices_By_Type_Dto
                    {
                        Date = s.Date,
                        Price = s.Open,
                        Stock_Symbol = s.Stock_Symbol,
                        Type = type.ToLower()
                    }).ToList();
                else if (type.ToLower() == "high")
                    returnedData = filteredData.Select(s => new Historical_Prices_By_Type_Dto
                    {
                        Date = s.Date,
                        Price = s.High,
                        Stock_Symbol = s.Stock_Symbol,
                        Type = type.ToLower()
                    }).ToList();
                else if (type.ToLower() == "low")
                    returnedData = filteredData.Select(s => new Historical_Prices_By_Type_Dto
                    {
                        Date = s.Date,
                        Price = s.Low,
                        Stock_Symbol = s.Stock_Symbol,
                        Type = type.ToLower()
                    }).ToList();
                else if (type.ToLower() == "close")
                    returnedData = filteredData.Select(s => new Historical_Prices_By_Type_Dto
                    {
                        Date = s.Date,
                        Price = s.Close,
                        Stock_Symbol = s.Stock_Symbol,
                        Type = type.ToLower()
                    }).ToList();
                else if (type.ToLower() == "volume")
                    returnedData = filteredData.Select(s => new Historical_Prices_By_Type_Dto
                    {
                        Date = s.Date,
                        Price = s.Volume,
                        Stock_Symbol = s.Stock_Symbol,
                        Type = type.ToLower()
                    }).ToList();
                else if (type.ToLower() == "dividends")
                    returnedData = filteredData.Select(s => new Historical_Prices_By_Type_Dto
                    {
                        Date = s.Date,
                        Price = s.Dividends,
                        Stock_Symbol = s.Stock_Symbol,
                        Type = type.ToLower()
                    }).ToList();
                else if (type.ToLower() == "stock_splits")
                    returnedData = filteredData.Select(s => new Historical_Prices_By_Type_Dto
                    {
                        Date = s.Date,
                        Price = s.Stock_Splits,
                        Stock_Symbol = s.Stock_Symbol,
                        Type = type.ToLower()
                    }).ToList();

                _logger.LogInformation("Retrieved stock price for {Symbol} from {Start} to {End} with {Count} records.", stockSymbol, startDate, endDate, filteredData.Count);

                return Ok(returnedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching stock price for symbol {Symbol} in date range {Start} to {End}", stockSymbol, start_time, end_time);
                return StatusCode(500, "Internal server error while fetching stock price.");
            }
        }

        [HttpGet("Get_Stock_Price_Silver/{stockSymbol}/{start_time}/{end_time}")]
        public async Task<IActionResult> GetStockPriceSilver(string stockSymbol, string start_time, string end_time)
        {
            try
            {
                if (!DateTime.TryParse(start_time, out var startDate) || !DateTime.TryParse(end_time, out var endDate))
                    return BadRequest("Invalid date format. Use yyyy-MM-dd.");

                if (startDate > endDate)
                    return BadRequest("Start date must be earlier than or equal to end date.");

                var cacheKey = $"StockPrice_Silver_{stockSymbol}";

                // Check if 365-day cached
                if (!_cache.TryGetValue(cacheKey, out List<Historical_Prices_Stock_Silver>? cachedData))
                {
                    cachedData = await _dioxieReadDbContext.HistoricalPricesStockSilver.Where(s => s.Stock_Symbol == stockSymbol).ToListAsync();

                    if (cachedData == null || cachedData.Count == 0)
                        return NotFound($"No stock price data found for symbol: {stockSymbol}");

                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                    _cache.Set(cacheKey, cachedData, cacheOptions);
                }

                var filteredData = cachedData.Where(s => s.Date != null &&
                                                DateTime.TryParseExact(s.Date, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate) &&
                                                parsedDate.Date >= startDate.Date && parsedDate.Date <= endDate.Date)
                                            .OrderBy(s => DateTime.ParseExact(s.Date!, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture))
                                            .ToList();

                if (filteredData.Count == 0)
                    return NotFound($"No stock price data found for {stockSymbol} between {startDate:yyyy-MM-dd} and {endDate:yyyy-MM-dd}.");

                _logger.LogInformation("Retrieved stock price for {Symbol} from {Start} to {End} with {Count} records.", stockSymbol, startDate, endDate, filteredData.Count);

                return Ok(filteredData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching stock price for symbol {Symbol} in date range {Start} to {End}", stockSymbol, start_time, end_time);
                return StatusCode(500, "Internal server error while fetching stock price.");
            }
        }

        [HttpGet("Get_Stock_Price_Silver/{stockSymbol}")]
        public async Task<IActionResult> GetStockPriceBronzeInRange(string stockSymbol)
        {
            try
            {
                var cacheKey = $"StockPrice_Silver_{stockSymbol}";

                // Check if 365-day cached
                if (!_cache.TryGetValue(cacheKey, out List<Historical_Prices_Stock_Silver>? cachedData))
                {
                    cachedData = await _dioxieReadDbContext.HistoricalPricesStockSilver.Where(s => s.Stock_Symbol == stockSymbol).ToListAsync();

                    if (cachedData == null || cachedData.Count == 0)
                        return NotFound($"No stock price data found for symbol: {stockSymbol}");

                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                    _cache.Set(cacheKey, cachedData, cacheOptions);
                }

                _logger.LogInformation("Retrieved stock price for symbol: {Symbol} with {Count} records from cache.", stockSymbol, cachedData?.Count);
                if (cachedData?.Count == 0)
                    return NotFound($"No stock price data found for symbol: {stockSymbol}");

                return Ok(cachedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching stock price for symbol: {Symbol}", stockSymbol);
                return StatusCode(500, "Internal server error while fetching stock price.");
            }
        }

        [HttpGet("Get_Stock_Price/{stockSymbol}/{start_time}/{end_time}")]
        public async Task<IActionResult> GetStockPriceInRange(string stockSymbol, string start_time, string end_time)
        {
            try
            {
                if (!DateTime.TryParse(start_time, out var startDate) || !DateTime.TryParse(end_time, out var endDate))
                    return BadRequest("Invalid date format. Use yyyy-MM-dd.");

                if (startDate > endDate)
                    return BadRequest("Start date must be earlier than or equal to end date.");

                var cacheKey = $"StockPrice_Silver_{stockSymbol}";

                // Check if 365-day cached
                if (!_cache.TryGetValue(cacheKey, out List<Historical_Prices_Stock_with_TA_Company_Information_Gold>? cachedData))
                {
                    cachedData = await _dioxieReadDbContext.HistoricalPricesStockWithTACompanyInformationGold
                        .Where(s => s.Stock_Symbol == stockSymbol)
                        .OrderByDescending(s => s.Date)
                        .ToListAsync();

                    if (cachedData == null || cachedData.Count == 0)
                        return NotFound($"No stock price data found for symbol: {stockSymbol}");

                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                    _cache.Set(cacheKey, cachedData, cacheOptions);
                }

                var filteredData = cachedData
                    .Where(s => s.Date.HasValue && s.Date.Value.Date >= startDate.Date && s.Date.Value.Date <= endDate.Date)
                    .OrderBy(s => s.Date)
                    .ToList();

                if (filteredData.Count == 0)
                    return NotFound($"No stock price data found for {stockSymbol} between {startDate:yyyy-MM-dd} and {endDate:yyyy-MM-dd}.");

                _logger.LogInformation("Retrieved stock price for {Symbol} from {Start} to {End} with {Count} records.", stockSymbol, startDate, endDate, filteredData.Count);

                return Ok(filteredData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching stock price for symbol {Symbol} in date range {Start} to {End}", stockSymbol, start_time, end_time);
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

        [HttpPost("Register_User/Request")]
        public async Task<IActionResult> RegisterUserRequest([FromBody] RegisterUserDto newUser)
        {
            try
            {
                string? sessionId = HttpContext.Session.GetString("SessionId");
                if (HttpContext.Session.GetString("SessionId") == null)
                {
                    sessionId = Guid.NewGuid().ToString();
                    HttpContext.Session.SetString("SessionId", sessionId);
               
                    string currentDateTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

                    int randomNumber = new Random().Next(100000000, 999999999);

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

                    _register_user_list_actives.Add(
                        sessionId,
                        new Dictionary<string, (string, RegisterUserDto)>
                        {
                            { randomNumber.ToString(), (currentDateTime, newUser) }
                        }
                    );

                    await Verify_Email_Services.Send_OTP_CodeAsync(randomNumber.ToString(), sessionId, newUser.email, newUser.username);

                    return Ok(new
                    {
                        message = "User registration request received. Please check your email for the OTP code.",
                        sessionId,
                        username = newUser.username,
                        email = newUser.email,
                        otpCode = randomNumber, 
                        timestamp = currentDateTime
                    });
                }
                else
                {
                    _logger.LogWarning("Session ID already exists: {SessionId}", sessionId);
                    return BadRequest("Session ID already exists. Please wait after next sessions.");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while checking user registration request.");
                return StatusCode(500, "Internal server error while checking user registration request.");
            }
        }

        [HttpGet("Register_User/{OTP_Number}/{SessionID}")]
        public async Task<IActionResult> RegisterUser(string OTP_Number, string SessionID)
        {
            try {
                Console.WriteLine(OTP_Number);
                Console.WriteLine(SessionID);
                if (string.IsNullOrWhiteSpace(OTP_Number) || string.IsNullOrWhiteSpace(SessionID))
                    return BadRequest("OTP Number and Session ID are required.");

                if (!_register_user_list_actives.TryGetValue(SessionID, out var otpEntries) || !otpEntries.TryGetValue(OTP_Number, out var entry))
                {
                    _logger.LogWarning("Invalid OTP or Session ID: {SessionID}, {OTP_Number}", SessionID, OTP_Number);
                    return BadRequest("Invalid OTP Number or Session ID.");
                }
                string requestDateTime = entry.Item1;

                if (!DateTime.TryParse(requestDateTime, out var requestDateTimeParsed))
                {
                    _logger.LogWarning("Invalid request date time format: {RequestDateTime}", requestDateTime);
                    return BadRequest("Invalid request date time format.");
                }
                if ((DateTime.UtcNow - requestDateTimeParsed).TotalMinutes > 5)
                {
                    _logger.LogWarning("OTP expired for Session ID: {SessionID}, OTP Number: {OTP_Number}", SessionID, OTP_Number);
                    _register_user_list_actives[SessionID].Remove(OTP_Number);
                    return BadRequest("OTP has expired. Please request a new one.");
                }

                RegisterUserDto newUser = entry.Item2;

                string encryptedPassword = AES_Services.Encrypt(newUser.password);

                var user = new User_DBO
                {
                    username = newUser.username,
                    password = encryptedPassword,
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
                _register_user_list_actives[SessionID].Remove(OTP_Number);

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

                string encryptedPassword = AES_Services.Encrypt(login.password);

                var user = await _dioxieReadDbContext.UserDbos.FirstOrDefaultAsync(u => u.username == login.username && u.password == encryptedPassword);

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

        [HttpGet("Get_Macro_Historical_by_Days/{number_of_days}")]
        public async Task<IActionResult> GetMacroHistoricalByDays(int number_of_days)
        {
            try
            {
                if (number_of_days <= 0)
                    return BadRequest("Number of days must be greater than 0.");

                var requestStartDate = DateTime.UtcNow.Date.AddDays(-number_of_days);
                var cacheKey = $"MacroHistoricalDays_{number_of_days}";

                List<Macro_Historical_Silver>? broaderCachedData = null;
                for (int i = number_of_days; i <= 60; i++) // 60 = max upper bound
                {
                    if (_cache.TryGetValue($"MacroHistoricalDays_{i}", out List<Macro_Historical_Silver>? cached))
                    {
                        broaderCachedData = cached
                            .Where(s => s.date.Date >= requestStartDate)
                            .ToList();
                        break;
                    }
                }

                if (broaderCachedData != null && broaderCachedData.Count > 0)
                {
                    return Ok(broaderCachedData);
                }

                // Fallback: Load from DB
                var allData = await _dioxieReadDbContext.MacroHistoricalSilver
                    .Where(s => s.date >= requestStartDate)
                    .OrderByDescending(s => s.date)
                    .ToListAsync();

                if (allData == null || allData.Count == 0)
                    return NotFound($"No macroeconomic data found in the last {number_of_days} days.");

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                _cache.Set(cacheKey, allData, cacheOptions);

                _logger.LogInformation("Fetched macroeconomic data for the last {Days} days with {Count} records.", number_of_days, allData.Count);

                return Ok(allData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching macroeconomic data for the last {Days} days.", number_of_days);
                return StatusCode(500, "Internal server error while fetching macroeconomic data.");
            }
        }

        [HttpPost("user/{id}/username")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUsername(int id, [FromBody] UpdateUsernameDto updateUsernameDto)
        {
            var user = await _dioxieReadDbContext.UserDbos.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.username = updateUsernameDto.Username;

            try
            {
                await _dioxieReadDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(new { message = "Username updated successfully." });
        }

        [HttpPost("user/{id}/name")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateName(int id, [FromBody] UpdateNameDto updateNameDto)
        {
            var user = await _dioxieReadDbContext.UserDbos.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.first_name = updateNameDto.FirstName;
            user.last_name = updateNameDto.LastName;

            try
            {
                await _dioxieReadDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(new { message = "User name updated successfully." });
        }
    }
}
