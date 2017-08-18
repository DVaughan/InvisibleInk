using System;
using System.Text;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Codon.ComponentModel;
using Codon.Services;
using Codon.UIModel;
using Codon.UIModel.Input;
using Outcoder.Cryptography;

namespace HiddenTextEncoder.UI
{
	public class MainViewModel : ViewModelBase
	{
		AesEncryptor encryptor = new AesEncryptor();
		Outcoder.Cryptography.HiddenTextEncoder encoder = new Outcoder.Cryptography.HiddenTextEncoder();
		const string keySettingId = "AesKey";
			
		public MainViewModel(ISettingsService settingsService)
		{
			AesParameters parameters = encryptor.GenerateAesParameters();

			var keyBytes = parameters.Key;

			string keySetting;
			if (settingsService.TryGetSetting(keySettingId, out keySetting))
			{
				key = keySetting;
			}
			else
			{
				key = Convert.ToBase64String(keyBytes);
				settingsService.SetSetting(keySettingId, key);
			}
			//iv = Convert.ToBase64String(parameters.IV);
		}

		string iv = "OolP/1dvx0OqrOc91mBE0Q==";

		string key;

		public string Key
		{
			get => key;
			set => Set(ref key, value);
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

		ActionCommand copyToClipboardCommand;

		public ICommand CopyToClipboardCommand => copyToClipboardCommand
									?? (copyToClipboardCommand = new ActionCommand(CopyToClipboard));

		void CopyToClipboard(object arg)
		{
			if (encodedText == null)
			{
				return;
			}

			var dataPackage = new DataPackage();
			dataPackage.SetText(encodedText);
			Clipboard.SetContent(dataPackage);
		}

		bool? useEncryption = false;

		public bool? UseEncryption
		{
			get => useEncryption;
			set => Set(ref useEncryption, value);
		}

	}
}
