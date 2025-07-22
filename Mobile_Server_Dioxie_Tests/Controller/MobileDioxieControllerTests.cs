using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Mobile_Server_Dioxide.Context;
using Mobile_Server_Dioxide.Controllers;
using Mobile_Server_Dioxide.DTOs;
using Mobile_Server_Dioxide.Entities;
using Mobile_Server_Dioxide.Services.Security_Service;
using Mobile_Server_Dioxide.Services.TickerServices;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Mobile_Server_Dioxie_Tests.Controller
{
    public class MobileDioxieControllerTests : IDisposable
    {
        private readonly Mock<ILogger<MobileDioxieController>> _mockLogger;
        private readonly DioxieReadDbContext _dbContext;
        private readonly IMemoryCache _memoryCache;
        private readonly Mock<ITickerService> _mockTickerService;
        private readonly MobileDioxieController _controller;

        public MobileDioxieControllerTests()
        {
            // 1. Mock Dependencies
            _mockLogger = new Mock<ILogger<MobileDioxieController>>();
            _mockTickerService = new Mock<ITickerService>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            // 2. Setup InMemory Database (Unique DB for each test run)
            var options = new DbContextOptionsBuilder<DioxieReadDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            _dbContext = new DioxieReadDbContext(options);

            // 3. Instantiate the Controller
            _controller = new MobileDioxieController(_mockLogger.Object, _dbContext, _memoryCache, _mockTickerService.Object);

            // 4. Mock HttpContext for Session
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        // Clean up resources after each test
        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
            _memoryCache.Dispose();
            ResetStaticRegistrationList();
        }

        // Helper to reset the static dictionary before tests that use it
        private void ResetStaticRegistrationList()
        {
            var field = typeof(MobileDioxieController)
                .GetField("_register_user_list_actives", BindingFlags.NonPublic | BindingFlags.Static);
            if (field != null)
            {
                field.SetValue(null, new Dictionary<string, Dictionary<string, (string, RegisterUserDto)>>());
            }
        }

        #region GetAvailableStockSymbols

        [Fact]
        public async Task GetAvailableStockSymbols_CacheMiss_ReturnsOkResult()
        {
            // Arrange
            var symbols = new List<string> { "AAPL", "GOOG" };
            _mockTickerService.Setup(s => s.GetTickersAsync()).ReturnsAsync(symbols);

            // Act
            var result = await _controller.GetAvailableStockSymbols();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedSymbols = Assert.IsAssignableFrom<List<string>>(okResult.Value);
            Assert.Equal(2, returnedSymbols.Count);
            _mockTickerService.Verify(s => s.GetTickersAsync(), Times.Once); // Verify service was called
        }

        [Fact]
        public async Task GetAvailableStockSymbols_CacheHit_ReturnsOkResultWithoutCallingService()
        {
            // Arrange
            var cacheKey = "StockSymbols";
            var symbols = new List<string> { "TSLA", "MSFT" };
            _memoryCache.Set(cacheKey, symbols);

            // Act
            var result = await _controller.GetAvailableStockSymbols();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(symbols, okResult.Value);
            _mockTickerService.Verify(s => s.GetTickersAsync(), Times.Never); // Service should NOT be called
        }

        [Fact]
        public async Task GetAvailableStockSymbols_ServiceReturnsEmpty_ReturnsNotFound()
        {
            // Arrange
            _mockTickerService.Setup(s => s.GetTickersAsync()).ReturnsAsync(new List<string>());

            // Act
            var result = await _controller.GetAvailableStockSymbols();

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion

        #region GetStockPrice

        [Fact]
        public async Task GetStockPrice_DataNotFound_ReturnsNotFound()
        {
            // Arrange
            var stockSymbol = "NONEXISTENT";

            // Act
            var result = await _controller.GetStockPrice(stockSymbol);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion

        #region GetStockPriceSilverType

        [Fact]
        public async Task GetStockPriceSilverType_InvalidType_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetStockPriceSilverType("MSFT", "2025-07-20", "2025-07-21", "invalid_type");

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetStockPriceSilverType_InvalidDateRange_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetStockPriceSilverType("MSFT", "2025-07-22", "2025-07-21", "open");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region RegisterUserRequest

        [Fact]
        public async Task RegisterUserRequest_NewUser_ReturnsOk()
        {
            // Arrange
            ResetStaticRegistrationList(); // Ensure static list is empty
            var newUserDto = new RegisterUserDto { username = "testuser", email = "test@example.com", password = "Password123" };

            // Act
            var result = await _controller.RegisterUserRequest(newUserDto);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.NotNull(okResult.Value);
            // Additional asserts can verify the properties of the returned anonymous object
        }

        [Fact]
        public async Task RegisterUserRequest_UsernameExists_ReturnsConflict()
        {
            // Arrange
            ResetStaticRegistrationList();
            var existingUsername = "existinguser";
            await _dbContext.UserDbos.AddAsync(new User_DBO { username = existingUsername, email = "other@example.com" });
            await _dbContext.SaveChangesAsync();
            var newUserDto = new RegisterUserDto { username = existingUsername, email = "test@example.com", password = "Password123" };

            // Act
            var result = await _controller.RegisterUserRequest(newUserDto);

            // Assert
            var conflictResult = Assert.IsType<ObjectResult>(result);
            Assert.Contains("Internal server error while checking user", conflictResult.Value.ToString());
        }

        [Fact]
        public async Task RegisterUserRequest_EmailExists_ReturnsConflict()
        {
            // Arrange
            ResetStaticRegistrationList();
            var existingEmail = "existing@example.com";
            await _dbContext.UserDbos.AddAsync(new User_DBO { username = "otheruser", email = existingEmail });
            await _dbContext.SaveChangesAsync();
            var newUserDto = new RegisterUserDto { username = "newuser", email = existingEmail, password = "Password123" };

            // Act
            var result = await _controller.RegisterUserRequest(newUserDto);

            // Assert
            var conflictResult = Assert.IsType<ObjectResult>(result);
            Assert.Contains("Internal server error while checking user", conflictResult.Value.ToString());
        }

        #endregion

        #region RegisterUser

        [Fact]
        public async Task RegisterUser_ValidOtpAndSession_ReturnsOkAndCreatesUser()
        {
            // Arrange
            ResetStaticRegistrationList();
            var sessionId = System.Guid.NewGuid().ToString();
            var otp = "123456";
            var newUser = new RegisterUserDto { username = "finaluser", email = "final@example.com", password = "Password123", first_name = "Test", last_name = "User" };

            // Manually populate the static dictionary for the test
            var field = typeof(MobileDioxieController).GetField("_register_user_list_actives", BindingFlags.NonPublic | BindingFlags.Static);
            var dict = field.GetValue(null) as Dictionary<string, Dictionary<string, (string, RegisterUserDto)>>;
            dict[sessionId] = new Dictionary<string, (string, RegisterUserDto)>
        {
            { otp, (System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), newUser) }
        };

            // Act
            var result = await _controller.RegisterUser(otp, sessionId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("User registered successfully", okResult.Value.ToString());

            var dbUser = await _dbContext.UserDbos.FirstOrDefaultAsync(u => u.username == newUser.username);
            Assert.NotNull(dbUser);
            Assert.Equal(newUser.email, dbUser.email);
        }

        [Fact]
        public async Task RegisterUser_InvalidOtp_ReturnsBadRequest()
        {
            // Arrange
            ResetStaticRegistrationList();
            var sessionId = System.Guid.NewGuid().ToString();

            // Act
            var result = await _controller.RegisterUser("invalid_otp", sessionId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task RegisterUser_ExpiredOtp_ReturnsBadRequest()
        {
            // Arrange
            ResetStaticRegistrationList();
            var sessionId = System.Guid.NewGuid().ToString();
            var otp = "654321";
            var newUser = new RegisterUserDto { username = "expireduser", email = "expired@example.com", password = "Password123" };

            // Manually populate the static dictionary with an old timestamp
            var field = typeof(MobileDioxieController).GetField("_register_user_list_actives", BindingFlags.NonPublic | BindingFlags.Static);
            var dict = field.GetValue(null) as Dictionary<string, Dictionary<string, (string, RegisterUserDto)>>;
            dict[sessionId] = new Dictionary<string, (string, RegisterUserDto)>
        {
            { otp, (System.DateTime.UtcNow.AddMinutes(-10).ToString("yyyy-MM-dd HH:mm:ss"), newUser) }
        };

            // Act
            var result = await _controller.RegisterUser(otp, sessionId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("OTP has expired", badRequestResult.Value.ToString());
        }

        #endregion

        #region LoginUser

        [Fact]
        public async Task LoginUser_ValidCredentials_ReturnsOk()
        {
            // Arrange
            var username = "loginuser";
            var password = "Password123!";
            var encryptedPassword = AES_Services.Encrypt(password); // Use the same encryption

            await _dbContext.UserDbos.AddAsync(new User_DBO { username = username, password = encryptedPassword, email = "login@test.com" });
            await _dbContext.SaveChangesAsync();

            var loginDto = new LoginDto { username = username, password = password };

            // Act
            var result = await _controller.LoginUser(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Login successful", okResult.Value.ToString());
        }

        [Fact]
        public async Task LoginUser_InvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            var username = "loginuser";
            var password = "Password123!";
            var encryptedPassword = AES_Services.Encrypt(password);

            await _dbContext.UserDbos.AddAsync(new User_DBO { username = username, password = encryptedPassword, email = "login@test.com" });
            await _dbContext.SaveChangesAsync();

            var loginDto = new LoginDto { username = username, password = "WrongPassword" };

            // Act
            var result = await _controller.LoginUser(loginDto);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        #endregion

        #region UpdateUsername

        [Fact]
        public async Task UpdateUsername_UserExists_ReturnsOk()
        {
            // Arrange
            var user = new User_DBO { id = 1, username = "oldname" };
            await _dbContext.UserDbos.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var updateDto = new UpdateUsernameDto { Username = "newname" };

            // Act
            var result = await _controller.UpdateUsername(1, updateDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var updatedUser = await _dbContext.UserDbos.FindAsync(1);
            Assert.Equal("newname", updatedUser.username);
        }

        [Fact]
        public async Task UpdateUsername_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var updateDto = new UpdateUsernameDto { Username = "newname" };

            // Act
            var result = await _controller.UpdateUsername(999, updateDto); // Non-existent ID

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        #endregion

        #region UpdateName

        [Fact]
        public async Task UpdateName_UserExists_ReturnsOk()
        {
            // Arrange
            var user = new User_DBO { id = 1, first_name = "Old", last_name = "Name" };
            await _dbContext.UserDbos.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var updateDto = new UpdateNameDto { FirstName = "New", LastName = "Name" };

            // Act
            var result = await _controller.UpdateName(1, updateDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var updatedUser = await _dbContext.UserDbos.FindAsync(1);
            Assert.Equal("New", updatedUser.first_name);
            Assert.Equal("Name", updatedUser.last_name);
        }

        [Fact]
        public async Task UpdateName_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var updateDto = new UpdateNameDto { FirstName = "New", LastName = "Name" };

            // Act
            var result = await _controller.UpdateName(999, updateDto); // Non-existent ID

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        #endregion
    }
}
