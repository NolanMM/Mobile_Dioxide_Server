using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mobile_Server_Dioxide.Services.Security_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Mobile_Server_Dioxie_Tests.Services_Tests.Security_Services
{
    [TestClass]
    public class AES_Services_Test
    {
        private const string TestKey = "ByNolanM";
        [Fact]
        [TestCategory("Roundtrip")]
        public void EncryptThenDecrypt_ShouldReturnOriginalString()
        {
            // Arrange
            string originalText = "This is a roundtrip test to ensure consistency.";

            // Act
            string encrypted = AES_Services.Encrypt(originalText);
            string decrypted = AES_Services.Decrypt(encrypted, TestKey);

            // Assert
            Assert.AreEqual(originalText, decrypted, "The final decrypted string must match the original string.");
        }

        [Fact]
        [TestCategory("Encrypt")]
        public void Encrypt_StandardString_ShouldReturnBase64String()
        {
            // Arrange
            string plainText = "This is a test string.";

            // Act
            string encryptedText = AES_Services.Encrypt(plainText);

            // Assert
            Assert.IsNotNull(encryptedText, "Encrypted string should not be null.");
            Assert.AreNotEqual(plainText, encryptedText, "Encrypted string should not be the same as the plain text.");
            // A simple check to see if the output is likely Base64
            Assert.IsTrue(Convert.TryFromBase64String(encryptedText, new Span<byte>(new byte[encryptedText.Length]), out _), "Encrypted string should be a valid Base64 string.");
        }

        [Fact]
        [TestCategory("Encrypt")]
        public void Encrypt_EmptyString_ShouldReturnValidEncryptedString()
        {
            // Arrange
            string plainText = "";

            // Act
            string encryptedText = AES_Services.Encrypt(plainText);

            // Assert
            Assert.IsNotNull(encryptedText);
            Assert.AreNotEqual(string.Empty, encryptedText, "Encrypting an empty string should not result in an empty string.");
        }

        [Fact]
        [TestCategory("Encrypt")]
        public void Encrypt_StringWithSpecialCharacters_ShouldEncryptCorrectly()
        {
            // Arrange
            string plainText = "!@#$%^&*()_+`~-=[]{}|;':,./<>?";

            // Act
            string encryptedText = AES_Services.Encrypt(plainText);
            string decryptedText = AES_Services.Decrypt(encryptedText, TestKey);

            // Assert
            Assert.AreEqual(plainText, decryptedText, "The decrypted text should match the original string with special characters.");
        }

        [Fact]
        [TestCategory("Decrypt")]
        public void Decrypt_ValidStringAndKey_ShouldReturnOriginalText()
        {
            string originalText = "Hello, World!";
            string encryptedText = AES_Services.Encrypt(originalText);

            string decryptedText = AES_Services.Decrypt(encryptedText, TestKey);

            Assert.AreEqual(originalText, decryptedText, "Decrypted text should match the original text.");
        }

        [Fact]
        [TestCategory("Decrypt")]
        [ExpectedException(typeof(FormatException))]
        public void Decrypt_MalformedBase64String_ShouldThrowFormatException()
        {
            string malformedText = "This is not a valid base64 string";

            var ex = Assert.ThrowsException<Exception>(() => AES_Services.Decrypt(malformedText, TestKey));

            Assert.IsTrue(ex.Message.Contains("The input is not a valid Base-64 string as it contains a non-base 64 character, more than two padding characters, or an illegal character among the padding characters."));
        }

        [Fact]
        [TestCategory("Decrypt")]
        [ExpectedException(typeof(CryptographicException))]
        public void Decrypt_ValidStringWithWrongKey_ShouldThrowCryptographicException()
        {
            string originalText = "Some secret data";
            string encryptedText = AES_Services.Encrypt(originalText);
            string wrongKey = "WrongKey";

            var ex = Assert.ThrowsException<Exception>(() => AES_Services.Decrypt(encryptedText, wrongKey));

            Assert.IsTrue(ex.Message.Contains("Padding is invalid and cannot be removed."));
        }

        [Fact]
        [TestCategory("Decrypt")]
        public void Decrypt_EmptyEncryptedString_ShouldReturnEmptyString()
        {
            string encryptedEmpty = AES_Services.Encrypt("");

            string decrypted = AES_Services.Decrypt(encryptedEmpty, TestKey);

            Assert.AreEqual("", decrypted);
        }

        [Fact]
        [TestCategory("Decrypt")]
        [ExpectedException(typeof(NullReferenceException))]
        public void Decrypt_NullInput_ShouldThrowNullReferenceException()
        {
            string nullInput = null;

            var ex = Assert.ThrowsException<Exception>(() => AES_Services.Decrypt(nullInput, TestKey));

            Assert.IsTrue(ex.Message.Contains("Object reference not set to an instance of an object."));
        }

        [Fact]
        [TestCategory("Decrypt")]
        public void Decrypt_KeyWithIncorrectLength_ShouldThrowException()
        {
            // Arrange
            string originalText = "Some data";
            string encryptedText = AES_Services.Encrypt(originalText);
            string badKey = "short";

            // Act
            var ex = Assert.ThrowsException<Exception>(() => AES_Services.Decrypt(encryptedText, badKey));

            // Assert
            Assert.IsTrue(ex.Message.Contains("Specified key is not a valid size for this algorithm"));
        }
    }
}
