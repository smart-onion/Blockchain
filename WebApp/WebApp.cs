using System.Text.Json;

namespace BlockChain
{
    public class WebApp
    {
        private struct Data
        {
            public string data { get; set; }
        }

        private struct TransactData
        {
            public string recipient { get; set; }
            public int amount { get; set; }
        }

        private WebApplication app;
        private Blockchain bc;
        private readonly string url;
        private readonly P2PServer p2p;
        private readonly P2PClient client;
        private Wallet wallet;
        private TransactionPool tp;
        public WebApp(Blockchain bc, P2PServer p2p, P2PClient client, string url, Wallet wallet, TransactionPool tp)
        {
            this.bc = bc;
            this.p2p = p2p;
            this.client = client;
            this.url = url;
            this.wallet = wallet;
            this.tp = tp;
            WebApplicationBuilder builder = WebApplication.CreateBuilder();
            app = builder.Build();
        }

        private void Init()
        {
            GETRequests();
            POSTRequests();
        }

        private void GETRequests()
        {
            app.MapGet("/blocks", () =>
            {
                return JsonSerializer.Serialize(bc);
            });

            app.MapGet("/transactions", () => 
            {
                return JsonSerializer.Serialize(tp);
            });
           
        }

        private void POSTRequests()
        {
            app.MapPost("/mine", async context =>
            {
                Data data = await JsonSerializer.DeserializeAsync<Data>(context.Request.Body);
                
                Block block = this.bc.AddBlock(data.data);
                await Console.Out.WriteLineAsync($"New block added: {block}");
                p2p.SendToClients(new BlockChain.Data(MessageType.CHAIN, this.bc.Serialize()));
                client.SyncChains();
                Results.Redirect("/blocks");
            });

            app.MapPost("/transact", async context =>
            {
                TransactData data = await JsonSerializer.DeserializeAsync<TransactData>(context.Request.Body);
                Transaction? transaction = wallet.CreateTransaction(data.recipient, data.amount, tp);

                if (transaction == null)
                {
                    await Console.Out.WriteLineAsync($"Amount: {data.amount} exceed balance");
                }
                Results.Redirect("/transactions");
            });
        }

        public void Run()
        {
            Init();
            app.Run($"http://{url}");
        }
    }
}
