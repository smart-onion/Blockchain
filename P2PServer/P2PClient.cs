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
            WebSocket socket = new WebSocket(address + "/");

            socket.Opened += async (sender, e) =>
            {
                Serilog.Log.Information($"Open socket on {address}");
            };

            socket.MessageReceived += (sender, e) =>
            {

                Data? data = JsonSerializer.Deserialize<Data>(e.Message);

                Serilog.Log.Information("Message received: " + data.mt);
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

        public void ConnectToPeers()
        {
            foreach (string address in sharedData.Addresses)
            {
                ConnectToPeer(address);
            }
        }

        private void SendChain(WebSocket socket)
        {
            socket.Send(JsonSerializer.Serialize(new Data(MessageType.CHAIN, sharedData.Bc.Serialize())));
        }

        private void SendTransaction(WebSocket socket, Transaction transaction)
        {
            socket.Send(JsonSerializer.Serialize(new Data(MessageType.TRANSACTION, transaction.Serialize())));
            
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
