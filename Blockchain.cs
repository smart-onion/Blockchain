using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlockChain
{
    public class Blockchain
    {
        private List<Block> chain;

        [JsonPropertyName("chain")]
        public List<Block> Chain { get => chain; }

        public Blockchain()
        {
            this.chain = new List<Block>();
            this.chain.Add(Block.GenesisBlock());
        }
        [JsonConstructorAttribute]
        public Blockchain(List<Block> chain)
        {
            this.chain = chain;
        }

        public Block AddBlock(string data)
        {
            Block lastBlock = this.chain[this.chain.Count - 1];
            Block newBlock = Block.MineBlock(lastBlock, data);
            this.chain.Add(newBlock);
            return newBlock;
        }

        public static bool ChainIsValid(Blockchain blockchain)
        {
            if (!blockchain.chain[0].Equals(Block.GenesisBlock())) return false;

            for (int i = 1; i < blockchain.chain.Count; i++)
            {
                Block block = blockchain.chain[i];
                Block lastBlock = blockchain.chain[i - 1];
                if (block.LastHash != lastBlock.Hash || block.Hash != Block.GenerateHash(block.Timestamp, block.LastHash, block.Data, block.Nonce))
                {
                    return false;
                }
            }

            return true;
        }

        public void ReplaceChain(Blockchain newChain)
        {
            if (newChain.chain.Count <= this.chain.Count)
            {
                Console.WriteLine("Received chain is not longer than the current chain.");
                return;
            }

            else if (!ChainIsValid(newChain))
            {
                Console.WriteLine("The received chain is not valid.");
                return;
            }
            Console.WriteLine("Replacing blockchain with new chain.");
            this.chain = newChain.chain;
        }
    }
}
