/* 
 The MIT License (MIT)

 Copyright (c) 2013 - 2014 Timothy D Meadows II

 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 THE SOFTWARE.
*/

using System.Linq;
using System.Text;
using Blackfeather.Data.Encoding;
using Blackfeather.Extention;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;

namespace Blackfeather.Security.Cryptography
{
	// TODO: Add more detailed error checking!

	/// <summary>
	/// Blackfeather Cryptology Dependancy Injectable Class (Powered By BounceCastle)
	/// </summary>
	public static class Cryptology
	{
		/// <summary>
		/// Supported cipher types
		/// </summary>
		public enum CipherType
		{
			AesCtr = 1,
			AesCbc = 2,
			AesCfb = 3,
			AesCts = 4,
			AesOfb = 5,
			CamelliaCbc = 6,
			CamelliaCfb = 7,
			CamelliaCts = 8,
			CamelliaOfb = 9
		}

		/// <summary>
		/// Supported digest types
		/// </summary>
		public enum CipherDigestType
		{
			Sha256 = 1,
			Gost3411 = 2,
			Tiger = 3
		}

		/// <summary>
		/// Supported padding types
		/// </summary>
		public enum CipherPaddingType
		{
			None = 1,
			Pkcs7 = 2,
			Ansix923 = 3,
			Iso10126 = 4,
		}

		/// <summary>
		/// Encrypt string data with a supported cipher, authenticate the cipher using a supported digest type, devinate the cipher password with a supported devination type, and, finally pad a cipher using a supported padding type.
		/// </summary>
		/// <param name="data">String data.</param>
		/// <param name="digestType">Supported digest types.</param>
		/// <param name="devinationType">Supported devination types.</param>
		/// <param name="cipherType">Supported cipher types.</param>
		/// <param name="paddingType">Supported padding types.</param>
		/// <param name="password">Cipher password. Will be devinated using the supplied devination type.</param>
		/// <param name="salt">Cipher salt to be used, should be proper format for the cipher type. (Note: Normally 8, or, 14 bytes)</param>
		/// <param name="secondaryVerifier">Secondary data you wish to use to validate the cipher. This is a secondary validation check. The cipher binary is already validated. (Note: This will increase the size of the cipher)</param>
		/// <returns>SaltedData</returns>
		public static SaltedData ToCipher(this string data, CipherDigestType digestType, KeyDevination.DevinationType devinationType, CipherType cipherType, CipherPaddingType paddingType, string password, byte[] salt, string secondaryVerifier = null)
		{
			CheckCipherSettings(cipherType, paddingType, data.Length);
			return _ToCipher(data, digestType, devinationType, cipherType, paddingType, password, salt, secondaryVerifier);
		}

		/// <summary>
		/// Encrypt byte data with a supported cipher, authenticate the cipher using a supported digest type, devinate the cipher password with a supported devination type, and, finally pad a cipher using a supported padding type.
		/// </summary>
		/// <param name="data">String data.</param>
		/// <param name="digestType">Supported digest types.</param>
		/// <param name="devinationType">Supported devination types.</param>
		/// <param name="cipherType">Supported cipher types.</param>
		/// <param name="paddingType">Supported padding types.</param>
		/// <param name="password">Cipher password. Will be devinated using the supplied devination type.</param>
		/// <param name="salt">Cipher salt to be used, should be proper format for the cipher type. (Note: Normally 8, or, 14 bytes)</param>
		/// <param name="secondaryVerifier">Secondary data you wish to use to validate the cipher. This is a secondary validation check. The cipher binary is already validated. (Note: This will increase the size of the cipher)</param>
		/// <returns>SaltedData</returns>
		public static SaltedData ToCipher(this byte[] data, CipherDigestType digestType, KeyDevination.DevinationType devinationType, CipherType cipherType, CipherPaddingType paddingType, string password, byte[] salt, string secondaryVerifier = null)
		{
			CheckCipherSettings(cipherType, paddingType, data.Length);
			return _ToCipher(data, digestType, devinationType, cipherType, paddingType, password, salt, secondaryVerifier);
		}

