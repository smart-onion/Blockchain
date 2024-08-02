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
    }
}
