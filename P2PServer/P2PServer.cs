//using System.Net.WebSockets;
using System.Text.Json;
using WebSocketSharp.Server;
using Serilog;
namespace BlockChain
{
    public class P2PServer
    {
        private P2PSharedData sharedData;
        private WebSocketServer server;
        public P2PServer(P2PSharedData sharedData)
        {
            this.sharedData = sharedData;
        }

        public void Listen()
        {
            server = new WebSocketServer($"ws://{sharedData.MyAddress}");
            server.AddWebSocketService<P2PBehavior>("/", () => new P2PBehavior(sharedData));

            server.Start();
            Serilog.Log.Information($"WebSocket server started at ws://{sharedData.MyAddress}");
        }

        public static void SendToClients(Data dataToSend, WebSocketServer server)
        {
            foreach (var path in server.WebSocketServices.Paths)
            {
                var service = server.WebSocketServices[path].Sessions;
                service.Broadcast(JsonSerializer.Serialize(dataToSend));
            }
        }

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