		/// <summary>
		/// Decrypt byte data with a supported cipher, authenticate the cipher using a supported digest type, devinate the cipher password with a supported devination type, and, finally pad a cipher using a supported padding type.
		/// </summary>
		/// <param name="data">String data.</param>
		/// <param name="digestType">Supported digest types.</param>
		/// <param name="devinationType">Supported devination types.</param>
		/// <param name="cipherType">Supported cipher types.</param>
		/// <param name="paddingType">Supported padding types.</param>
		/// <param name="password">Cipher password. Will be devinated using the supplied devination type.</param>
		/// <param name="salt">Cipher salt to be used, should be proper format for the cipher type. (Note: Normally 8, or, 14 bytes)</param>
		/// <param name="secondaryVerifier">Secondary data you wish to use to validate the cipher. This is a secondary validation check. The cipher binary is already validated. (Note: This will increase the size of the cipher)</param>
		/// <returns>Byte data</returns>
		public static byte[] FromCipher(this byte[] data, CipherDigestType digestType, KeyDevination.DevinationType devinationType, CipherType cipherType, CipherPaddingType paddingType, string password, byte[] salt, string secondaryVerifier = null)
		{
			CheckCipherSettings(cipherType, paddingType, data.Length);
			return _FromCipher(data, digestType, devinationType, cipherType, paddingType, password, salt, secondaryVerifier);
		}

		/// <summary>
		/// Decrypt string data with a supported cipher, authenticate the cipher using a supported digest type, devinate the cipher password with a supported devination type, and, finally pad a cipher using a supported padding type.
		/// </summary>
		/// <param name="data">String data.</param>
		/// <param name="digestType">Supported digest types.</param>
		/// <param name="devinationType">Supported devination types.</param>
		/// <param name="cipherType">Supported cipher types.</param>
		/// <param name="paddingType">Supported padding types.</param>
		/// <param name="password">Cipher password. Will be devinated using the supplied devination type.</param>
		/// <param name="salt">Cipher salt to be used, should be proper format for the cipher type. (Note: Normally 8, or, 14 bytes)</param>
		/// <param name="secondaryVerifier">Secondary data you wish to use to validate the cipher. This is a secondary validation check. The cipher binary is already validated. (Note: This will increase the size of the cipher)</param>
		/// <returns>Byte data</returns>
		public static string FromCipher(this string data, CipherDigestType digestType, KeyDevination.DevinationType devinationType, CipherType cipherType, CipherPaddingType paddingType, string password, byte[] salt, string secondaryVerifier = null)
		{
			CheckCipherSettings(cipherType, paddingType, data.Length);
			return _FromCipher(data, digestType, devinationType, cipherType, paddingType, password, salt, secondaryVerifier);
		}

		private static SaltedData _ToCipher(string data, CipherDigestType digestType, KeyDevination.DevinationType devinationType, CipherType cipherType, CipherPaddingType paddingType, string password, byte[] salt = null, string secondaryVerifier = null)
		{
			var salting = salt ?? 16.ToRandomBytes();
			var cipher = CipherUtilities.GetCipher(string.Format("{0}/{1}", ToCipherString(cipherType), ToPaddingString(paddingType)));
			var key = password.ToHash(ToDigestType(digestType), devinationType, salting);
			var iv = 16.ToRandomBytes();

			if (!string.IsNullOrWhiteSpace(secondaryVerifier))
			{
				data += secondaryVerifier.ToHmac(ToMacDigestType(digestType), devinationType, key.Data, salt).Data.ToBase64();
			}

			var input = new UTF8Encoding().GetBytes(data);
			cipher.Init(true, new ParametersWithIV(new KeyParameter(key.Data), iv));
			var output = cipher.DoFinal(input);
			cipher.Reset();

			var verifierHash = output.ToHmac(ToMacDigestType(digestType), devinationType, key.Data, salting);
			output = output.Prepend(iv);
			output = output.Append(verifierHash.Data);

			return new SaltedData() { Data = output, Salt = salting };
		}

