using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Scaffold
{
    public class PasswordHasher
    {
        private readonly byte[] salt = Array.Empty<byte>();
        private readonly int iterationCount = 10_000;
        private readonly int numBytesRequested = 256 / 8;

        public string Encrypt(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: iterationCount,
                numBytesRequested: numBytesRequested));
        }
    }
}
