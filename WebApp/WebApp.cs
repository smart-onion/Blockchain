using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace BlockChain
{
    /// <summary>
    /// Represents a web application that provides HTTP endpoints for interacting with the blockchain.
    /// </summary>
    public class WebApp
    {
        private struct Data
        {
            /// <summary>
            /// Gets or sets the data for a blockchain block.
            /// </summary>
            public string data { get; set; }
        }

        private struct TransactData
        {
            /// <summary>
            /// Gets or sets the recipient address for a transaction.
            /// </summary>
            public string recipient { get; set; }

            /// <summary>
            /// Gets or sets the amount of cryptocurrency for a transaction.
            /// </summary>
            public int amount { get; set; }
        }

        private WebApplication app;
        private Blockchain bc;
        private readonly string url;
        private Wallet wallet;
        private TransactionPool tp;
        private PubSub publisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApp"/> class with the specified parameters.
        /// </summary>
        /// <param name="bc">The blockchain instance to be used by the web application.</param>
        /// <param name="publisher">The Redis Clientinstance for network communication.</param>
        /// <param name="url">The URL on which the web application will run.</param>
        /// <param name="wallet">The wallet instance for creating transactions.</param>
        /// <param name="tp">The transaction pool instance for managing transactions.</param>
        public WebApp(Blockchain bc, PubSub publisher, string url, Wallet wallet, TransactionPool tp)
        {
            this.bc = bc;
            this.publisher = publisher;
            this.url = url;
            this.wallet = wallet;
            this.tp = tp;
            WebApplicationBuilder builder = WebApplication.CreateBuilder();
            app = builder.Build();
        }

        /// <summary>
        /// Initializes HTTP request mappings for the web application.
        /// </summary>
        private void Init()
        {
            GETRequests();
            POSTRequests();
        }

        /// <summary>
        /// Maps HTTP GET requests to appropriate endpoints.
        /// </summary>
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

            app.MapGet("/public-key", () =>
            {
                return JsonSerializer.Serialize(wallet.PublicKey);
            });
        }

        /// <summary>
        /// Maps HTTP POST requests to appropriate endpoints.
        /// </summary>
        private void POSTRequests()
        {
            app.MapPost("/mine", async context =>
            {
                Data data = await JsonSerializer.DeserializeAsync<Data>(context.Request.Body);

                Block block = this.bc.AddBlock(data.data);
                await Console.Out.WriteLineAsync($"New block added: {block}");
                publisher.BroadcastChain();
                context.Response.Redirect("/blocks");
            });

            app.MapPost("/transact", async context =>
            {
                TransactData data = await JsonSerializer.DeserializeAsync<TransactData>(context.Request.Body);
                Transaction? transaction = wallet.CreateTransaction(data.recipient, data.amount, tp);

                if (transaction == null)
                {
                    context.Response.WriteAsync($"Amount: {data.amount} exceed balance");
                    return;
                }
                publisher.BroadcastTransaction(transaction);
                context.Response.Redirect("/transactions");
            });

        }

        /// <summary>
        /// Runs the web application on the specified URL.
        /// </summary>
        public void Run()
        {
            Init();
            app.Run($"http://{url}");
        }
    }
}
