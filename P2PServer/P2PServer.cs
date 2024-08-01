using System.Text.Json;
using WebSocketSharp.Server;
using Serilog;

namespace BlockChain
{
    /// <summary>
    /// Represents the P2P server for managing WebSocket connections and broadcasting data to clients.
    /// </summary>
    public class P2PServer
    {
        private P2PSharedData sharedData;
        private WebSocketServer server;

        /// <summary>
        /// Initializes a new instance of the <see cref="P2PServer"/> class with the specified shared data.
        /// </summary>
        /// <param name="sharedData">The shared data used across the P2P network.</param>
        public P2PServer(P2PSharedData sharedData)
        {
            this.sharedData = sharedData;
        }

        /// <summary>
        /// Starts listening for incoming WebSocket connections on the server.
        /// </summary>
        public void Listen()
        {
            server = new WebSocketServer($"ws://{sharedData.MyAddress}");
            server.AddWebSocketService<P2PBehavior>("/", () => new P2PBehavior(sharedData));

            server.Start();
            Serilog.Log.Information($"WebSocket server started at ws://{sharedData.MyAddress}");
        }

        /// <summary>
        /// Sends data to all connected clients via the specified WebSocket server.
        /// </summary>
        /// <param name="dataToSend">The data to be sent to clients.</param>
        /// <param name="server">The WebSocket server used to broadcast the data.</param>
        public static void SendToClients(Data dataToSend, WebSocketServer server)
        {
            foreach (var path in server.WebSocketServices.Paths)
            {
                var service = server.WebSocketServices[path].Sessions;
                service.Broadcast(JsonSerializer.Serialize(dataToSend));
            }
        }

        /// <summary>
        /// Sends data to all connected clients using the current WebSocket server instance.
        /// </summary>
        /// <param name="dataToSend">The data to be sent to clients.</param>
        public void SendToClients(Data dataToSend)
        {
            foreach (var path in server.WebSocketServices.Paths)
            {
                var service = server.WebSocketServices[path].Sessions;
                service.Broadcast(JsonSerializer.Serialize(dataToSend));
            }
        }
    }
}
