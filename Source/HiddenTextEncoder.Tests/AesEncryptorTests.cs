using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Outcoder.Cryptography.Tests
{
	[TestClass]
	public class AesEncryptorTests
	{
		readonly Random random = new Random();

		[TestMethod]
		public void ShouldEncryptAndDecrypt()
		{
			var aesEncryptor = new AesEncryptor();
			var stringGenerator = new StringGenerator();

			for (int i = 0; i < 1000; i++)
			{
				string randomString = stringGenerator.CreateRandomString(random.Next(1, 30));

				var parameters = aesEncryptor.GenerateAesParameters();
				byte[] keyBytes = parameters.Key;
				byte[] ivBytes = parameters.IV;

				byte[] encryptedBytes = aesEncryptor.EncryptString(randomString, keyBytes, ivBytes);

				Assert.IsNotNull(encryptedBytes);

				string unencrypted = aesEncryptor.DecryptBytes(encryptedBytes, keyBytes, ivBytes);

				Assert.AreEqual(randomString, unencrypted);
			}
		}
	}
}
