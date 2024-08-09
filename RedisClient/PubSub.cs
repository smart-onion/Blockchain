using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlockChain
{
    /// <summary>
    /// The <see cref="PubSub"/> class manages the publish/subscribe (Pub/Sub) mechanism for broadcasting
    /// blockchain data and transactions using Redis. It handles the communication between nodes in the blockchain network.
    /// </summary>
    public class PubSub
    {
        /// <summary>
        /// A struct to encapsulate data being sent over the channels, including the sender's ID and the serialized data.
        /// </summary>
        private struct DataToSend
        {
            /// <summary>
            /// Gets or sets the unique ID of the sender.
            /// </summary>
            public string ID { get; set; }

            /// <summary>
            /// Gets or sets the serialized data to be sent.
            /// </summary>
            public string Data { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="DataToSend"/> struct with the sender's ID and data to send.
            /// </summary>
            /// <param name="ID">The unique ID of the sender.</param>
            /// <param name="Data">The data to send, which is serialized to a string.</param>
            public DataToSend(string ID, ISendable Data)
            {
                this.ID = ID;
                this.Data = Data.Serialize();
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="DataToSend"/> struct with the sender's ID and serialized data.
            /// </summary>
            /// <param name="ID">The unique ID of the sender.</param>
            /// <param name="Data">The serialized data string.</param>
            [JsonConstructor]
            public DataToSend(string ID, string Data)
            {
                this.ID = ID;
                this.Data = Data;
            }
        }

        static readonly ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
            "redis-15621.c1.us-east1-2.gce.redns.redis-cloud.com:15621,password=nEQpXj3tUyY1MtqhuiCWfYkyiXlwIKE8"
        );
        private ISubscriber publisher = redis.GetSubscriber();
        private ISubscriber subscriber = redis.GetSubscriber();
        private Blockchain bc;
        private TransactionPool transactionPool;
        private string id;

        /// <summary>
        /// Gets the Redis publisher used for broadcasting messages to other nodes.
        /// </summary>
        public ISubscriber Publisher { get => this.publisher; }

        /// <summary>
        /// Gets the Redis subscriber used for receiving messages from other nodes.
        /// </summary>
        public ISubscriber Subscriber { get => this.subscriber; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PubSub"/> class with the specified blockchain, transaction pool, and Redis subscribers.
        /// </summary>
        /// <param name="bc">The blockchain instance to manage and broadcast.</param>
        /// <param name="transactionPool">The transaction pool containing transactions to be broadcasted.</param>
        public PubSub(Blockchain bc, TransactionPool transactionPool)
        {
            id = ChainUtility.GetUniqueID();
            this.bc = bc;
            this.transactionPool = transactionPool;
            SubscribeToChannels();
            PublishToNewSubscriber();
        }

        /// <summary>
        /// Subscribes to the necessary channels for receiving blockchain and transaction updates.
        /// </summary>
        private void SubscribeToChannels()
        {
            subscriber.Subscribe(Channels.CHAIN, OnChainReceived);
            subscriber.Subscribe(Channels.TRANSACTION, OnTransactionReceived);
        }

        /// <summary>
        /// Handles the receipt of a transaction message, updating the transaction pool with the received transaction.
        /// </summary>
        /// <param name="channel">The Redis channel from which the message was received.</param>
        /// <param name="message">The serialized message received.</param>
        private void OnTransactionReceived(RedisChannel channel, RedisValue message)
        {
            var data = JsonSerializer.Deserialize<DataToSend>(message);
            if (data.ID == this.id) return;
            Transaction transaction = JsonSerializer.Deserialize<Transaction>(data.Data);
            transactionPool.UpdateOrAddTransaction(transaction);
        }

        /// <summary>
        /// Handles the receipt of a blockchain message, replacing the current blockchain with the received one if valid.
        /// </summary>
        /// <param name="channel">The Redis channel from which the message was received.</param>
        /// <param name="message">The serialized message received.</param>
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

        /// <summary>
        /// Publishes a message to a specified channel.
        /// </summary>
        /// <param name="channel">The name of the channel to which the message will be published.</param>
        /// <param name="message">The message to publish, wrapped in a <see cref="DataToSend"/> struct.</param>
        private void Publish(string channel, DataToSend message)
        {
            var messageToSend = JsonSerializer.Serialize(message);
            publisher.Publish(channel, messageToSend);
        }

        /// <summary>
        /// Broadcasts the current state of the blockchain to all subscribers.
        /// </summary>
        public void BroadcastChain()
        {
            Publish(Channels.CHAIN, new DataToSend(id, bc));
        }

        /// <summary>
        /// Broadcasts a transaction to all subscribers.
        /// </summary>
        /// <param name="transaction">The transaction to broadcast.</param>
        public void BroadcastTransaction(Transaction transaction)
        {
            Publish(Channels.TRANSACTION, new DataToSend(id, transaction));
        }

        /// <summary>
        /// Publishes a message indicating a new subscriber has joined and broadcasts the current chain to the new subscriber.
        /// </summary>
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
