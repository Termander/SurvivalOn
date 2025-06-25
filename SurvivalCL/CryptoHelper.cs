using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalCL
{
    public static class CryptoHelper
    {
        // Use a fixed key/IV for demo; in production, store securely!
        // Use a fixed key/IV for demo; in production, store securely!
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("ACNO112yuaz990lnze770ielm128nyit"); // 32 bytes for AES-256
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("AyCuNaz222324Oze"); // 16 bytes for AES

        public static string Encrypt(string plainText)
        {
            /*
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;
            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
                sw.Write(plainText);
            return Convert.ToBase64String(ms.ToArray());*/
            return plainText;
        }

        public static string Decrypt(string cipherText)
        {
            /*
            var buffer = Convert.FromBase64String(cipherText);
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;
            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(buffer);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
            */
            return cipherText;
        }
    }
}
