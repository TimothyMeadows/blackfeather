using System;
using System.Diagnostics;
using System.Text;
using Blackfeather.Data.Encoding;
using Blackfeather.Security.Cryptography;

namespace BlackfeatherTestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            // ecb needs more info
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var salt = ("6DGL3DJQFXZQKD3HPKEAPZ3D5M").FromBase32();
            var data = DateTime.Now.ToBinary().ToString();
            var dataLength = data.Length;
            var cipher = data.ToCipher(Cryptology.CipherDigestType.Tiger, KeyDevination.DevinationType.Pbkdf2,
                Cryptology.CipherType.AesCts, Cryptology.CipherPaddingType.Pkcs7, "caw!", salt);

            var cipherHex = cipher.Data.ToHex();
            var cipherHexLength = cipherHex.Length;
            var cipherBase32 = cipher.Data.ToBase32();
            var cipherBase32Length = cipherBase32.Length;
            var cipherBase64 = cipher.Data.ToBase64();
            var cipherBase64Length = cipherBase64.Length;

            var original = cipher.Data.FromCipher(Cryptology.CipherDigestType.Tiger,
                KeyDevination.DevinationType.Pbkdf2, Cryptology.CipherType.AesCts, Cryptology.CipherPaddingType.Pkcs7,
                "caw!", salt);
            var originalString = new UTF8Encoding().GetString(original);
            
            stopwatch.Stop();

            // 8043A14FB700E3ADE03E5B81EAA8D8BAC6EB15F8EEE4F20F57132716F06D96CD
            // 36C0A54302CD69E0E29868F5A63C3B6AD3DFF9BD71EF499B186B56D7DA2F002FAFFE2131ADF71780C8CE79271857B7A9
            // 683A8DE6DCA0307EE060BECD0196C765B8ED19A9F83C213994431BA6030450E608AD76BCF837052AECD2FA45418ADCD79332078274A5EDCBCCEB7C2D0A028B48
            // DAE79DA2BBA0A00DEA53A6E229244107878D45C085E3A8F279E646572763D5D6
            return;
        }
    }
}
