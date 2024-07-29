using System.Security.Cryptography;

namespace BlockChain
{
    public class TransactionPool
    {
        List<Transaction> transactions;

        public List<Transaction> Transactions { get => this.transactions; }

        public TransactionPool()
        {
            this.transactions = new List<Transaction>();
        }

        public void UpdateOrAddTransaction(Transaction transaction)
        {
            Transaction? transactionWithID = this.transactions.Find(t => t.ID == transaction.ID);

            if (transactionWithID != null)
            {
                this.transactions[this.transactions.IndexOf(transactionWithID)] = transaction;
            }
            else
            {
                this.transactions.Add(transaction);
            }
                
        }

        public Transaction? ExistedTransaction(string address)
        {
            return this.transactions.Find(t => t.Input.Address.Equals(address));

        }
    }
}
