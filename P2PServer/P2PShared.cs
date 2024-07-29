using WebSocket4Net;
using WebSocketSharp.Server;


namespace BlockChain
{
    public class P2PSharedData
    {

        public string MyAddress { get; set; }
        public WsAddresses Addresses { get; set; }
        public HashSet<string> ConnectedAddresses { get; set; }
        public Blockchain Bc { get; set; }
        public List<WebSocket> Sockets { get; set; }

        public WebSocketServer Server { get; set; }
        public P2PClient Client { get; set; }

        public P2PSharedData(Blockchain bc, List<string> addresses, string myAddress)
        {
            this.MyAddress = myAddress;
            this.Addresses = new WsAddresses(addresses);
            this.ConnectedAddresses = new HashSet<string>();
            this.Sockets = new List<WebSocket>();
            this.Bc = bc;

        }


    }
}
