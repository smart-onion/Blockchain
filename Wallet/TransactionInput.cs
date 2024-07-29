using System.Security.Cryptography;

namespace BlockChain
{
    public class TransactionInput
    {
        double timestamp;
        int amount;
        string address;
        byte[] signature;

        public int Amount { get => this.amount; }
        public string Address { get => this.address; }
        public byte[] Signature { get => this.signature; }
        public TransactionInput() { }
        public TransactionInput(int amount, string address, byte[] signature)
        {
            this.timestamp = DateTime.Now.Ticks / TimeSpan.TicksPerMicrosecond;
            this.amount = amount;
            this.address = address;
            this.signature = signature;
        }

    }
}
