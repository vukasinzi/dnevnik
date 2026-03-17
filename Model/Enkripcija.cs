using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace journal.Model
{
    internal class Enkripcija
    {
        public string salt { get; set; }
        public string iv { get; set; }
        public string tag { get; set; }
        public string data { get; set; }
    
    
        public static Enkripcija enkriptuj(string plaintext,string lozinka)
        {
            byte[] saltB = RandomNumberGenerator.GetBytes(16);
            byte[] ivB = RandomNumberGenerator.GetBytes(12);
            byte[] kljucB = izvediKljuc(lozinka, saltB);
            byte[] plaintextB = Encoding.UTF8.GetBytes(plaintext);
            byte[] ciphertext = new byte[plaintextB.Length];
            byte[] tagB = new byte[16];

            using var aes = new AesGcm(kljucB,16);
            aes.Encrypt(ivB, plaintextB, ciphertext, tagB);


            return new Enkripcija
            {
                 salt = Convert.ToBase64String(saltB),
                 iv = Convert.ToBase64String(ivB),
                 tag = Convert.ToBase64String(tagB),
                 data = Convert.ToBase64String(ciphertext)
            };
        }
        public string dekriptuj(string lozinka)
        {
            byte[] saltB = Convert.FromBase64String(salt);
            byte[] ivB = Convert.FromBase64String(iv);
            byte[] tagB = Convert.FromBase64String(tag);
            byte[] ciphertext = Convert.FromBase64String(data);
            byte[] kljuc = izvediKljuc(lozinka, saltB);
            byte[] plaintext = new byte[ciphertext.Length];
          
                using var aes = new AesGcm(kljuc, 16);
                aes.Decrypt(ivB, ciphertext, tagB, plaintext);
            
            
            return Encoding.UTF8.GetString(plaintext);
        }
        private static byte[] izvediKljuc(string lozinka, byte[] saltB)
        {
            return Rfc2898DeriveBytes.Pbkdf2(
              password: Encoding.UTF8.GetBytes(lozinka),
              salt: saltB,
              iterations: 150_000,
              hashAlgorithm: HashAlgorithmName.SHA256,
              outputLength: 32
          );
        }
    }
}
