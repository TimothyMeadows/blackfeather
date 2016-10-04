/* 
 The MIT License (MIT)

 Copyright (c) 2013 - 2017 Timothy D Meadows II

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

namespace Blackfeather.Security.Cryptography
{
	// TODO: Add more detailed error checking!

	/// <summary>
	/// Blackfeather Cryptology Dependency Injectable Class (Powered By BounceCastle)
	/// </summary>
	public static class Cryptology
	{
		/// <summary>
		/// Encrypt string data with a supported cipher, authenticate the cipher using a supported digest type, deviate the cipher password with a supported divination type, and, finally pad a cipher using a supported padding type.
		/// </summary>
		/// <param name="data">String data.</param>
		/// <param name="password">Cipher password. Will be deviated using the supplied divination type.</param>
		/// <param name="salt">Cipher salt to be used, should be proper format for the cipher type. (Note: Normally 8, or, 14 bytes)</param>
		/// <param name="secondaryVerifier">Secondary data you wish to use to validate the cipher. This is a secondary validation check. The cipher binary is already validated. (Note: This will increase the size of the cipher)</param>
		/// <returns>SaltedData</returns>
		public static SaltedData ToEncryptionInMotion(this string data, string password, byte[] salt, string secondaryVerifier = null)
		{
            var salting = salt ?? 16.ToRandomBytes();
            var cipher = CipherUtilities.GetCipher("AES/CTR/NOPADDING");
            var key = password.ToHash(salting);
            var iv = 16.ToRandomBytes();

            if (!string.IsNullOrWhiteSpace(secondaryVerifier))
            {
                data += secondaryVerifier.ToHmac(key.Data, salt).Data.ToBase64();
            }

            var input = new UTF8Encoding().GetBytes(data);
            cipher.Init(true, new ParametersWithIV(new KeyParameter(key.Data), iv));
            var output = cipher.DoFinal(input);
            cipher.Reset();

            var verifierHash = output.ToHmac(key.Data, salting);
            output = output.Prepend(iv);
            output = output.Append(verifierHash.Data);

            return new SaltedData() { Data = output, Salt = salting };
        }

		/// <summary>
		/// Encrypt byte data with a supported cipher, authenticate the cipher using a supported digest type, deviate the cipher password with a supported divination type, and, finally pad a cipher using a supported padding type.
		/// </summary>
		/// <param name="data">String data.</param>
		/// <param name="password">Cipher password. Will be deviated using the supplied divination type.</param>
		/// <param name="salt">Cipher salt to be used, should be proper format for the cipher type. (Note: Normally 8, or, 14 bytes)</param>
		/// <param name="secondaryVerifier">Secondary data you wish to use to validate the cipher. This is a secondary validation check. The cipher binary is already validated. (Note: This will increase the size of the cipher)</param>
		/// <returns>SaltedData</returns>
		public static SaltedData ToEncryptionInMotion(this byte[] data, string password, byte[] salt, string secondaryVerifier = null)
		{
            var salting = salt ?? 16.ToRandomBytes();
            var cipher = CipherUtilities.GetCipher("AES/CTR/NOPADDING");
            var key = password.ToHash(salting);
            var iv = 16.ToRandomBytes();

            if (!string.IsNullOrWhiteSpace(secondaryVerifier))
            {
                data = data.Append(secondaryVerifier.ToHmac(key.Data, salt).Data);
            }

            cipher.Init(true, new ParametersWithIV(new KeyParameter(key.Data), iv));
            var output = cipher.DoFinal(data);
            cipher.Reset();

            var verifierHash = output.ToHmac(key.Data, salting);
            output = output.Prepend(iv);
            output = output.Append(verifierHash.Data);

            return new SaltedData() { Data = output, Salt = salting };
        }

        /// <summary>
		/// Encrypt string data with a supported cipher, authenticate the cipher using a supported digest type, deviate the cipher password with a supported divination type, and, finally pad a cipher using a supported padding type.
		/// </summary>
		/// <param name="data">String data.</param>
		/// <param name="password">Cipher password. Will be deviated using the supplied divination type.</param>
		/// <param name="salt">Cipher salt to be used, should be proper format for the cipher type. (Note: Normally 8, or, 14 bytes)</param>
		/// <param name="secondaryVerifier">Secondary data you wish to use to validate the cipher. This is a secondary validation check. The cipher binary is already validated. (Note: This will increase the size of the cipher)</param>
		/// <returns>SaltedData</returns>
		public static SaltedData ToEncryptionAtRest(this string data, string password, byte[] salt, string secondaryVerifier = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7");
            var key = password.ToHash(salting);
            var iv = 16.ToRandomBytes();

            if (!string.IsNullOrWhiteSpace(secondaryVerifier))
            {
                data += secondaryVerifier.ToHmac(key.Data, salt).Data.ToBase64();
            }

            var input = new UTF8Encoding().GetBytes(data);
            cipher.Init(true, new ParametersWithIV(new KeyParameter(key.Data), iv));
            var output = cipher.DoFinal(input);
            cipher.Reset();

            var verifierHash = output.ToHmac(key.Data, salting);
            output = output.Prepend(iv);
            output = output.Append(verifierHash.Data);

            return new SaltedData() { Data = output, Salt = salting };
        }

        /// <summary>
		/// Encrypt byte data with a supported cipher, authenticate the cipher using a supported digest type, deviate the cipher password with a supported divination type, and, finally pad a cipher using a supported padding type.
		/// </summary>
		/// <param name="data">String data.</param>
		/// <param name="password">Cipher password. Will be deviated using the supplied divination type.</param>
		/// <param name="salt">Cipher salt to be used, should be proper format for the cipher type. (Note: Normally 8, or, 14 bytes)</param>
		/// <param name="secondaryVerifier">Secondary data you wish to use to validate the cipher. This is a secondary validation check. The cipher binary is already validated. (Note: This will increase the size of the cipher)</param>
		/// <returns>SaltedData</returns>
		public static SaltedData ToEncryptionAtRest(this byte[] data, string password, byte[] salt, string secondaryVerifier = null)
        {
            var salting = salt ?? 16.ToRandomBytes();
            var cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7");
            var key = password.ToHash(salting);
            var iv = 16.ToRandomBytes();

            if (!string.IsNullOrWhiteSpace(secondaryVerifier))
            {
                data = data.Append(secondaryVerifier.ToHmac(key.Data, salt).Data);
            }

            cipher.Init(true, new ParametersWithIV(new KeyParameter(key.Data), iv));
            var output = cipher.DoFinal(data);
            cipher.Reset();

            var verifierHash = output.ToHmac(key.Data, salting);
            output = output.Prepend(iv);
            output = output.Append(verifierHash.Data);

            return new SaltedData() { Data = output, Salt = salting };
        }

        /// <summary>
        /// Decrypt byte data with a supported cipher, authenticate the cipher using a supported digest type, deviate the cipher password with a supported divination type, and, finally pad a cipher using a supported padding type.
        /// </summary>
        /// <param name="data">String data.</param>
        /// <param name="password">Cipher password. Will be deviated using the supplied divination type.</param>
        /// <param name="salt">Cipher salt to be used, should be proper format for the cipher type. (Note: Normally 8, or, 14 bytes)</param>
        /// <param name="secondaryVerifier">Secondary data you wish to use to validate the cipher. This is a secondary validation check. The cipher binary is already validated. (Note: This will increase the size of the cipher)</param>
        /// <returns>Byte data</returns>
        public static byte[] FromEncryptionInMotion(this byte[] data, string password, byte[] salt, string secondaryVerifier = null)
		{
            var iv = data.Slice(0, 16);
            var key = password.ToHash(salt);

            var suspectedInputVerifier = data.Slice(data.Length - 32, data.Length);
            var input = data.Slice(16, data.Length - 32);

            var expectedInputVerifier = input.ToHmac(key.Data, salt);
            if (!suspectedInputVerifier.SequenceEqual(expectedInputVerifier.Data))
            {
                return null;
            }

            var cipher = CipherUtilities.GetCipher("AES/CTR/NOPADDING");
            cipher.Init(false, new ParametersWithIV(new KeyParameter(key.Data), iv));
            var outputData = cipher.DoFinal(input);
            cipher.Reset();

            if (!string.IsNullOrWhiteSpace(secondaryVerifier))
            {
                var expectedVerifier = secondaryVerifier.ToHmac(key.Data, salt).Data.ToHex();
                var suspectedVerifier = new UTF8Encoding().GetString(outputData.Slice(outputData.Length - 64, outputData.Length)).ToUpper();
                if (!expectedVerifier.SequenceEqual(suspectedVerifier))
                {
                    return null;
                }

                outputData = outputData.Slice(0, outputData.Length - 64);
            }

            return outputData;
        }

		/// <summary>
		/// Decrypt string data with a supported cipher, authenticate the cipher using a supported digest type, deviate the cipher password with a supported divination type, and, finally pad a cipher using a supported padding type.
		/// </summary>
		/// <param name="data">String data.</param>
		/// <param name="password">Cipher password. Will be deviated using the supplied divination type.</param>
		/// <param name="salt">Cipher salt to be used, should be proper format for the cipher type. (Note: Normally 8, or, 14 bytes)</param>
		/// <param name="secondaryVerifier">Secondary data you wish to use to validate the cipher. This is a secondary validation check. The cipher binary is already validated. (Note: This will increase the size of the cipher)</param>
		/// <returns>Byte data</returns>
		public static string FromEncryptionInMotion(this string data, string password, byte[] salt, string secondaryVerifier = null)
		{
            var iv = new UTF8Encoding().GetBytes(data.Substring(0, 16));
            var key = password.ToHash(salt);

            var suspectedInputVerifier = data.Substring(data.Length - 32, data.Length);
            var input = new UTF8Encoding().GetBytes(data.Substring(16, data.Length - 32));
            var expectedInputVerifier = input.ToHmac(key.Data, salt);
            if (!suspectedInputVerifier.Equals(expectedInputVerifier.Data))
            {
                return string.Empty;
            }

            var cipher = CipherUtilities.GetCipher("AES/CTR/NOPADDING");
            cipher.Init(false, new ParametersWithIV(new KeyParameter(key.Data), iv));
            var outputData = cipher.DoFinal(input);
            cipher.Reset();

            var output = new UTF8Encoding().GetString(outputData);
            if (!string.IsNullOrWhiteSpace(secondaryVerifier))
            {
                var expectedVerifier = secondaryVerifier.ToHmac(key.Data, salt).Data.ToHex();
                var suspectedVerifier = output.Substring(output.Length - 64, 64);
                if (!expectedVerifier.SequenceEqual(suspectedVerifier))
                {
                    return string.Empty;
                }

                output = output.Substring(0, output.Length - 64);
            }

            return output;
        }

        /// <summary>
        /// Decrypt byte data with a supported cipher, authenticate the cipher using a supported digest type, deviate the cipher password with a supported divination type, and, finally pad a cipher using a supported padding type.
        /// </summary>
        /// <param name="data">String data.</param>
        /// <param name="password">Cipher password. Will be deviated using the supplied divination type.</param>
        /// <param name="salt">Cipher salt to be used, should be proper format for the cipher type. (Note: Normally 8, or, 14 bytes)</param>
        /// <param name="secondaryVerifier">Secondary data you wish to use to validate the cipher. This is a secondary validation check. The cipher binary is already validated. (Note: This will increase the size of the cipher)</param>
        /// <returns>Byte data</returns>
        public static byte[] FromEncryptionAtRest(this byte[] data, string password, byte[] salt, string secondaryVerifier = null)
        {
            var iv = data.Slice(0, 16);
            var key = password.ToHash(salt);

            var suspectedInputVerifier = data.Slice(data.Length - 32, data.Length);
            var input = data.Slice(16, data.Length - 32);

            var expectedInputVerifier = input.ToHmac(key.Data, salt);
            if (!suspectedInputVerifier.SequenceEqual(expectedInputVerifier.Data))
            {
                return null;
            }

            var cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7");
            cipher.Init(false, new ParametersWithIV(new KeyParameter(key.Data), iv));
            var outputData = cipher.DoFinal(input);
            cipher.Reset();

            if (!string.IsNullOrWhiteSpace(secondaryVerifier))
            {
                var expectedVerifier = secondaryVerifier.ToHmac(key.Data, salt).Data.ToHex();
                var suspectedVerifier = new UTF8Encoding().GetString(outputData.Slice(outputData.Length - 64, outputData.Length)).ToUpper();
                if (!expectedVerifier.SequenceEqual(suspectedVerifier))
                {
                    return null;
                }

                outputData = outputData.Slice(0, outputData.Length - 64);
            }

            return outputData;
        }

        /// <summary>
		/// Decrypt string data with a supported cipher, authenticate the cipher using a supported digest type, deviate the cipher password with a supported divination type, and, finally pad a cipher using a supported padding type.
		/// </summary>
		/// <param name="data">String data.</param>
		/// <param name="password">Cipher password. Will be deviated using the supplied divination type.</param>
		/// <param name="salt">Cipher salt to be used, should be proper format for the cipher type. (Note: Normally 8, or, 14 bytes)</param>
		/// <param name="secondaryVerifier">Secondary data you wish to use to validate the cipher. This is a secondary validation check. The cipher binary is already validated. (Note: This will increase the size of the cipher)</param>
		/// <returns>Byte data</returns>
		public static string FromEncryptionAtRest(this string data, string password, byte[] salt, string secondaryVerifier = null)
        {
            var iv = new UTF8Encoding().GetBytes(data.Substring(0, 16));
            var key = password.ToHash(salt);

            var suspectedInputVerifier = data.Substring(data.Length - 32, data.Length);
            var input = new UTF8Encoding().GetBytes(data.Substring(16, data.Length - 32));
            var expectedInputVerifier = input.ToHmac(key.Data, salt);
            if (!suspectedInputVerifier.Equals(expectedInputVerifier.Data))
            {
                return string.Empty;
            }

            var cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7");
            cipher.Init(false, new ParametersWithIV(new KeyParameter(key.Data), iv));
            var outputData = cipher.DoFinal(input);
            cipher.Reset();

            var output = new UTF8Encoding().GetString(outputData);
            if (!string.IsNullOrWhiteSpace(secondaryVerifier))
            {
                var expectedVerifier = secondaryVerifier.ToHmac(key.Data, salt).Data.ToHex();
                var suspectedVerifier = output.Substring(output.Length - 64, 64);
                if (!expectedVerifier.SequenceEqual(suspectedVerifier))
                {
                    return string.Empty;
                }

                output = output.Substring(0, output.Length - 64);
            }

            return output;
        }
    }
}
