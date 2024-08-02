using System.Text;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace BlockChain
{
    /// <summary>
    /// Represents an output for a blockchain transaction.
    /// </summary>
    public class TransactionOutput
    {
        private int amount;
        private string address;

        /// <summary>
        /// Gets or sets the amount associated with the transaction output.
        /// </summary>
        public int Amount { get => this.amount; set { this.amount = value; } }

        /// <summary>
        /// Gets the address associated with the transaction output.
        /// </summary>
        public string Address { get => this.address; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionOutput"/> class.
        /// </summary>
        public TransactionOutput() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionOutput"/> class with the specified amount and address.
        /// </summary>
        /// <param name="amount">The amount associated with the transaction output.</param>
        /// <param name="address">The address associated with the transaction output.</param>
        [JsonConstructor]
        public TransactionOutput(int amount, string address)
        {
            this.amount = amount;
            this.address = address;
        }

        /// <summary>
        /// Computes a SHA-256 hash of the list of transaction outputs.
        /// </summary>
        /// <param name="outputs">The list of transaction outputs to hash.</param>
        /// <returns>A byte array containing the SHA-256 hash of the transaction outputs.</returns>
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
