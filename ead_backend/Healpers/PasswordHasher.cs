// File: PasswordHasher
// Author: M.W.H.S.L Ruwanpura
// IT Number: IT21191688
// Description:

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;

namespace ead_backend.Healpers
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            var salt = new byte[16];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return Convert.ToBase64String(salt) + ":" + hashed;
        }

        public static bool VerifyPassword(string hashedPassword, string password)
        {
            var parts = hashedPassword.Split(':');
            var salt = Convert.FromBase64String(parts[0]);
            var storedHash = parts[1];

            var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hash == storedHash;
        }
    }
}
