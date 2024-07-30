using System.Text.Json;
using WebSocketSharp;
using WebSocketSharp.Server;
using Serilog;

namespace BlockChain
{
    internal class P2PBehavior : WebSocketBehavior
    {
        P2PSharedData sharedData;
        public P2PBehavior(P2PSharedData sharedData)
        {
            this.sharedData = sharedData;
        }

        protected override void OnError(WebSocketSharp.ErrorEventArgs e)
        {
            Serilog.Log.Error("Ops from server");
        }

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

            /*            Blockchain? data = JsonSerializer.Deserialize<Blockchain>(e.Data);
                        this.bc.ReplaceChain(data);*/
        }

        protected override void OnOpen()
        {
            try
            {
                Serilog.Log.Information("New connection established.");
                SendData<Blockchain>(MessageType.CHAIN, sharedData.Bc);
                SendData<TransactionPool>(MessageType.TRANSACTION, sharedData.TransactionPool);
            }
            catch (Exception ex)
            {

                Serilog.Log.Error($"Error on connection open: {ex.Message}");
            }
            
        }

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

        private void SendData<T>(MessageType type, ISendable<T> data)
        {
            Send(JsonSerializer.Serialize(new Data(type, data.Serialize())));
        }
    }
}
