using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace BlockChain
{
    /// <summary>
    /// Represents a block in the blockchain.
    /// </summary>
    public class Block
    {
        private const long MINE_RATE = 3000;

        private readonly double timestamp;
        private readonly string lastHash;
        private string hash;
        private Transaction data;
        private double nonce;
        private int difficulty;

        /// <summary>
        /// Gets the timestamp of the block.
        /// </summary>
        public double Timestamp { get => this.timestamp; }

        /// <summary>
        /// Gets the hash of the previous block.
        /// </summary>
        public string LastHash { get => this.lastHash; }

        /// <summary>
        /// Gets the hash of the current block.
        /// </summary>
        public string Hash { get => this.hash; }

        /// <summary>
        /// Gets the data stored in the block.
        /// </summary>
        public Transaction Data { get => this.data; }

        /// <summary>
        /// Gets the nonce used for mining the block.
        /// </summary>
        public double Nonce { get => this.nonce; }

        /// <summary>
        /// Gets the difficulty of mining the block.
        /// </summary>
        public int Difficulty { get => this.difficulty; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Block"/> class.
        /// </summary>
        /// <param name="timestamp">The timestamp of the block.</param>
        /// <param name="lastHash">The hash of the previous block.</param>
        /// <param name="hash">The hash of the current block.</param>
        /// <param name="data">The data stored in the block.</param>
        /// <param name="nonce">The nonce used for mining the block.</param>
        /// <param name="difficulty">The difficulty of mining the block.</param>
        [JsonConstructor]
        public Block(double timestamp, string lastHash, string hash, Transaction data, double nonce, int difficulty)
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
            this.data = Transaction.GetGenesisTransaction();
            
            this.nonce = 0;
            this.difficulty = 4;
            this.hash = GenerateHash(timestamp, lastHash, data, nonce, difficulty);
        }

        /// <summary>
        /// Generates the genesis block.
        /// </summary>
        /// <returns>The genesis block.</returns>
        public static Block GenesisBlock()
        {
            return new Block();
        }

        /// <summary>
        /// Mines a new block.
        /// </summary>
        /// <param name="lastBlock">The previous block in the chain.</param>
        /// <param name="data">The data to store in the new block.</param>
        /// <returns>The mined block.</returns>
        public static Block MineBlock(Block lastBlock, Transaction data)
        {
            double nonce = 0;
            double timestamp;
            string hash;
            int difficulty;
            do
            {
                nonce++;
                timestamp = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
                difficulty = AdjustDifficulty(lastBlock, timestamp);
                hash = GenerateHash(timestamp, lastBlock.hash, data, nonce, difficulty);
            } while (hash.Substring(0, difficulty) != new string('0', difficulty));

            return new Block(timestamp, lastBlock.hash, hash, data, nonce, difficulty);
        }

        private static int AdjustDifficulty(Block lastBlock, double currentTime)
        {
            int diff = lastBlock.difficulty;
            int res = lastBlock.timestamp + MINE_RATE > currentTime ? diff + 1 : diff - 1;
            return Math.Clamp(res, 3, 6);
        }

        /// <summary>
        /// Generates a hash for the block.
        /// </summary>
        /// <param name="timestamp">The timestamp of the block.</param>
        /// <param name="lastHash">The hash of the previous block.</param>
        /// <param name="data">The data stored in the block.</param>
        /// <param name="nonce">The nonce used for mining the block.</param>
        /// <param name="difficulty">The difficulty of mining the block.</param>
        /// <returns>The generated hash.</returns>
        public static string GenerateHash(double timestamp, string lastHash, Transaction data, double nonce, int difficulty)
        {
            string temp = $"{timestamp}{lastHash}{data.Serialize()}{nonce}{difficulty}";
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(temp));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current block.
        /// </summary>
        /// <param name="obj">The object to compare with the current block.</param>
        /// <returns>true if the specified object is equal to the current block; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            Block? block = obj as Block;
            return block != null && 
                this.data.Serialize() == block.data.Serialize() && 
                this.timestamp == block.timestamp && 
                this.lastHash == block.lastHash && 
                this.hash == block.hash;
        }

        /// <summary>
        /// Returns a string that represents the current block.
        /// </summary>
        /// <returns>A string that represents the current block.</returns>
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