		private static SaltedData _ToCipher(byte[] data, CipherDigestType digestType, KeyDevination.DevinationType devinationType, CipherType cipherType, CipherPaddingType paddingType, string password, byte[] salt = null, string secondaryVerifier = null)
		{
			var salting = salt ?? 16.ToRandomBytes();
			var cipher = CipherUtilities.GetCipher(string.Format("{0}/{1}", ToCipherString(cipherType), ToPaddingString(paddingType)));
			var key = password.ToHash(ToDigestType(digestType), devinationType, salting);
			var iv = 16.ToRandomBytes();

			if (!string.IsNullOrWhiteSpace(secondaryVerifier))
			{
				data = data.Append(secondaryVerifier.ToHmac(ToMacDigestType(digestType), devinationType, key.Data, salt).Data);
			}

			cipher.Init(true, new ParametersWithIV(new KeyParameter(key.Data), iv));
			var output = cipher.DoFinal(data);
			cipher.Reset();

			var verifierHash = output.ToHmac(ToMacDigestType(digestType), devinationType, key.Data, salting);
			output = output.Prepend(iv);
			output = output.Append(verifierHash.Data);

			return new SaltedData() { Data = output, Salt = salting };
		}

		private static byte[] _FromCipher(byte[] data, CipherDigestType digestType, KeyDevination.DevinationType devinationType, CipherType cipherType, CipherPaddingType paddingType, string password, byte[] salt, string secondaryVerifier = null)
		{
			var digestLength = ToDigestLength(digestType);
			var iv = data.Slice(0, 16);
			var key = password.ToHash(ToDigestType(digestType), devinationType, salt);

			var suspectedInputVerifier = data.Slice(data.Length - digestLength, data.Length);
			var input = data.Slice(16, data.Length - digestLength);

			var expectedInputVerifier = input.ToHmac(ToMacDigestType(digestType), devinationType, key.Data, salt);
			if (!suspectedInputVerifier.SequenceEqual(expectedInputVerifier.Data))
			{
				return null;
			}

			var cipher = CipherUtilities.GetCipher(string.Format("{0}/{1}", ToCipherString(cipherType), ToPaddingString(paddingType)));
			cipher.Init(false, new ParametersWithIV(new KeyParameter(key.Data), iv));
			var outputData = cipher.DoFinal(input);
			cipher.Reset();

			if (!string.IsNullOrWhiteSpace(secondaryVerifier))
			{
				var expectedVerifier = secondaryVerifier.ToHmac(ToMacDigestType(digestType), devinationType, key.Data, salt).Data.ToHex();
				var suspectedVerifier = new UTF8Encoding().GetString(outputData.Slice((outputData.Length - 1) - (digestLength*2) + 1, outputData.Length)).ToUpper();
				if (!expectedVerifier.SequenceEqual(suspectedVerifier))
				{
					return null;
				}

				outputData = outputData.Slice(0, outputData.Length - digestLength * 2);
			}

			return outputData;
		}

		private static string _FromCipher(string data, CipherDigestType digestType, KeyDevination.DevinationType devinationType, CipherType cipherType, CipherPaddingType paddingType, string password, byte[] salt, string secondaryVerifier = null)
		{
			var digestLength = ToDigestLength(digestType);
			var digestHexLength = digestLength * 2;
			var iv = new UTF8Encoding().GetBytes(data.Substring(0, 16));
			var key = password.ToHash(ToDigestType(digestType), devinationType, salt);

			var suspectedInputVerifier = data.Substring(data.Length - digestLength, data.Length);
			var input = new UTF8Encoding().GetBytes(data.Substring(16, data.Length - digestLength));
			var expectedInputVerifier = input.ToHmac(ToMacDigestType(digestType), devinationType, key.Data, salt);
			if (!suspectedInputVerifier.Equals(expectedInputVerifier.Data))
			{
				return string.Empty;
			}

			var cipher = CipherUtilities.GetCipher(string.Format("{0}/{1}", ToCipherString(cipherType), ToPaddingString(paddingType)));
			cipher.Init(false, new ParametersWithIV(new KeyParameter(key.Data), iv));
			var outputData = cipher.DoFinal(input);
			cipher.Reset();

			var output = new UTF8Encoding().GetString(outputData);
			if (!string.IsNullOrWhiteSpace(secondaryVerifier))
			{
				var expectedVerifier = secondaryVerifier.ToHmac(ToMacDigestType(digestType), devinationType, key.Data, salt).Data.ToHex();
				var suspectedVerifier = output.Substring(output.Length - digestHexLength, digestHexLength);
				if (!expectedVerifier.SequenceEqual(suspectedVerifier))
				{
					return string.Empty;
				}

				output = output.Substring(0, output.Length - digestHexLength);
			}

			return output;
		}

