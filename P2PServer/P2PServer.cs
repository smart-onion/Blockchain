//using System.Net.WebSockets;
using System.Text.Json;
using WebSocketSharp.Server;

namespace BlockChain
{
    public class P2PServer
    {
        private P2PSharedData sharedData;
        public P2PServer(P2PSharedData sharedData)
        {
            this.sharedData = sharedData;
        }

        public void Listen()
        {
            sharedData.Server = new WebSocketServer($"ws://{sharedData.MyAddress}");
            sharedData.Server.AddWebSocketService<P2PBehavior>("/", () => new P2PBehavior(sharedData));

            sharedData.Server.Start();
            Console.WriteLine($"WebSocket server started at ws://{sharedData.MyAddress}");
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
            foreach (var path in sharedData.Server.WebSocketServices.Paths)
            {
                var service = sharedData.Server.WebSocketServices[path].Sessions;
                service.Broadcast(JsonSerializer.Serialize(dataToSend));
            }
        }
    }
}
