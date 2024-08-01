using System.Text.Json.Serialization;

namespace BlockChain
{    
     /// <summary>
     /// Represents the type of message in the p2p network.
     /// </summary>
    public enum MessageType
    {
        CHAIN,
        TRANSACTION,
        ADDRESS,
    }
    /// <summary>
    /// Represents data in the blockchain.
    /// </summary>
    public class Data
    {
        public MessageType mt { get; set; }
        public string data { get; set; }

        [JsonConstructor]
        public Data(MessageType mt, string data)
        {
            this.mt = mt;
            this.data = data;
        }
        public Data(MessageType mt, ISendable data)
        {
            this.mt = mt;
            this.data = data.Serialize();
        }
    }
}
