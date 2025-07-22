using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mobile_Server_Dioxide.Services.OTP_Module_Services;
using System.Text.RegularExpressions;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using System.Reflection;

namespace Mobile_Server_Dioxie_Tests.Services_Tests.OTP_Module_Services
{
    [TestClass]
    public class OTP_Module_Services_Tests
    {
        private const string AllowedCharactersPattern = "^[a-zA-Z0-9]+$";

        [Fact]
        [TestCategory("GenerateRandomKey")]
        public void GRK01_GenerateStandardLengthKey_ShouldReturnCorrectLengthString()
        {
            // Arrange
            int length = 6;

            // Act
            string key = Verify_Email_Services.GenerateRandomKey(length);

            // Assert
            Assert.IsNotNull(key, "The generated key should not be null.");
            Assert.AreEqual(length, key.Length, "The key length should match the requested length.");
            Assert.IsTrue(Regex.IsMatch(key, AllowedCharactersPattern), "The key should only contain alphanumeric characters.");
        }

        [Fact]
        [TestCategory("GenerateRandomKey")]
        public void GRK02_GenerateZeroLengthKey_ShouldReturnEmptyString()
        {
            // Arrange
            int length = 0;

            // Act
            string key = Verify_Email_Services.GenerateRandomKey(length);

            // Assert
            Assert.IsNotNull(key, "The generated key should not be null.");
            Assert.AreEqual(string.Empty, key, "A key with zero length should be an empty string.");
        }

        [Fact]
        [TestCategory("GenerateRandomKey")]
        public void GRK03_GenerateLongKey_ShouldReturnCorrectLengthString()
        {
            // Arrange
            int length = 128;

            // Act
            string key = Verify_Email_Services.GenerateRandomKey(length);

            // Assert
            Assert.IsNotNull(key, "The generated key should not be null.");
            Assert.AreEqual(length, key.Length, "The key length should match the requested length.");
        }

        [Fact]
        [TestCategory("GenerateRandomKey")]
        public void GRK04_MultipleCalls_ShouldProduceDifferentKeys()
        {
            // Arrange
            int length = 8;

            // Act
            string key1 = Verify_Email_Services.GenerateRandomKey(length);
            string key2 = Verify_Email_Services.GenerateRandomKey(length);

            // Assert
            Assert.AreNotEqual(key1, key2, "Multiple calls to GenerateRandomKey should produce different results.");
        }

        [Fact]
        [TestCategory("CreateEmailBody")]
        public void CEB01_StandardInputs_ShouldGenerateCorrectHtml()
        {
            // Arrange
            // Use Reflection to test the private static method 'CreateEmailBody'
            var methodInfo = typeof(Verify_Email_Services).GetMethod("CreateEmailBody", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(methodInfo, "The private method 'CreateEmailBody' was not found.");

            string otpCode = "123456";
            string endpointLink = "http://valid.url/verify";
            string username = "JohnDoe";
            object[] parameters = { otpCode, endpointLink, username };

            // Act
            string emailBody = (string)methodInfo.Invoke(null, parameters);

            // Assert
            Assert.IsTrue(emailBody.Contains(otpCode), "Email body should contain the OTP code.");
            Assert.IsTrue(emailBody.Contains($"Hello {username},"), "Email body should contain the username.");
            Assert.IsTrue(emailBody.Contains($"href='{endpointLink}'"), "Email body should contain the correct verification link.");
        }

        [Fact]
        [TestCategory("CreateEmailBody")]
        public void CEB02_EmptyStringInputs_ShouldGenerateValidHtml()
        {
            // Arrange
            var methodInfo = typeof(Verify_Email_Services).GetMethod("CreateEmailBody", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(methodInfo, "The private method 'CreateEmailBody' was not found.");

            string otpCode = "";
            string endpointLink = "";
            string username = "";
            object[] parameters = { otpCode, endpointLink, username };

            // Act
            string emailBody = (string)methodInfo.Invoke(null, parameters);

            // Assert
            Assert.IsTrue(emailBody.Contains("Hello ,"), "Email body should handle an empty username.");
            Assert.IsTrue(emailBody.Contains("href=''"), "Email body should handle an empty endpoint link.");
            Assert.IsFalse(string.IsNullOrEmpty(emailBody), "Email body should not be null or empty.");
        }

        [Fact]
        [TestCategory("CreateEmailBody")]
        public void CEB03_SpecialCharacterInputs_ShouldPlaceCharactersCorrectly()
        {
            // Arrange
            var methodInfo = typeof(Verify_Email_Services).GetMethod("CreateEmailBody", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(methodInfo, "The private method 'CreateEmailBody' was not found.");

            string otpCode = "A&B<C";
            string endpointLink = "http://a.com?q=x&y=z";
            string username = "User<Name>";
            object[] parameters = { otpCode, endpointLink, username };

            // Act
            string emailBody = (string)methodInfo.Invoke(null, parameters);

            // Assert
            Assert.IsTrue(emailBody.Contains(otpCode), "Email body should contain the OTP with special characters.");
            Assert.IsTrue(emailBody.Contains($"Hello {username},"), "Email body should contain the username with special characters.");
            Assert.IsTrue(emailBody.Contains($"href='{endpointLink}'"), "Email body should contain the link with special characters.");
        }
    }
}
