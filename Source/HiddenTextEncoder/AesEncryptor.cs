using System;
using System.IO;
using System.Security.Cryptography;

namespace Outcoder.Cryptography
{
	public class AesParameters
	{
		public byte[] Key { get; set; }
		public byte[] IV { get; set; }
	}

	public class AesEncryptor
	{
		public AesParameters GenerateAesParameters()
		{
			var result = new AesParameters();

			using (var aes = Aes.Create())
			{
				aes.GenerateKey();
				aes.GenerateIV();
				result.Key = aes.Key;
				result.IV = aes.IV;
			}

			return result;
		}

		public byte[] EncryptString(string plainText, byte[] key, byte[] iv)
		{
			if (string.IsNullOrEmpty(plainText))
			{
				throw new ArgumentNullException(nameof(plainText));
			}

			if (key == null || key.Length <= 0)
			{
				throw new ArgumentException(nameof(key));
			}

			if (iv == null || iv.Length <= 0)
			{
				throw new ArgumentException(nameof(iv));
			}

			byte[] encrypted;

			using (Aes aes = Aes.Create())
			{
				aes.Key = key;
				aes.IV = iv;

				// Create a decrytor to perform the stream transform.
				ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

				// Create the streams used for encryption.
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(
								memoryStream, encryptor, CryptoStreamMode.Write))
					{
						using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
						{
							//Write all data to the stream.
							streamWriter.Write(plainText);
						}
						encrypted = memoryStream.ToArray();
					}
				}
			}


			// Return the encrypted bytes from the memory stream.
			return encrypted;
		}

		public string DecryptBytes(byte[] cipherText, byte[] key, byte[] iv)
		{
			if (cipherText == null || cipherText.Length <= 0)
			{
				throw new ArgumentException(nameof(cipherText));
			}

			if (key == null || key.Length <= 0)
			{
				throw new ArgumentException(nameof(key));
			}

			if (iv == null || iv.Length <= 0)
			{
				throw new ArgumentNullException(nameof(iv));
			}

			string result;

			// Create an Aes object
			// with the specified key and IV.
			using (Aes aes = Aes.Create())
			{
				aes.Key = key;
				aes.IV = iv;

				// Create a decrytor to perform the stream transform.
				ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

				// Create the streams used for decryption.
				using (MemoryStream memoryStream = new MemoryStream(cipherText))
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
					{
						using (StreamReader streamReader = new StreamReader(cryptoStream))
						{
							// Read the decrypted bytes from the decrypting stream
							// and place them in a string.
							result = streamReader.ReadToEnd();
						}
					}
				}
			}

			return result;
		}
	}
}
