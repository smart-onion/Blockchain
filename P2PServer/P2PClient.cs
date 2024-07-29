using System.Net;
using System.Text.Json;
using WebSocket4Net;

namespace BlockChain
{
    public class P2PClient
    {
        P2PSharedData sharedData;

        public P2PClient(P2PSharedData sharedData)
        {
            this.sharedData = sharedData;
        }

        public void ConnectToPeer(string address)
        {
            if (sharedData.ConnectedAddresses.Contains(address)) return;

            WebSocket socket = new WebSocket(address + "/");

            socket.Opened += async (sender, e) =>
            {
                Console.WriteLine($"Open socket on {address}");
                //SendAddress(socket); 
                sharedData.ConnectedAddresses.Add(address);
                await Task.Delay(500); // 500 ms delay
            };


            socket.MessageReceived += (sender, e) =>
            {

                Data? data = JsonSerializer.Deserialize<Data>(e.Message, new JsonSerializerOptions
                {
                    Converters = { new ISendableConverter() },
                    WriteIndented = true
                });

                Console.WriteLine("Message received: " + data.mt);
                switch (data?.mt)
                {
                    case MessageType.CHAIN:
                        break;
                    case MessageType.TRANSACTION:
                        break;
                    case MessageType.ADDRESS:
                        sharedData.Addresses.AddAddress((WsAddresses)data.data);
                        P2PServer.SendToClients(new Data(MessageType.ADDRESS, sharedData.Addresses), sharedData.Server);
                        ConnectToPeers();
                        break;
                    default:
                        break;
                };
            };


            socket.Error += (sender, e) =>
            {
                Console.WriteLine("Error on socket");
                sharedData.ConnectedAddresses.Remove(address);

            };

            socket.Closed += (sender, e) =>
            {
                Console.WriteLine($"Connection close on {address}");
                sharedData.ConnectedAddresses.Remove(address);

            };
            socket.Open();
            sharedData.Sockets.Add(socket);
        }

        public void ConnectToPeers()
        {
            foreach (string address in sharedData.Addresses.Addresses)
            {
                if (address == $"ws://{sharedData.MyAddress}" || sharedData.ConnectedAddresses.Contains(address))
                {
                    continue; // Skip if it's the same as my address or already connected
                }

                ConnectToPeer(address);
            }
        }

        

        private void SendChain(WebSocket socket)
        {
            socket.Send(JsonSerializer.Serialize(new Data(MessageType.CHAIN, sharedData.Bc)));
        }

        private void SendTransaction(WebSocket socket, Transaction transaction)
        {
            socket.Send(JsonSerializer.Serialize(new Data(MessageType.TRANSACTION, transaction)));
        }

        public void BroadcastTransactions(Transaction transaction)
        {
            sharedData.Sockets.ForEach(socket => SendTransaction(socket, transaction));
        }

        public void SyncChains()
        {
            foreach (WebSocket socket in sharedData.Sockets)
            {
                SendChain(socket);
            }
        }
    }
}
