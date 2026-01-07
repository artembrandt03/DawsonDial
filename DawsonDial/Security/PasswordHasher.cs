using System;
using System.Security.Cryptography;

namespace DawsonDial.Security
{
    /// <summary>
    /// Provides methods for hashing and verifying passwords.
    /// </summary>
    public static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int KeySize  = 32;
        private const int Iterations = 100_000;

        /// <summary>
        /// Hashes a password using PBKDF2 with SHA256.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>The hashed password as a base64 string.</returns>
        public static string Hash(string password)
        {
            // Generate a salt
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            // Derive a key
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var key = pbkdf2.GetBytes(KeySize);

            // Salt + Key
            var hashBytes = new byte[SaltSize + KeySize];
            Buffer.BlockCopy(salt, 0, hashBytes, 0, SaltSize);
            Buffer.BlockCopy(key, 0, hashBytes, SaltSize, KeySize);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Verifies a password against a hashed password.
        /// </summary>
        /// <param name="password">The password to verify.</param>
        /// <param name="hashedPassword">The hashed password to verify against.</param>
        /// <returns>True if the password matches the hashed password; otherwise, false.</returns>
        public static bool Verify(string password, string hashedPassword)
        {
            var hashBytes = Convert.FromBase64String(hashedPassword);

            // Get the salt
            var salt = new byte[SaltSize];
            Buffer.BlockCopy(hashBytes, 0, salt, 0, SaltSize);

            // Re-derive key
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var key = pbkdf2.GetBytes(KeySize);

            // Compare the stored key to the derived key
            for (int i = 0; i < KeySize; i++)
            {
                if (hashBytes[i + SaltSize] != key[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
