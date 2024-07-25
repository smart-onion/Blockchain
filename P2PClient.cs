using System.Text.Json;
using WebSocket4Net;

namespace BlockChain
{
    public class P2PClient
    {
        private List<string> addresses;
        private Blockchain bc;
        private List<WebSocket> sockets = new List<WebSocket>();


        public P2PClient(Blockchain bc, List<string> addrs)
        {
            this.bc = bc;
            this.addresses = addrs;
        }

        public void ConnectToPeers()
        {
            foreach (string address in this.addresses)
            {
                WebSocket socket = new WebSocket(address + "/");


                socket.Opened += (sender, e) =>
                {
                    Console.WriteLine($"Open socket on {address}");
                };


                socket.MessageReceived += (sender, e) =>
                {
                    Blockchain? data = JsonSerializer.Deserialize<Blockchain>(e.Message);
                    Console.WriteLine($"Chain received");
                    this.bc.ReplaceChain(data);
                };

                socket.Error += (sender, e) =>
                {
                    Console.WriteLine("Error on socket");
                };

                socket.Closed += (sender, e) =>
                {
                    Console.WriteLine($"Connection close on {address}");
                };
                socket.Open();

                this.sockets.Add(socket);

            }
        }

        private void SendChain(WebSocket socket)
        {
            socket.Send(JsonSerializer.Serialize(bc));
        }

        public void SyncChains()
        {
            foreach (WebSocket socket in this.sockets)
            {

                SendChain(socket);
            }
        }
    }
}
