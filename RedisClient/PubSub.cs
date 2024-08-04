using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace BlockChain
{

    public class PubSub
    {
        private struct DataToSend
        {
            public string ID { get; set; }
            public string Data { get; set; }

            public DataToSend(string ID, ISendable Data)
            {
                this.ID = ID;
                this.Data = Data.Serialize();
            }

            [JsonConstructor]
            public DataToSend(string ID, string Data)
            {
                this.ID = ID;
                this.Data = Data;
            }
        }

        static readonly ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("redis-15621.c1.us-east1-2.gce.redns.redis-cloud.com:15621,password=nEQpXj3tUyY1MtqhuiCWfYkyiXlwIKE8");
        private ISubscriber publisher = redis.GetSubscriber();
        private ISubscriber subscriber = redis.GetSubscriber();
        private Blockchain bc;
        private TransactionPool transactionPool;
        private string id;
        public ISubscriber Publisher { get => this.publisher; }
        public ISubscriber Subscriber { get => this.subscriber; }
        public PubSub(Blockchain bc, TransactionPool transactionPool)
        {
            id = ChainUtility.GetUniqueID();
            this.bc = bc;
            this.transactionPool = transactionPool;
            SubscribeToChannels();
            PublishToNewSubscriber();
        }

        private void SubscribeToChannels()
        {
            subscriber.Subscribe(Channels.CHAIN, OnChainReceived);
            subscriber.Subscribe(Channels.TRANSACTION, OnTransactionReceived);
        }

        private void OnTransactionReceived(RedisChannel channel, RedisValue message)
        {
            var data = JsonSerializer.Deserialize<DataToSend>(message);
            if (data.ID == this.id) return;
            Transaction transaction = JsonSerializer.Deserialize<Transaction>(data.Data);
            transactionPool.UpdateOrAddTransaction(transaction);
        }
        private void OnChainReceived(RedisChannel channel, RedisValue message)
        {
            var data = JsonSerializer.Deserialize<DataToSend>(message);
            if (data.ID == this.id) return;

            Blockchain bc = JsonSerializer.Deserialize<Blockchain>(data.Data);
            this.bc.ReplaceChain(bc, () =>
            {
                transactionPool.Clear(this.bc);
            });
            Serilog.Log.Information("New chain received.");

        }

        private void Publish(string channel, DataToSend message)
        {
            var messageToSend = JsonSerializer.Serialize(message);
            publisher.Publish(channel, messageToSend);
        }



        public void BroadcastChain()
        {
             Publish(Channels.CHAIN, new DataToSend(id, bc));
        }

        public void BroadcastTransaction(Transaction transaction)
        {
            Publish(Channels.TRANSACTION, new DataToSend(id, transaction));
        }

        private void PublishToNewSubscriber()
        {
            if (Channels.ROOT.Equals("ROOT"))
            {
                subscriber.Subscribe(Channels.NEW_CLIENT, (ch, msg) =>
                {
                    Serilog.Log.Information("A new subscriber has connected");
                    BroadcastChain();
                });

            }
            publisher.Publish(Channels.NEW_CLIENT, Channels.NEW_CLIENT);
            
        }
    }
}
