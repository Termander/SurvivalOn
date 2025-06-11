using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SurvivalOn.Models
{
    public static class CryptoHelper
    {
        // Use a fixed key/IV for demo; in production, store securely!
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("ACNO112yuaz990lnze770ielm128nyit"); // 32 bytes for AES-256
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("AyCuNaz222324Oze"); // 16 bytes for AES

        public static string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            if (aes == null)
                throw new PlatformNotSupportedException("AES encryption is not supported on this platform.");
            
            aes.Key = Key;
            aes.IV = IV;
            aes.Mode = CipherMode.CBC; // Explicitly set the mode
            aes.Padding = PaddingMode.PKCS7; // Explicitly set the padding
            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
                sw.Write(plainText);
            return Convert.ToBase64String(ms.ToArray());
        }

        public static string Decrypt(string cipherText)
        {
            var buffer = Convert.FromBase64String(cipherText);
            using var aes = Aes.Create();
            if (aes == null)
                throw new PlatformNotSupportedException("AES decryption is not supported on this platform.");
            
            aes.Key = Key;
            aes.IV = IV;
            aes.Mode = CipherMode.CBC; // Explicitly set the mode
            aes.Padding = PaddingMode.PKCS7; // Explicitly set the padding
            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(buffer);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
}