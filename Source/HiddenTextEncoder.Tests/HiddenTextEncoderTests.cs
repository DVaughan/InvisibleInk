using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Outcoder.Cryptography.Tests
{
	[TestClass]
	public class HiddenTextEncoderTests
	{
		readonly Random random = new Random();

		[TestMethod]
		public void ShouldEncodeAndDecode()
		{
			var hiddenTextEncoder = new HiddenTextEncoder();
			string whiteSpaceCharacters = hiddenTextEncoder.GetAllSpaceCharactersAsString();
			var stringGenerator = new StringGenerator();

			for (int i = 0; i < 1000; i++)
			{
				string s = stringGenerator.CreateRandomString(random.Next(0, 30));
				var encoded = hiddenTextEncoder.EncodeAsciiString(s);

				Assert.IsNotNull(encoded);

				foreach (char c in encoded)
				{
					Assert.IsTrue(whiteSpaceCharacters.Contains(c));
				}

				var unencoded = hiddenTextEncoder.DecodeSpaceString(encoded);
				Assert.AreEqual(s, unencoded);
			}
		}
	}
}
