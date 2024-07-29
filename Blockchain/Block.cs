using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace BlockChain
{
    public class Block
    {
        private const long MINE_RATE = 3000 * 10000;

        private readonly double timestamp;
        private readonly string lastHash;
        private string          hash;
        private string          data;
        private double          nonce;
        private int             difficulty;

        public double Timestamp     { get => this.timestamp; }
        public string LastHash      { get => this.lastHash; }
        public string Hash          { get => this.hash; }
        public string Data          { get => this.data; }
        public double Nonce         { get => this.nonce; }
        public int    Difficulty    { get => this.difficulty; }

        [JsonConstructor]
        public Block(double timestamp, string lastHash, string hash, string data, double nonce, int difficulty) 
        {
            this.timestamp = timestamp;
            this.lastHash = lastHash;
            this.hash = hash;
            this.data = data;
            this.nonce = nonce;
            this.difficulty = difficulty;
        
        }
        private Block()
        {
            this.timestamp = 0;
            this.lastHash = "-----";
            this.data = new String("");
            this.nonce = 0;
            this.difficulty = 4;
            this.hash = GenerateHash(timestamp, lastHash, data, nonce, difficulty);

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
            int difficulty;
            do
            {
                nonce++;
                timestamp = DateTime.Now.Ticks / TimeSpan.TicksPerMicrosecond;

                difficulty = AdjustDifficulty(lastBlock, timestamp);

                hash = Block.GenerateHash(timestamp, lastBlock.hash, data, nonce, difficulty);
            } while (hash.Substring(0, difficulty) != new StringBuilder("0".Length * difficulty).Insert(0, "0", difficulty).ToString());

            return new Block(timestamp, lastBlock.hash, hash, data, nonce, difficulty);
        }

        private static int AdjustDifficulty(Block lastBlock, double currentTime)
        {
            int diff = lastBlock.difficulty;

            int res = lastBlock.timestamp + MINE_RATE > currentTime ? diff + 1 : diff - 1;

            if (res > 6) return 6;
            else if (res < 3) return 3;

            return res;
        }

        public static string GenerateHash(double timestamp, string lastHash, string data, double nonce, int difficulty) 
        {
            string temp = $"{timestamp}{lastHash}{data}{nonce}{difficulty}";
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
                $"\tData      : {this.data}\n" +
                $"\tDifficulty: {this.difficulty}\n" +
                $"\tNonce     : {this.nonce}\n";
        }
    }
}
