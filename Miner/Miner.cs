using Pipelines.Sockets.Unofficial.Arenas;

namespace BlockChain
{
    public class Miner
    {
        private Blockchain bc;
        private TransactionPool transactionPool;
        private Wallet wallet;
        private PubSub publisher;

        public Miner(Blockchain bc, TransactionPool tp, Wallet wallet, PubSub publisher)
        {
            this.bc = bc;
            this.transactionPool = tp;
            this.wallet = wallet;
            this.publisher = publisher;
        }

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

        private int GetReward()
        {
            return 1;
        }
    }
}
