
using System;
using System.Linq;

namespace Outcoder.Cryptography.Tests
{
	class StringGenerator
	{
		readonly Random random = new Random();

		public string CreateRandomString(int length)
		{
			/* From https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings-in-c */
			const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[random.Next(s.Length)]).ToArray());
		}
	}
}
