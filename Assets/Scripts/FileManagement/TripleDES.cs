using System;
using System.Security.Cryptography;
using System.Text;

namespace yipli.Windows.Cryptography
{
    public static class TripleDES
    {
        private const string securityKey = "sportifying childhood";

        public static string Encrypt(string TextToEncrypt)
        {
            byte[] EncryptedArray = UTF8Encoding.UTF8.GetBytes(TextToEncrypt);

            MD5CryptoServiceProvider MD5CryptoService = new MD5CryptoServiceProvider();

            byte[] securityKeyArray = MD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(securityKey));

            MD5CryptoService.Clear();

            var TripleDESCryptoService = new TripleDESCryptoServiceProvider();

            TripleDESCryptoService.Key = securityKeyArray;

            TripleDESCryptoService.Mode = CipherMode.ECB;

            TripleDESCryptoService.Padding = PaddingMode.PKCS7;

            var MyCrytpoTransform = TripleDESCryptoService.CreateEncryptor();

            byte[] resultArray = MyCrytpoTransform.TransformFinalBlock(EncryptedArray, 0, EncryptedArray.Length);

            TripleDESCryptoService.Clear();

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }



        public static string Decrypt(string TextToDecrypt)
        {
            byte[] DecryptArray = Convert.FromBase64String(TextToDecrypt);

            MD5CryptoServiceProvider MD5CryptoService = new MD5CryptoServiceProvider();

            byte[] securityKeyArray = MD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(securityKey));

            MD5CryptoService.Clear();

            var TripleDESCryptoService = new TripleDESCryptoServiceProvider();

            TripleDESCryptoService.Key = securityKeyArray;

            TripleDESCryptoService.Mode = CipherMode.ECB;

            TripleDESCryptoService.Padding = PaddingMode.PKCS7;

            var CrytpoTransform = TripleDESCryptoService.CreateDecryptor();

            byte[] resultArray = CrytpoTransform.TransformFinalBlock(DecryptArray, 0, DecryptArray.Length);

            TripleDESCryptoService.Clear();

            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}
