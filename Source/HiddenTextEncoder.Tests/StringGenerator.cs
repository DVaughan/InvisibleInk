using System;
using System.Linq;

namespace Outcoder.Cryptography.Tests
{
	class StringGenerator
	{
		readonly Random random = new Random();
		readonly string asciiCharacters
			= new string(Enumerable.Range(0, 255).Select(x => (char)x).ToArray());

		public string CreateRandomString(int length)
		{
			return new string(Enumerable.Repeat(asciiCharacters, length)
				.Select(s => s[random.Next(s.Length)]).ToArray());
		}
	}
}
