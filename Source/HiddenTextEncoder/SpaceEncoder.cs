using System.Collections.Generic;
using System.Text;

namespace Outcoder.Cryptography
{
	/*
	 digram
	 IsNullOrWhiteSpace return false if the space characters are unicode.
	 */
	 /// <summary>
	 /// This class encodes and decodes text to, and from, text containing
	 /// only unicode space characters. 
	 /// </summary>
	public class SpaceEncoder
	{
		readonly char[] characters =
		{
			'\u0020', /* Space */
			'\u00A0', /* No-Break Space */
			//'\u1680', /* Ogham Space Mark */
			'\u180E', /* Mongolian Vowel Separator */
			'\u2000', /* En Quad */
			'\u2001', /* Em Quad */
			'\u2002', /* En Space */
			'\u2003', /* Em Space */
			'\u2004', /* Three-Per-Em Space */
			'\u2005', /* Four-Per-Em Space */
			'\u2006', /* Six-Per-Em Space */
			'\u2007', /* Figure Space */
			'\u2008', /* Punctuation Space */
			'\u2009', /* Thin Space */
			'\u200A', /* Hair Space */
			'\u202F', /* Narrow No-Break Space */
			'\u205F', /* Medium Mathematical Space */
			'\u3000' /* Ideographic Space */
		};

		readonly Dictionary<char, short> characterIndexDictionary
			= new Dictionary<char, short>();

		public SpaceEncoder()
		{
			for (short i = 0; i < characters.Length; i++)
			{
				characterIndexDictionary[characters[i]] = i;
			}
		}

		/// <summary>
		/// Retrieves all the space characters as a <c>string</c>.
		/// </summary>
		/// <returns>The space characters as a <c>string</c>.</returns>
		public string GetAllSpaceCharactersAsString()
		{
			return string.Join(string.Empty, characters);
		}

		/// <summary>
		/// Retrieves the list of space characters.
		/// </summary>
		public IEnumerable<char> SpaceCharacters => characters;

		/// <summary>
		/// Converts the specified text into a space encoded string.
		/// </summary>
		/// <param name="text">The text to encode.</param>
		/// <returns>The space encoded string.</returns>
		public string EncodeAsciiString(string text)
		{
			var asciiBytes = ConvertStringToAscii(text);
			var encryptedBytes = new char[asciiBytes.Length * 2];
			var encryptedByteCount = 0;

			var stringLength = asciiBytes.Length;

			for (var i = 0; i < stringLength; i++)
			{
				short asciiByte = asciiBytes[i];
				var highPart = (short)(asciiByte / 16);
				var lowPart = (short)(asciiByte % 16);

				encryptedBytes[encryptedByteCount] = characters[highPart];
				encryptedBytes[encryptedByteCount + 1] = characters[lowPart];
				encryptedByteCount += 2;
			}

			var result = string.Join(string.Empty, encryptedBytes);
			return result;
		}

		static byte[] ConvertStringToAscii(string text)
		{
			int length = text.Length;
			byte[] result = new byte[length];

			for (var ix = 0; ix < length; ++ix)
			{
				char c = text[ix];
				if (c <= 0x7f)
				{
					result[ix] = (byte)c;
				}
				else
				{
					result[ix] = (byte)'?';
				}
			}

			return result;
		}

		/// <summary>
		/// Unencodes the specified space encoded string.
		/// </summary>
		/// <param name="spaceString">The string to convert back.</param>
		/// <returns>The original text before it was encoded.</returns>
		public string DecodeSpaceString(string spaceString)
		{
			var spaceStringLength = spaceString.Length;

			var asciiBytes = new byte[spaceStringLength / 2];

			var arrayLength = 0;
			for (var i = 0; i < spaceStringLength; i += 2)
			{
				char space1 = spaceString[i];
				char space2 = spaceString[i + 1];
				short index1 = characterIndexDictionary[space1];
				short index2 = characterIndexDictionary[space2];

				int highPart = index1 * 16;
				short lowPart = index2;

				var asciiByte = highPart + lowPart;
				asciiBytes[arrayLength] = (byte)asciiByte;
				arrayLength++;
			}

			var result = Encoding.ASCII.GetString(asciiBytes, 0, asciiBytes.Length);
			return result;
		}
	}
}