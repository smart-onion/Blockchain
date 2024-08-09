namespace BlockChain
{
    /// <summary>
    /// The <see cref="Miner"/> class is responsible for validating transactions, mining new blocks,
    /// and adding them to the blockchain. It also handles rewarding the miner and broadcasting the updated chain.
    /// </summary>
    public class Miner
    {
        private Blockchain bc;
        private TransactionPool transactionPool;
        private Wallet wallet;
        private PubSub publisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="Miner"/> class with the specified blockchain,
        /// transaction pool, wallet, and publisher.
        /// </summary>
        /// <param name="bc">The blockchain to which mined blocks will be added.</param>
        /// <param name="tp">The transaction pool containing transactions to be validated and mined.</param>
        /// <param name="wallet">The miner's wallet for handling rewards.</param>
        /// <param name="publisher">The publisher responsible for broadcasting the updated blockchain.</param>
        public Miner(Blockchain bc, TransactionPool tp, Wallet wallet, PubSub publisher)
        {
            this.bc = bc;
            this.transactionPool = tp;
            this.wallet = wallet;
            this.publisher = publisher;
        }

        /// <summary>
        /// Mines new blocks by validating transactions from the transaction pool, adding them to the blockchain,
        /// and rewarding the miner. After mining, the updated blockchain is broadcasted and the transaction pool is cleared.
        /// </summary>
        public void Mine()
        {
            var validPool = transactionPool.ValidTransactions();

            if (validPool.Count <= 0) return;

            int reward = 0;

            foreach (var item in validPool)
            {
                var block = bc.AddBlock(item);
                reward += block.Difficulty;
            }

            bc.AddBlock(Transaction.RewardTransaction(wallet, Wallet.BlockchainWallet(), reward));

            publisher.BroadcastChain();

            transactionPool.Clear();
        }
    }
}
