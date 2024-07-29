using System.Text;
using System.Security.Cryptography;


namespace BlockChain
{
    public class TransactionOutput
    {
        int amount;
        string address;

        public int Amount { get => this.amount; set { this.amount = value; } }
        public string Address { get => this.address; }

        public TransactionOutput()
        {

        }

        public TransactionOutput(int amount, string address)
        {
            this.amount = amount;
            this.address = address;
        }

        public static byte[] GetOutputHash(List<TransactionOutput> outputs)
        {
            string temp = "";
            foreach (var item in outputs)
            {
                temp += item.amount;
                temp += item.address;
            }
            // Create a SHA256 object
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Compute the hash of the input string
                return sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(temp));
            }
        }
    }
}
