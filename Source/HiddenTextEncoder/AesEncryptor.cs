using System;
using System.IO;
using System.Security.Cryptography;

namespace Outcoder.Cryptography
{
	/// <summary>
	/// This class holds the parameters that are required 
	/// to encrypt and decrypt using the <see cref="AesEncryptor"/>
	/// </summary>
	public class AesParameters
	{
		public byte[] Key { get; set; }
		public byte[] IV { get; set; }
	}

	/// <summary>
	/// This class encrypts and decrypts text using the AES algorithm.
	/// </summary>
	public class AesEncryptor
	{
		/// <summary>
		/// Generates a random AES key and IV.
		/// </summary>
		/// <returns>The <c>AesParameters</c> containing 
		/// a random key and IV.</returns>
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

		/// <summary>
		/// Encrypts the specified plain text.
		/// </summary>
		/// <param name="plainText">The text to encrypt.</param>
		/// <param name="key">A key to use for the encryption. 
		/// The same key must be used for decryption.</param>
		/// <param name="iv">An intermediate value to use for the encryption. 
		/// The same IV must be used for decryption.</param>
		/// <returns>A byte array of encrypted characters.</returns>
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

				ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(
								memoryStream, encryptor, CryptoStreamMode.Write))
					{
						using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
						{
							streamWriter.Write(plainText);
						}
						encrypted = memoryStream.ToArray();
					}
				}
			}

			return encrypted;
		}

		/// <summary>
		/// Returns the specified cipher text to plain text.
		/// </summary>
		/// <param name="cipherText">The cipher text bytes.</param>
		/// <param name="key">The key that was used to encrypt the plain text.</param>
		/// <param name="iv">The initialization vector, which helps to prevent an attacker 
		/// to infer relationships between segments of the encrypted message.</param>
		/// <returns></returns>
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

			using (Aes aes = Aes.Create())
			{
				aes.Key = key;
				aes.IV = iv;

				ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

				using (MemoryStream memoryStream = new MemoryStream(cipherText))
				{
					using (CryptoStream cryptoStream = new CryptoStream(
								memoryStream, decryptor, CryptoStreamMode.Read))
					{
						using (StreamReader streamReader = new StreamReader(cryptoStream))
						{
							result = streamReader.ReadToEnd();
						}
					}
				}
			}

			return result;
		}
	}
}
