using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text.Json;

namespace BlockChain
{
    /// <summary>
    /// Represents a pool of transactions that can be added, updated, or retrieved.
    /// </summary>
    public class TransactionPool
    {
        private Dictionary<string, Transaction> transactions;

        public Dictionary<string, Transaction> Transactions => this.transactions;
        /// <summary>
        /// Gets the list of transactions in the pool.
        /// </summary>
        //public List<Transaction> Transactions => this.transactions;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionPool"/> class.
        /// </summary>
        public TransactionPool()
        {
            this.transactions = new Dictionary<string, Transaction>();
        }

        /// <summary>
        /// Updates an existing transaction or adds a new transaction to the pool.
        /// </summary>
        /// <param name="transaction">The transaction to be added or updated.</param>
        public void UpdateOrAddTransaction(Transaction transaction)
        {
            this.transactions[transaction.ID] = transaction;

            /*Transaction? transactionWithID = this.transactions.Find(t => t.ID == transaction.ID);

            if (transactionWithID != null)
            {
                this.transactions[this.transactions.IndexOf(transactionWithID)] = transaction;
            }
            else
            {
                this.transactions.Add(transaction);
            }*/
        }

        /// <summary>
        /// Verifies the signature of the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction to be verified.</param>
        /// <returns><c>true</c> if the transaction is valid; otherwise, <c>false</c>.</returns>
        public static bool VerifyTransaction(Transaction? transaction)
        {
            if (transaction == null) return false;
            return KeyPair.VerifySignature(KeyPair.GetECParameters(transaction.Input.Address),
                                                transaction.Input.Signature,
                                                TransactionOutput.GetOutputHash(transaction.Output));
        }

        /// <summary>
        /// Retrieves a transaction from the pool based on the address.
        /// </summary>
        /// <param name="address">The address associated with the transaction input.</param>
        /// <returns>The existing transaction if found; otherwise, <c>null</c>.</returns>
        public Transaction? ExistedTransaction(string address)
        {
            foreach (Transaction transaction in this.transactions.Values)
            {
                if (transaction.Input == null) continue;
                if (transaction.Input.Address.Equals(address)) return transaction;
            }
            return null;
        }
    }
}
