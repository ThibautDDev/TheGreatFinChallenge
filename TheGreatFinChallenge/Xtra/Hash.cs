using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace TheGreatFinChallenge.Xtra
{
    public class Hash
    {
        public static string HashPassword(string password, Byte[] salt) => 
            Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));


        public static string HashPassword(string password, string salt) =>
            Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Convert.FromBase64String(salt),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            

        public static Byte[] GenerateSalt()
        {
            var salt = new byte[32];
            using (var random = new RNGCryptoServiceProvider()) random.GetNonZeroBytes(salt);
            return salt;
        }


        public static string ConvertSaltToString(Byte[] salt) => Convert.ToBase64String(salt);

        public static bool PasswordMeetsRequirements(string password) => HasDigit(password) && HasSpecialChars(password) && password.Length >= 8;

        public static bool HasSpecialChars(string testString) => testString.Any(c => !Char.IsLetterOrDigit(c));

        public static bool HasDigit(string testString) => testString.Any(char.IsDigit);
    }
}
