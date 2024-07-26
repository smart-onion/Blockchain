using System.Text.Json;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace BlockChain
{
    internal class P2PBehavior : WebSocketBehavior
    {
        Blockchain bc;

        public P2PBehavior(Blockchain bc)
        {
            this.bc = bc;
        }

        protected override void OnError(WebSocketSharp.ErrorEventArgs e)
        {
            Console.WriteLine("Opps from server");
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            Blockchain? data = JsonSerializer.Deserialize<Blockchain>(e.Data);
            Console.WriteLine($"Server Received chain");
            this.bc.ReplaceChain(data);
        }

        protected override void OnOpen()
        {
            try
            {
                Console.WriteLine("New connection established.");
                Send(JsonSerializer.Serialize(bc));

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
                var clientAddress = Context.UserEndPoint.ToString();
                Console.WriteLine($"Connection closed: {clientAddress}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error on connection close: {ex.Message}");
            }

        }
    }
}
