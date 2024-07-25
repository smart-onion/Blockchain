using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace BlockChain
{
    public class Block
    {
        private const int difficulty = 4;

        private readonly double timestamp;
        private readonly string lastHash;
        private string hash;
        private string data;
        private double nonce;

        public double Timestamp { get => this.timestamp; }
        public string LastHash { get => this.lastHash; }
        public string Hash { get => this.hash; }
        public string Data { get => this.data; }
        public double Nonce { get => this.nonce; }

        [JsonConstructor]
        public Block(double timestamp, string lastHash, string hash, string data, double nonce) 
        {
            this.timestamp = timestamp;
            this.lastHash = lastHash;
            this.hash = hash;
            this.data = data;
            this.nonce = nonce;
        
        }
        private Block()
        {
            this.timestamp = 0;
            this.lastHash = "-----";
            this.data = new String("");
            this.nonce = 0;
            this.hash = GenerateHash(timestamp, lastHash, data, nonce);

        }

        public static Block GenesisBlock()
        {
            return new Block();
        }

        public static Block MineBlock(Block lastBlock, string data)
        {
            double nonce = 0;
            double timestamp;
            string hash;
            do
            {
                nonce++;
                timestamp = DateTime.Now.Ticks / TimeSpan.TicksPerMicrosecond;
                hash = Block.GenerateHash(timestamp, lastBlock.hash, data, nonce);
            } while (hash.Substring(0, difficulty) != new StringBuilder("0".Length * difficulty).Insert(0, "0", difficulty).ToString());

            return new Block(timestamp, lastBlock.hash, hash, data, nonce);
        }

        public static string GenerateHash(double timestamp, string lastHash, string data, double nonce) 
        {
            string temp = $"{timestamp}{lastHash}{data}{nonce}";
            // Create a SHA256 object
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Compute the hash of the input string
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(temp));

                // Convert the byte array to a hexadecimal string
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }

        }

        public override bool Equals(object? obj)
        {
            Block? block = obj as Block;
            if (block == null) return false;

            if (block.data == this.data && this.timestamp == block.timestamp && this.lastHash == block.lastHash && this.hash == block.hash)
            {
                return true;
            }
            return false;
        }
        public override string ToString()
        {
            return "Block:\n" +
                $"\tTimestamp : {this.timestamp}\n" +
                $"\tLast Hash : {this.lastHash}\n" +
                $"\tHash      : {this.hash}\n" +
                $"\tData      : {this.data}\n";
        }
    }
}
