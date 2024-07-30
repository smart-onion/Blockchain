
using System.Security.Cryptography;
using System.Text;
using Serilog;
namespace BlockChain
{
    public class Wallet
    {
        int balance = 500;
        KeyPair keyPair;
        string publicKey;

        public int Balance { get => this.balance; }
        public KeyPair KeyPair { get => this.keyPair; }
        public string PublicKey { get => this.publicKey; }

        public Wallet()
        {
            this.keyPair = new KeyPair();
            this.publicKey = KeyPair.GetPublicKey(keyPair.Keys.ExportParameters(false));
        }

        public Transaction? CreateTransaction(string recipient, int amount, TransactionPool tranPool)
        {
            if (amount > this.balance)
            {
                Serilog.Log.Information($"Amount: {amount} exceed current balance: {this.balance}");
                return null;
            }
            Transaction? transaction = tranPool.ExistedTransaction(this.publicKey);

            if (transaction != null)
            {
                transaction.Update(this, recipient, amount);
            }
            else
            {
                transaction = Transaction.NewTransaction(this, recipient, amount);
                tranPool.UpdateOrAddTransaction(transaction);
            }

            return transaction;
        }

        public byte[] Sign(byte[] dataHash)
        {
            return this.keyPair.Keys.SignHash(dataHash);
        }

        public override string ToString()
        {
            return $"Wallet -\n\tPublicKey:{this.publicKey.ToString()}\n\tBalance: {this.balance}";
        }
    }

}
