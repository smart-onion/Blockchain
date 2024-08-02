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
        private string id;
        public ISubscriber Publisher { get => this.publisher; }
        public ISubscriber Subscriber { get => this.subscriber; }
        public PubSub(Blockchain bc)
        {
            id = ChainUtility.GetUniqueID();
            this.bc = bc;
            SubscribeToChannels();
            PublishToNewSubscriber();
        }

        private void SubscribeToChannels()
        {
            subscriber.Subscribe(Channels.CHAIN, OnChainReceived);
        }

        private void OnConnected(RedisChannel channel, RedisValue message)
        {
        }
        private void OnChainReceived(RedisChannel channel, RedisValue message)
        {
            var data = JsonSerializer.Deserialize<DataToSend>(message);
            if (data.ID == this.id) return;

            Blockchain bc = JsonSerializer.Deserialize<Blockchain>(data.Data);
            this.bc.ReplaceChain(bc);
            Serilog.Log.Information("New chain received.");

        }

        private void Publish(string channel, DataToSend message)
        {
            var messageToSend = message.Data;
            publisher.Publish(channel, messageToSend);
        }

        public void BroadcastChain()
        {
             Publish(Channels.CHAIN, new DataToSend(id, bc));
        }

        private void PublishToNewSubscriber()
        {
            if (Channels.ROOT.Equals("ROOT"))
            {
                subscriber.Subscribe(Channels.NEW_CLIENT);

                Serilog.Log.Information("A new subscriber has connected");
                BroadcastChain();

            }
            publisher.Publish(Channels.NEW_CLIENT, Channels.NEW_CLIENT);
            
        }
    }
}
