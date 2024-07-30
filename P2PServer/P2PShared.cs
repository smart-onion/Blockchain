using WebSocket4Net;
using WebSocketSharp.Server;


namespace BlockChain
{
    public class P2PSharedData
    {

        public string MyAddress { get; set; }
        public List<string> Addresses { get; set; }
        public Blockchain Bc { get; set; }
        public TransactionPool TransactionPool { get; set; }
        public List<WebSocket> Sockets { get; set; }

        public P2PSharedData(Blockchain bc, TransactionPool tp, List<string> addresses, string myAddress)
        {
            this.MyAddress = myAddress;
            this.Sockets = new List<WebSocket>();
            this.Bc = bc;
            this.TransactionPool = tp;
            this.Addresses = addresses;
        }


    }
}
