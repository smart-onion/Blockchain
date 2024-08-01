using System.Net;
using System.Text.Json;
using WebSocket4Net;

namespace BlockChain
{
    /// <summary>
    /// Represents the P2P client for connecting to peers and managing blockchain data.
    /// </summary>
    public class P2PClient
    {
        private P2PSharedData sharedData;

        /// <summary>
        /// Initializes a new instance of the <see cref="P2PClient"/> class with the specified shared data.
        /// </summary>
        /// <param name="sharedData">The shared data used across the P2P network.</param>
        public P2PClient(P2PSharedData sharedData)
        {
            this.sharedData = sharedData;
        }

        /// <summary>
        /// Connects to a peer at the specified address.
        /// </summary>
        /// <param name="address">The address of the peer to connect to.</param>
        public void ConnectToPeer(string address)
        {
            WebSocket socket = new WebSocket(address + "/");

            socket.Opened += async (sender, e) =>
            {
                Serilog.Log.Information($"Open socket on {address}");
            };

            socket.MessageReceived += (sender, e) =>
            {
                Data? data = JsonSerializer.Deserialize<Data>(e.Message);

                Serilog.Log.Information("Message received: " + data?.mt);
                switch (data?.mt)
                {
                    case MessageType.CHAIN:
                        Blockchain chain = JsonSerializer.Deserialize<Blockchain>(data.data);
                        Serilog.Log.Information("Server received new chain.");
                        sharedData.Bc.ReplaceChain(chain);
                        break;
                    case MessageType.TRANSACTION:
                        Transaction transaction = JsonSerializer.Deserialize<Transaction>(data.data);
                        Serilog.Log.Information("Server received new transaction.");
                        sharedData.TransactionPool.UpdateOrAddTransaction(transaction);
                        break;
                    default:
                        break;
                };
            };

            socket.Error += (sender, e) =>
            {
                Serilog.Log.Error("Error on socket");
            };

            socket.Closed += (sender, e) =>
            {
                Serilog.Log.Information($"Connection close on {address}");
            };

            socket.Open();
            sharedData.Sockets.Add(socket);
        }

        /// <summary>
        /// Connects to all peers listed in the shared data.
        /// </summary>
        public void ConnectToPeers()
        {
            foreach (string address in sharedData.Addresses)
            {
                ConnectToPeer(address);
            }
        }

        /// <summary>
        /// Sends the current blockchain data to the specified WebSocket.
        /// </summary>
        /// <param name="socket">The WebSocket to send the blockchain data to.</param>
        private void SendChain(WebSocket socket)
        {
            socket.Send(JsonSerializer.Serialize(new Data(MessageType.CHAIN, sharedData.Bc)));
        }

        /// <summary>
        /// Sends a transaction to the specified WebSocket.
        /// </summary>
        /// <param name="socket">The WebSocket to send the transaction to.</param>
        /// <param name="transaction">The transaction to be sent.</param>
        private void SendTransaction(WebSocket socket, Transaction transaction)
        {
            socket.Send(JsonSerializer.Serialize(new Data(MessageType.TRANSACTION, transaction)));
        }

        /// <summary>
        /// Broadcasts a transaction to all connected peers.
        /// </summary>
        /// <param name="transaction">The transaction to be broadcasted.</param>
        public void BroadcastTransactions(Transaction transaction)
        {
            sharedData.Sockets.ForEach(socket => SendTransaction(socket, transaction));
        }

        /// <summary>
        /// Synchronizes the blockchain data with all connected peers.
        /// </summary>
        public void SyncChains()
        {
            foreach (WebSocket socket in sharedData.Sockets)
            {
                SendChain(socket);
            }
        }
    }
}
