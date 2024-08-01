using WebSocket4Net;
using WebSocketSharp.Server;

namespace BlockChain
{
    /// <summary>
    /// Represents the shared data for the peer-to-peer (P2P) network.
    /// </summary>
    public class P2PSharedData
    {
        /// <summary>
        /// Gets or sets the address of the current node.
        /// </summary>
        public string MyAddress { get; set; }

        /// <summary>
        /// Gets or sets the list of addresses of other nodes in the network.
        /// </summary>
        public List<string> Addresses { get; set; }

        /// <summary>
        /// Gets or sets the blockchain associated with the current node.
        /// </summary>
        public Blockchain Bc { get; set; }

        /// <summary>
        /// Gets or sets the transaction pool associated with the current node.
        /// </summary>
        public TransactionPool TransactionPool { get; set; }

        /// <summary>
        /// Gets or sets the list of WebSocket connections to other nodes.
        /// </summary>
        public List<WebSocket> Sockets { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="P2PSharedData"/> class with the specified blockchain, transaction pool, addresses, and current node address.
        /// </summary>
        /// <param name="bc">The blockchain associated with the current node.</param>
        /// <param name="tp">The transaction pool associated with the current node.</param>
        /// <param name="addresses">The list of addresses of other nodes in the network.</param>
        /// <param name="myAddress">The address of the current node.</param>
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
