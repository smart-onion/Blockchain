using System.Text.Json;
using WebSocketSharp;
using WebSocketSharp.Server;
using Serilog;

namespace BlockChain
{
    /// <summary>
    /// Represents the behavior for handling WebSocket connections in the P2P network.
    /// </summary>
    internal class P2PBehavior : WebSocketBehavior
    {
        private P2PSharedData sharedData;

        /// <summary>
        /// Initializes a new instance of the <see cref="P2PBehavior"/> class with the specified shared data.
        /// </summary>
        /// <param name="sharedData">The shared data used across the P2P network.</param>
        public P2PBehavior(P2PSharedData sharedData)
        {
            this.sharedData = sharedData;
        }

        /// <summary>
        /// Handles errors that occur during WebSocket communication.
        /// </summary>
        /// <param name="e">The event arguments containing error information.</param>
        protected override void OnError(WebSocketSharp.ErrorEventArgs e)
        {
            Serilog.Log.Error("Ops from server");
        }

        /// <summary>
        /// Handles incoming messages from clients.
        /// </summary>
        /// <param name="e">The event arguments containing the received message.</param>
        protected override void OnMessage(MessageEventArgs e)
        {
            Data? data = JsonSerializer.Deserialize<Data>(e.Data);

            Serilog.Log.Information("Server side message: " + data.mt);
            switch (data?.mt)
            {
                case MessageType.CHAIN:
                    var chain = JsonSerializer.Deserialize<Blockchain>(data.data);
                    Serilog.Log.Information("Server received new chain.");
                    sharedData.Bc.ReplaceChain(chain);
                    break;
                case MessageType.TRANSACTION:
                    var transaction = JsonSerializer.Deserialize<Transaction>(data.data);
                    Serilog.Log.Information("Server received new transaction.");
                    sharedData.TransactionPool.UpdateOrAddTransaction(transaction);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Handles the event when a new WebSocket connection is opened.
        /// </summary>
        protected override void OnOpen()
        {
            try
            {
                Serilog.Log.Information("New connection established.");
                SendData(MessageType.CHAIN, sharedData.Bc);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error($"Error on connection open: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles the event when a WebSocket connection is closed.
        /// </summary>
        /// <param name="e">The event arguments containing information about the closed connection.</param>
        protected override void OnClose(CloseEventArgs e)
        {
            try
            {
                var clientAddress = Context?.UserEndPoint?.ToString();
                Serilog.Log.Information($"Connection closed: {clientAddress}");
            }
            catch (Exception ex)
            {
                Serilog.Log.Error($"Error on connection close: {ex.Message}");
            }
        }

        /// <summary>
        /// Sends data to the connected WebSocket client.
        /// </summary>
        /// <typeparam name="T">The type of the data being sent.</typeparam>
        /// <param name="type">The type of the message.</param>
        /// <param name="data">The data to be sent.</param>
        private void SendData(MessageType type, ISendable data)
        {
            Send(JsonSerializer.Serialize(new Data(type, data)));
        }
    }
}