		private static void CheckCipherSettings(CipherType cipherType, CipherPaddingType paddingType, int length)
		{
			if (paddingType == CipherPaddingType.None)
			{
				switch (cipherType)
				{
				case CipherType.AesCtr:
				case CipherType.AesCfb:
				case CipherType.AesOfb:
				case CipherType.CamelliaCfb:
				case CipherType.CamelliaOfb:
					break;
				default:
					throw new CryptoException(string.Format("{0} can't be used without padding.", ToCipherString(cipherType)));
				}
			}

			switch (cipherType)
			{
			case CipherType.AesCts:
			case CipherType.CamelliaCts:
				if (length < 16)
				{
					throw new CryptoException("Cipher mode requires at lest one block of data. This means the data must be at least 16 bytes or greater per standard block size.");
				}
				break;
			}
		}

		private static string ToCipherString(CipherType cipher)
		{
			var cipherString = "AES/CBC";
			switch (cipher)
			{
			case CipherType.AesCtr:
				cipherString = "AES/CTR";
				break;
			case CipherType.AesCfb:
				cipherString = "AES/CFB";
				break;
			case CipherType.AesCts:
				cipherString = "AES/CTS";
				break;
			case CipherType.AesOfb:
				cipherString = "AES/OFB";
				break;
			case CipherType.CamelliaCbc:
				cipherString = "CAMELLIA/CBC";
				break;
			case CipherType.CamelliaCfb:
				cipherString = "CAMELLIA/CFB";
				break;
			case CipherType.CamelliaCts:
				cipherString = "CAMELLIA/CTS";
				break;
			case CipherType.CamelliaOfb:
				cipherString = "CAMELLIA/OFB";
				break;
			}

			return cipherString;
		}

		private static string ToPaddingString(CipherPaddingType padding)
		{
			var paddingString = "NOPADDING";
			switch (padding)
			{
			case CipherPaddingType.Pkcs7:
				paddingString = "PKCS7PADDING";
				break;
			case CipherPaddingType.Ansix923:
				paddingString = "X923PADDING";
				break;
			case CipherPaddingType.Iso10126:
				paddingString = "ISO10126PADDING";
				break;
			}

			return paddingString;
		}

		private static int ToDigestLength(CipherDigestType digestType)
		{
			var digest = 0;

			switch (digestType)
			{
			case CipherDigestType.Sha256:
				digest = 32;
				break;
			case CipherDigestType.Gost3411:
				digest = 32;
				break;
			case CipherDigestType.Tiger:
				digest = 24;
				break;
			}

			return digest;
		}

		private static Hash.DigestType ToDigestType(CipherDigestType digestType)
		{
			var digest = default(Hash.DigestType);

			switch (digestType)
			{
			case CipherDigestType.Sha256:
				digest = Hash.DigestType.Sha256;
				break;
			case CipherDigestType.Gost3411:
				digest = Hash.DigestType.Gost3411;
				break;
			case CipherDigestType.Tiger:
				digest = Hash.DigestType.Tiger;
				break;
			}

			return digest;
		}

		private static Hmac.DigestType ToMacDigestType(CipherDigestType digestType)
		{
			var digest = default(Hmac.DigestType);

			switch (digestType)
			{
			case CipherDigestType.Sha256:
				digest = Hmac.DigestType.Sha256;
				break;
			case CipherDigestType.Gost3411:
				digest = Hmac.DigestType.Gost3411;
				break;
			case CipherDigestType.Tiger:
				digest = Hmac.DigestType.Tiger;
				break;
			}

			return digest;
		}
	}
}
