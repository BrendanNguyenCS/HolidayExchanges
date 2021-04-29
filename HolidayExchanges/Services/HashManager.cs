using System;
using System.Security.Cryptography;

namespace HolidayExchanges.Services
{
    /// <summary>
    /// Represents the logic for the password protection process
    /// </summary>
    public class HashManager
    {
        /// <value>The size, in bytes, for the pre-hash password.</value>
        private const int _salt_size = 32;

        /// <value>The number of iterations the hashing algorithm will perform before finishing.</value>
        private const int _num_iterations = 10000;

        /// <value>The size of the final hashed password.</value>
        private const int _subkey_length = 32; // 256 bits % 8 bits per byte

        /// <summary>
        /// Generates a random salt
        /// </summary>
        /// <returns></returns>
        public byte[] GenerateSalt()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var randomNumber = new byte[_salt_size];
                rng.GetNonZeroBytes(randomNumber);

                return randomNumber;
            }
        }

        /// <summary>
        /// Computes the hash given a <paramref name="password"/> string and a <paramref name="salt"/>.
        /// </summary>
        /// <param name="password">A password string.</param>
        /// <param name="salt">A random salt <see cref="byte"/> array.</param>
        /// <returns>The hashed and salted password using PBKDF2 and <see cref="SHA256"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public byte[] ComputeHash(string password, byte[] salt)
        {
            if (string.IsNullOrEmpty(password) || salt == null) throw new ArgumentNullException();

            var rfc = new Rfc2898DeriveBytes(password, salt, _num_iterations, HashAlgorithmName.SHA256);
            var hashPassword = rfc.GetBytes(_subkey_length);
            return hashPassword;
        }

        /// <summary>
        /// Authenticates entered password against stored password and salt during a login attempt.
        /// </summary>
        /// <param name="enteredPassword">The password used in the login attempt.</param>
        /// <param name="storedPassword">The password that is stored in the db.</param>
        /// <param name="storedSalt">
        /// The salt stored in the db that is associated with the stored password.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the hashed <paramref name="enteredPassword"/> with the
        /// <paramref name="storedSalt"/> matches the <paramref name="storedPassword"/>. <see
        /// langword="false"/> otherwise.
        /// </returns>
        public bool VerifyPassword(string enteredPassword, string storedPassword, string storedSalt)
        {
            if (string.IsNullOrEmpty(enteredPassword) || string.IsNullOrEmpty(storedPassword) || string.IsNullOrEmpty(storedSalt)) return false;

            try
            {
                var saltBytes = Convert.FromBase64String(storedSalt);
                var storedFullHash = Convert.FromBase64String(storedPassword);
                var enteredHash = this.ComputeHash(enteredPassword, saltBytes);

                return Compare(enteredHash, storedFullHash);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Private helper method for simplified hash computation.
        /// </summary>
        /// <param name="first">A <see cref="byte"/> array.</param>
        /// <param name="second">A <see cref="byte"/> array.</param>
        /// <returns>
        /// The combined <see cref="byte"/> array consisting of <paramref name="first"/> with
        /// <paramref name="second"/> appended on the end.
        /// </returns>
        private byte[] Combine(byte[] first, byte[] second)
        {
            var result = new byte[first.Length + second.Length];

            Buffer.BlockCopy(first, 0, result, 0, first.Length);
            Buffer.BlockCopy(second, 0, result, first.Length, second.Length);

            return result;
        }

        /// <summary>
        /// Private helper method for simplified password verification process.
        /// </summary>
        /// <param name="first">A <see cref="byte"/> array.</param>
        /// <param name="second">Another <see cref="byte"/> array.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="first"/> and <paramref name="second"/> are
        /// equal in length and character order. <see langword="false"/> otherwise.
        /// </returns>
        private bool Compare(byte[] first, byte[] second)
        {
            if (first == null && second == null) return true;

            if (first == null || second == null || first.Length != second.Length) return false;

            var areSame = true;

            for (int i = 0; i < first.Length; i++) areSame &= (first[i] == second[i]);

            return areSame;
        }
    }
}