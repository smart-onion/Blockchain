using System.Text.Json.Serialization;

namespace BlockChain
{
    public enum MessageType
    {
        CHAIN,
        TRANSACTION,
        ADDRESS,
    }
    public class Data
    {
        public MessageType mt { get; set; }
        public ISendable data { get; set; }
        [JsonConstructor]
        public Data(MessageType mt, ISendable data)
        {
            this.mt = mt;
            this.data = data;
        }
    }
}
