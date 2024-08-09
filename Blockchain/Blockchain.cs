using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Serilog;

namespace BlockChain
{
    /// <summary>
    /// Represents a blockchain consisting of a series of blocks.
    /// </summary>
    public class Blockchain : ISendable
    {
        /// <summary>
        /// Represents a method that will handle a successful operation or event.
        /// </summary>
        public delegate void SuccessHandler();
        private List<Block> chain;

        /// <summary>
        /// Gets the list of blocks in the blockchain.
        /// </summary>
        public List<Block> Chain => chain;

        /// <summary>
        /// Initializes a new instance of the <see cref="Blockchain"/> class with a genesis block.
        /// </summary>
        public Blockchain()
        {
            this.chain = new List<Block>();
            this.chain.Add(Block.GenesisBlock());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blockchain"/> class with a specified chain of blocks.
        /// </summary>
        /// <param name="chain">The list of blocks to initialize the blockchain with.</param>
        [JsonConstructor]
        public Blockchain(List<Block> chain)
        {
            this.chain = chain;
        }

        /// <summary>
        /// Adds a new block with the specified data to the blockchain.
        /// </summary>
        /// <param name="data">The data to be added to the new block.</param>
        /// <returns>The newly created block.</returns>
        public Block AddBlock(Transaction data)
        {
            Block lastBlock = this.chain[this.chain.Count - 1];
            Block newBlock = Block.MineBlock(lastBlock, data);
            this.chain.Add(newBlock);
            return newBlock;
        }

        /// <summary>
        /// Checks if the provided blockchain is valid.
        /// </summary>
        /// <param name="blockchain">The blockchain to be validated.</param>
        /// <returns><c>true</c> if the blockchain is valid; otherwise, <c>false</c>.</returns>
        public static bool ChainIsValid(Blockchain blockchain)
        {
            if (!blockchain.chain[0].Equals(Block.GenesisBlock())) return false;

            for (int i = 1; i < blockchain.chain.Count; i++)
            {
                Block block = blockchain.chain[i];
                Block lastBlock = blockchain.chain[i - 1];
                if (block.LastHash != lastBlock.Hash || block.Hash != Block.GenerateHash(block.Timestamp, block.LastHash, block.Data, block.Nonce, block.Difficulty))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Replaces the current blockchain with a new one if it is valid and longer than the current chain.
        /// </summary>
        /// <param name="newChain">The new blockchain to replace the current one.</param>
        /// <returns><c>true</c> if the blockchain was successfully replaced; otherwise, <c>false</c>.</returns>
        public bool ReplaceChain(Blockchain newChain, SuccessHandler? onSuccess)
        {
            if (newChain.chain.Count <= this.chain.Count)
            {
                Serilog.Log.Information("Received chain is not longer than the current chain.");
                return false;
            }
            else if (!ChainIsValid(newChain))
            {
                Serilog.Log.Information("The received chain is not valid.");
                return false;
            }

            Serilog.Log.Information("Replacing blockchain with new chain.");
            this.chain = newChain.chain;

            onSuccess?.Invoke();

            return true;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current blockchain.
        /// </summary>
        /// <param name="obj">The object to compare with the current blockchain.</param>
        /// <returns><c>true</c> if the specified object is equal to the current blockchain; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            Blockchain? b = (Blockchain?)obj;
            if (this.chain.Equals(b?.chain))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Serializes the blockchain to a JSON string.
        /// </summary>
        /// <returns>A JSON string representation of the blockchain.</returns>
        public string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
