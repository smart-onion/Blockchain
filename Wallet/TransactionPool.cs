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

        /// <summary>
        /// Gets the dictionary of transactions in the pool.
        /// </summary>
        public Dictionary<string, Transaction> Transactions => this.transactions;

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
        }

        /// <summary>
        /// Retrieves a list of valid transactions from the transaction pool.
        /// </summary>
        /// <returns>A list of <see cref="Transaction"/> objects that have been verified as valid.</returns>
        public List<Transaction> ValidTransactions()
        {
            List<Transaction> validPool = new List<Transaction>();

            foreach (var transaction in this.transactions.Values)
            {
                if (Transaction.VerifyTransaction(transaction))
                {
                    validPool.Add(transaction);
                }
            }
            return validPool;
        }

        /// <summary>
        /// Clears all transactions from the transaction pool.
        /// </summary>
        public void Clear()
        {
            this.transactions.Clear();
        }

        /// <summary>
        /// Clears transactions from the transaction pool that are present in the specified blockchain.
        /// </summary>
        /// <param name="bc">The blockchain instance whose blocks' transactions will be removed from the pool.</param>
        public void Clear(Blockchain bc)
        {
             foreach (Block block in bc.Chain)
            {
                if (this.transactions.ContainsKey(block.Data.ID))
                {
                    this.transactions.Remove(block.Data.ID);
                }
            }
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
