using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlockChain
{
    /// <summary>
    /// Represents a blockchain transaction.
    /// </summary>
    public class Transaction : ISendable
    {
        private string id;
        TransactionInput input;
        List<TransactionOutput> output;

        /// <summary>
        /// Gets the unique identifier of the transaction.
        /// </summary>
        public string ID { get => this.id; }

        /// <summary>
        /// Gets the list of transaction outputs.
        /// </summary>
        public List<TransactionOutput> Output { get => this.output; }

        /// <summary>
        /// Gets or sets the transaction input.
        /// </summary>
        public TransactionInput Input { get => this.input; set { this.input = value; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transaction"/> class with a unique ID and empty input and output lists.
        /// </summary>
        public Transaction()
        {
            this.id = ChainUtility.GetUniqueID();
            this.input = new TransactionInput();
            this.output = new List<TransactionOutput>();
        }

        // <summary>
        /// Initializes a new instance of the <see cref="Transaction"/> class with specified ID, input, and output.
        /// </summary>
        /// <param name="id">The unique identifier of the transaction.</param>
        /// <param name="input">The input details of the transaction.</param>
        /// <param name="output">The list of transaction outputs.</param>
        [JsonConstructor]
        protected Transaction(string id, TransactionInput input, List<TransactionOutput> output)
        {
            this.id = id;
            this.input = input;
            this.output = output;
        }

        public static Transaction GetGenesisTransaction()
        {
            Transaction tr = new Transaction();
            tr.id = "genesis-transaction";
            return tr;
        }

        /// <summary>
        /// Updates the transaction with a new amount and recipient, and signs it with the sender's wallet.
        /// </summary>
        /// <param name="senderWallet">The sender's wallet.</param>
        /// <param name="recipient">The recipient's address.</param>
        /// <param name="amount">The amount to be updated.</param>
        /// <returns>The updated transaction if successful; otherwise, <c>null</c>.</returns>
        public Transaction? Update(Wallet senderWallet, string recipient, int amount)
        {
            var senderOutput = this.output.Find(a => a.Address.Equals(senderWallet.PublicKey));

            if (amount > senderWallet.Balance)
            {
                Serilog.Log.Information($"Amount: {amount} exceed balance.");
                return null;
            }

            senderOutput.Amount = senderOutput.Amount - amount;
            this.output.Add(new TransactionOutput(amount, recipient));
            Transaction.SignTransaction(this, senderWallet);

            return this;
        }

        /// <summary>
        /// Creates a new transaction with the specified amount and recipient, and signs it with the sender's wallet.
        /// </summary>
        /// <param name="senderWallet">The sender's wallet.</param>
        /// <param name="recipient">The recipient's address.</param>
        /// <param name="amount">The amount to be transferred.</param>
        /// <returns>The new transaction if successful; otherwise, <c>null</c>.</returns>

        public static Transaction? NewTransaction(Wallet senderWallet, string recipient, int amount)
        {
            Transaction transaction = new Transaction();
            if (amount > senderWallet.Balance)
            {
                Serilog.Log.Information($"Amount: {amount} exceeds balance");
                return null;
            }

            transaction.Output.Add(new TransactionOutput(senderWallet.Balance - amount, senderWallet.PublicKey));
            transaction.Output.Add(new TransactionOutput(amount, recipient));
            Transaction.SignTransaction(transaction, senderWallet);
            return transaction;
        }

        public static Transaction RewardTransaction(Wallet minerWallet, Wallet blockchainWallet, int amount)
        {
            Transaction transaction = new Transaction();
            transaction.Output.Add(new TransactionOutput(amount, minerWallet.PublicKey));
            Transaction.SignTransaction(transaction, blockchainWallet);
            return transaction;
        }

        /// <summary>
        /// Signs the specified transaction with the sender's wallet.
        /// </summary>
        /// <param name="transaction">The transaction to be signed.</param>
        /// <param name="senderWallet">The sender's wallet.</param>
        public static void SignTransaction(Transaction? transaction, Wallet senderWallet)
        {
            if (transaction == null) return;

            transaction.Input = new TransactionInput(senderWallet.Balance, 
                                                     senderWallet.PublicKey, 
                                                     senderWallet.Sign(TransactionOutput.GetOutputHash(transaction.Output)));
        }

         /// <summary>
        /// Verifies the signature of the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction to be verified.</param>
        /// <returns><c>true</c> if the transaction is valid; otherwise, <c>false</c>.</returns>
        public static bool VerifyTransaction(Transaction? transaction)
        {
            if (transaction == null) return false;

            int total = 0;

            foreach (var item in transaction.Output)
            {
                total += item.Amount;
            }

            if (transaction.Input.Amount != total) return false;


            return KeyPair.VerifySignature(KeyPair.GetECParameters(transaction.Input.Address),
                                                transaction.Input.Signature,
                                                TransactionOutput.GetOutputHash(transaction.Output));
        }

        /// <summary>
        /// Serializes the transaction to a JSON string.
        /// </summary>
        /// <returns>A JSON string representation of the transaction.</returns>
        public string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }

    }
}
