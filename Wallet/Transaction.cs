using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace BlockChain
{
    public class Transaction : ISendable
    {
        private string id;
        TransactionInput input;
        List<TransactionOutput> output;

        public string ID { get => this.id; }
        public List<TransactionOutput> Output { get => this.output; }
        public TransactionInput Input { get => this.input; set { this.input = value; } }
        public Transaction()
        {
            this.id = ChainUtility.GetUniqueID();
            this.input = new TransactionInput();
            this.output = new List<TransactionOutput>();
        }

        [JsonConstructor]
        protected Transaction(string id, TransactionInput input, List<TransactionOutput> output)
        {
            this.id = id;
            this.input = input;
            this.output = output;
        }

        public Transaction? Update(Wallet senderWallet, string recipient, int amount)
        {
            var senderOutput = this.output.Find(a => a.Address.Equals(senderWallet.PublicKey));

            if (amount > senderWallet.Balance)
            {
                Console.WriteLine($"Amount: {amount} exceed balance.");
                return null;
            }

            senderOutput.Amount = senderOutput.Amount - amount;
            this.output.Add(new TransactionOutput(amount, recipient));
            Transaction.SignTransaction(this, senderWallet);

            return this;
        }

        public static Transaction? NewTransaction(Wallet senderWallet, string recipient, int amount)
        {
            Transaction transaction = new Transaction();
            if (amount > senderWallet.Balance)
            {
                Console.WriteLine($"Amount: {amount} exceeds balance");
                return null;
            }

            transaction.Output.Add(new TransactionOutput(senderWallet.Balance - amount, senderWallet.PublicKey));
            transaction.Output.Add(new TransactionOutput(amount, recipient));
            Transaction.SignTransaction(transaction, senderWallet);
            return transaction;
        }

        public static void SignTransaction(Transaction? transaction, Wallet senderWallet)
        {
            if (transaction == null) return;

            transaction.Input = new TransactionInput(senderWallet.Balance, 
                                                     senderWallet.PublicKey, 
                                                     senderWallet.Sign(TransactionOutput.GetOutputHash(transaction.Output)));
        }

        public static bool VerifyTransaction(Transaction? transaction)
        {
            if (transaction == null) return false;
            return KeyPair.VerifySignature(KeyPair.GetECParameters(transaction.Input.Address), 
                                                transaction.Input.Signature, 
                                                TransactionOutput.GetOutputHash(transaction.Output));
        }
    }
}
