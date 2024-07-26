//using System.Net.WebSockets;
using System.Text.Json;
using WebSocketSharp.Server;

namespace BlockChain
{
    public class P2PServer
    {
        private Blockchain bc;
        private WebSocketServer server;
        private string url;
        public P2PServer(Blockchain bc, string url)
        {
            this.url = url;
            this.bc = bc;
        }

        public void Listen()
        {
            server = new WebSocketServer($"ws://{url}");
            server.AddWebSocketService<P2PBehavior>("/", () => new P2PBehavior(this.bc));

            server.Start();
            Console.WriteLine($"WebSocket server started at ws://{url}");
        }


        public void SendToClients()
        {
            foreach (var path in server.WebSocketServices.Paths)
            {
                var service = server.WebSocketServices[path].Sessions;
                service.Broadcast(JsonSerializer.Serialize(bc));
            }
        }
    }
}
