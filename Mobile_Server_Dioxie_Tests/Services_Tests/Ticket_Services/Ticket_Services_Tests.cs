using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mobile_Server_Dioxide.Services.TickerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Mobile_Server_Dioxie_Tests.Services_Tests.Ticket_Services
{
    [TestClass]
    public class Ticket_Services_Tests
    {
        private const string TestJsonFilePath = "tickers.json";

        [Fact]
        public void Cleanup()
        {
            // Ensure the test file is deleted after each test runs
            if (File.Exists(TestJsonFilePath))
            {
                File.Delete(TestJsonFilePath);
            }
        }

        [Fact]
        [TestCategory("TickerService")]
        public async Task GetTickersAsync_FileExistsAndIsValid_ShouldReturnListOfTickers()
        {
            // Arrange
            var expectedTickers = new List<string> { "AAPL", "GOOG", "MSFT" };
            string jsonContent = JsonSerializer.Serialize(expectedTickers);
            await File.WriteAllTextAsync(TestJsonFilePath, jsonContent);

            var tickerService = new TickerService();

            // Act
            var result = await tickerService.GetTickersAsync();

            // Assert
            Assert.IsNotNull(result, "The result should not be null when the file is valid.");
            CollectionAssert.AreEqual(expectedTickers, result, "The returned list of tickers should match the expected list.");
        }

        [Fact]
        [TestCategory("TickerService")]
        public async Task GetTickersAsync_FileDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            if (File.Exists(TestJsonFilePath))
            {
                File.Delete(TestJsonFilePath);
            }
            var tickerService = new TickerService();

            // Act
            var result = await tickerService.GetTickersAsync();

            // Assert
            Assert.IsNull(result, "The result should be null when the ticker file does not exist.");
        }

        [Fact]
        [TestCategory("TickerService")]
        public async Task GetTickersAsync_FileContainsInvalidJson_ShouldThrowJsonException()
        {
            // Arrange
            string invalidJsonContent = "[\"AAPL\", \"GOOG\",";
            await File.WriteAllTextAsync(TestJsonFilePath, invalidJsonContent);

            var tickerService = new TickerService();

            // Act & Assert
            await Assert.ThrowsExceptionAsync<JsonException>(async () => await tickerService.GetTickersAsync());
        }

        [Fact]
        [TestCategory("TickerService")]
        public async Task GetTickersAsync_FileIsEmpty_ShouldThrowJsonException()
        {
            // Arrange
            // Create an empty file
            await File.WriteAllTextAsync(TestJsonFilePath, string.Empty);

            var tickerService = new TickerService();

            // Act & Assert
            // Deserializing an empty string is not valid for a list
            await Assert.ThrowsExceptionAsync<JsonException>(async () => await tickerService.GetTickersAsync());
        }
    }
}
