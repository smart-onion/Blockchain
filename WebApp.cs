using System.Text.Json;

namespace BlockChain
{
    public class WebApp
    {
        private struct Data
        {
            public string data { get; set; }
        }

        private WebApplication app;
        private Blockchain bc;
        private readonly string url;
        private readonly P2PServer p2p;
        private readonly P2PClient client;

        public WebApp(Blockchain bc, P2PServer p2p, P2PClient client, string url)
        {
            this.bc = bc;
            this.p2p = p2p;
            this.client = client;
            this.url = url;
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
           
        }

        private void POSTRequests()
        {
            app.MapPost("/mine", async context =>
            {
                Data data = await JsonSerializer.DeserializeAsync<Data>(context.Request.Body);
                
                Block block = this.bc.AddBlock(data.data);
                await Console.Out.WriteLineAsync($"New block added: {block}");
                p2p.SendToClients();
                client.SyncChains();
                Results.Redirect("/blocks");
            });
        }

        public void Run()
        {
            Init();
            app.Run($"http://{url}");
        }
    }
}
