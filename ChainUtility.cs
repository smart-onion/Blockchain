using System.Security.Cryptography;
using System.Text;

namespace BlockChain
{
    public static class ChainUtility
    {
        public static string GetUniqueID()
        {
            Guid uuid = Guid.NewGuid();

            return uuid.ToString();
        }

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
