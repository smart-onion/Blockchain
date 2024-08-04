using System.Security.Cryptography;
using System.Text;
using Serilog;

namespace BlockChain
{
    /// <summary>
    /// Represents a wallet in the blockchain with a balance and cryptographic key pair.
    /// </summary>
    public class Wallet
    {
        private static int startBalance = 500;

        private int balance;
        private KeyPair keyPair;
        private string publicKey;

        /// <summary>
        /// Gets the current balance of the wallet.
        /// </summary>
        public int Balance => this.balance;

        /// <summary>
        /// Gets the key pair associated with the wallet.
        /// </summary>
        public KeyPair KeyPair => this.keyPair;

        /// <summary>
        /// Gets the public key of the wallet.
        /// </summary>
        public string PublicKey => this.publicKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="Wallet"/> class with a default balance and a new key pair.
        /// </summary>
        public Wallet()
        {
            this.balance = Wallet.startBalance;
            this.keyPair = new KeyPair();
            this.publicKey = KeyPair.GetPublicKey(keyPair.Keys.ExportParameters(false));
        }

        /// <summary>
        /// Creates a transaction from the wallet to a specified recipient.
        /// </summary>
        /// <param name="recipient">The recipient's address.</param>
        /// <param name="amount">The amount to be transferred.</param>
        /// <param name="tranPool">The transaction pool to manage the transactions.</param>
        /// <returns>The created transaction if successful; otherwise, <c>null</c>.</returns>
        public Transaction? CreateTransaction(string recipient, int amount, TransactionPool tranPool, Blockchain bc)
        {
            this.balance = CalculateBalance(bc, this.publicKey);

            if (amount > this.balance)
            {
                Serilog.Log.Information($"Amount: {amount} exceeds current balance: {this.balance}");
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

        public int CalculateBalance(Blockchain bc, string address)
        {
            bool hasConductedTransaction = false;
            int outputTotal = 0;

            for (int i = bc.Chain.Count - 1; i > 0; i--)
            {
                var block = bc.Chain[i];

                foreach (var item in block.Data.Output)
                {

                    if (item.Address == address)
                    {
                        outputTotal += item.Amount;
                    }
                }

                if (block.Data.Input.Address == address)
                {
                    return outputTotal;
                }

            }

            return startBalance + outputTotal;
        }

        /// <summary>
        /// Signs the given data hash using the wallet's key pair.
        /// </summary>
        /// <param name="dataHash">The hash of the data to be signed.</param>
        /// <returns>The digital signature as a byte array.</returns>
        public byte[] Sign(byte[] dataHash)
        {
            return this.keyPair.Keys.SignHash(dataHash);
        }


        public static Wallet BlockchainWallet()
        {
            return new Wallet();
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Wallet"/> object.
        /// </summary>
        /// <returns>A string representation of the wallet, including the public key and balance.</returns>
        public override string ToString()
        {
            return $"Wallet -\n\tPublicKey: {this.publicKey}\n\tBalance: {this.balance}";
        }
    }
}
