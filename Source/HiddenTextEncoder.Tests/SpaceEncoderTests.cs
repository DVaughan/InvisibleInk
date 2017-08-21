using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Outcoder.Cryptography.Tests
{
	[TestClass]
	public class SpaceEncoderTests
	{
		readonly Random random = new Random();

		[TestMethod]
		public void ShouldEncodeAndDecode()
		{
			var encoder = new SpaceEncoder();

			string whiteSpaceCharacters = encoder.GetAllSpaceCharactersAsString();
			var stringGenerator = new StringGenerator();

			for (int i = 0; i < 1000; i++)
			{
				string s = stringGenerator.CreateRandomString(random.Next(0, 30));
				var encoded = encoder.EncodeAsciiString(s);

				Assert.IsNotNull(encoded);

				foreach (char c in encoded)
				{
					Assert.IsTrue(whiteSpaceCharacters.Contains(c));
				}

				var unencoded = encoder.DecodeSpaceString(encoded);
				Assert.AreEqual(s, unencoded);
			}

			//string encoded = encoder.EncodeAsciiString("Make this hidden");
			//string original = encoder.DecodeSpaceString(encoded);
		}
	}
}
