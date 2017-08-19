using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;

using Codon.ComponentModel;
using Codon.Services;
using Codon.UIModel;
using Codon.UIModel.Input;

namespace Outcoder.Cryptography.HiddenTextApp
{
	public class MainViewModel : ViewModelBase
	{
		readonly ISettingsService settingsService;
		readonly AesEncryptor encryptor = new AesEncryptor();
		readonly HiddenTextEncoder encoder = new HiddenTextEncoder();

		const string keySettingId = "AesKey";
		const string firstKeyId = "AesKeyOriginal";
			
		public MainViewModel(ISettingsService settingsService)
		{
			this.settingsService = settingsService;
			AesParameters parameters = encryptor.GenerateAesParameters();

			var keyBytes = parameters.Key;

			string keySetting;
			if (settingsService.TryGetSetting(keySettingId, out keySetting) && !string.IsNullOrWhiteSpace(keySetting))
			{
				key = keySetting;
			}
			else
			{
				key = Convert.ToBase64String(keyBytes);
				settingsService.SetSetting(keySettingId, key);
				settingsService.SetSetting(firstKeyId, key);
			}
			//iv = Convert.ToBase64String(parameters.IV);
		}

		string iv = "OolP/1dvx0OqrOc91mBE0Q==";

		string key;

		public string Key
		{
			get => key;
			set
			{
				if (Set(ref key, value) == AssignmentResult.Success)
				{
					settingsService.SetSetting(keySettingId, key);
				}
			}
		}

		string plainText;

		public string PlainText
		{
			get => plainText;
			set
			{
				if (Set(ref plainText, value) == AssignmentResult.Success)
				{
					encodedText = Encode(plainText);
					OnPropertyChanged(nameof(EncodedText));
				}
			}
		}

		byte[] GetBytes(string text)
		{
			return Convert.FromBase64String(text);
		}

		string Encode(string text)
		{
			string textTemp;

			if (useEncryption == true)
			{
				try
				{
					byte[] bytes = encryptor.EncryptString(text, GetBytes(key), GetBytes(iv));
					textTemp = Convert.ToBase64String(bytes);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
					return string.Empty;
				}
			}
			else
			{
				textTemp = text;
			}

			var result = encoder.EncodeAsciiString(textTemp);
			return result;
		}

		string encodedText;

		public string EncodedText
		{
			get => encodedText;
			set
			{
				if (Set(ref encodedText, value) == AssignmentResult.Success)
				{
					plainText = Decode(encodedText);
					OnPropertyChanged(nameof(PlainText));
				}
			}
		}

		string Decode(string encodedText)
		{
			var spaceCharacters = new List<char>(encoder.GetAllSpaceCharactersAsString());

			var sb = new StringBuilder();

			foreach (char c in spaceCharacters)
			{
				string encodedValue = "\\u" + ((int)c).ToString("x4");
				sb.Append(encodedValue);
			}

			string regexString = "(?<spaces>[" + sb.ToString() + "]{4,})";
			Regex regex = new Regex(regexString);

			var matches = regex.Matches(encodedText);

			sb.Clear();

			foreach (Match match in matches)
			{
				string spaces = match.Groups["spaces"].Value;
				try
				{
					string decodedSubstring = DecodeSubstring(spaces);
					sb.AppendLine(decodedSubstring);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
			}

			return sb.ToString();
		}

		string DecodeSubstring(string encodedText)
		{
			string unencodedText = encoder.DecodeSpaceString(encodedText);

			if (useEncryption == true)
			{
				try
				{
					byte[] bytes = Convert.FromBase64String(unencodedText);
					string text = encryptor.DecryptBytes(bytes, GetBytes(key), GetBytes(iv));
					return text;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
					return string.Empty;
				}
			}

			return unencodedText;
		}

		ActionCommand<string> copyToClipboardCommand;

		public ActionCommand<string> CopyToClipboardCommand => copyToClipboardCommand
					?? (copyToClipboardCommand = new ActionCommand<string>(CopyToClipboard));

		void CopyToClipboard(string arg)
		{
			string textToCopy;

			if (arg == "EncodedText")
			{
				textToCopy = encodedText;
			}
			else if (arg == "PlainText")
			{
				textToCopy = plainText;
			}
			else
			{
				return;
			}

			var dataPackage = new DataPackage();
			dataPackage.SetText(textToCopy);
			Clipboard.SetContent(dataPackage);
		}

		bool? useEncryption = false;

		public bool? UseEncryption
		{
			get => useEncryption;
			set
			{
				if (Set(ref useEncryption, value) == AssignmentResult.Success)
				{
					encodedText = Encode(plainText);
					OnPropertyChanged(nameof(EncodedText));
				}
			}
		}

		ActionCommand refreshKeyCommand;

		public ICommand RefreshKeyCommand => refreshKeyCommand
					?? (refreshKeyCommand = new ActionCommand(RefreshKey));

		void RefreshKey(object arg)
		{
			string originalKey;
			if (settingsService.TryGetSetting(firstKeyId, out originalKey))
			{
				Key = originalKey;
			}
			else
			{
				/* Shouldn't get here unless something went awry with the settings. */
				AesParameters parameters = encryptor.GenerateAesParameters();
				var keyBytes = parameters.Key;

				Key = Convert.ToBase64String(keyBytes);
				settingsService.SetSetting(firstKeyId, key);
			}
		}

	}
}
