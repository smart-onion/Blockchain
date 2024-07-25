//using System.Net.WebSockets;
using System.Text.Json;
using WebSocketSharp.Server;

namespace BlockChain
{
    public class P2PServer
    {
        private readonly int port;
        private Blockchain bc;
        WebSocketServer server;
        public P2PServer(Blockchain bc, int port)
        {
            this.port = port;
            this.bc = bc;
        }

        public void Listen()
        {
            server = new WebSocketServer($"ws://0.0.0.0:{port}");
            server.AddWebSocketService<P2PBehavior>("/", () => new P2PBehavior(this.bc));

            server.Start();
            Console.WriteLine($"WebSocket server started at ws://localhost:{port}");
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
