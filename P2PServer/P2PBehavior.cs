using System.Text.Json;
using WebSocketSharp;
using WebSocketSharp.Server;

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
            Console.WriteLine("Opps from server");
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            Data? data = JsonSerializer.Deserialize<Data>(e.Data, new JsonSerializerOptions
            {
                Converters = { new ISendableConverter() },
                WriteIndented = true
            });

            Console.WriteLine("Server side message: " + data.mt);
            switch (data?.mt)
            {
                case MessageType.CHAIN:
                    break;
                case MessageType.TRANSACTION:
                    break;
                case MessageType.ADDRESS:
                    sharedData.Addresses.AddAddress((WsAddresses)data.data);
                    sharedData.Client.ConnectToPeers();
                    break;
                default:
                    break;
            }

            /*            Blockchain? data = JsonSerializer.Deserialize<Blockchain>(e.Data);
                        Console.WriteLine($"Server Received chain");
                        this.bc.ReplaceChain(data);*/
        }

        protected override void OnOpen()
        {
            try
            {
                Console.WriteLine("New connection established.");
                if (!sharedData.Addresses.Addresses.Contains(sharedData.MyAddress))
                {
                    sharedData.Addresses.Addresses.Add($"ws://{sharedData.MyAddress}");
                }
                SendAddresses();


            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error on connection open: {ex.Message}");
            }
            
        }

        protected override void OnClose(CloseEventArgs e)
        {
            try
            {
                //ar clientAddress = Context?.UserEndPoint?.ToString();
                Console.WriteLine($"Connection closed: ");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error on connection close: {ex.Message}");
            }

        }

        private void SendAddresses()
        {
            Send(JsonSerializer.Serialize(new Data(MessageType.ADDRESS, sharedData.Addresses), new JsonSerializerOptions
            {
                Converters = { new ISendableConverter() },
                WriteIndented = true
            }));
        }
    }
}
