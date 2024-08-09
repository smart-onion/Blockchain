using System.Security.Cryptography;
using System.Text;

namespace BlockChain
{
    /// <summary>
    /// Provides utility methods for generating unique identifiers and computing cryptographic hashes.
    /// </summary>
    public static class ChainUtility
    {
        /// <summary>
        /// Generates a new unique identifier using GUID.
        /// </summary>
        /// <returns>A string representing the unique identifier.</returns>
        public static string GetUniqueID()
        {
            Guid uuid = Guid.NewGuid();
            return uuid.ToString();
        }

        /// <summary>
        /// Computes the SHA-256 cryptographic hash of the given input string.
        /// </summary>
        /// <param name="data">The input string to hash.</param>
        /// <returns>A byte array containing the SHA-256 hash of the input string.</returns>
        public static byte[] GetCryptoHash(string data)
        {
            // Create a SHA256 object
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Compute the hash of the input string
                return sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));
            }
        }
    }
}
