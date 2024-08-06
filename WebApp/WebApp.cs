using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            public Transaction data { get; set; }
        }

        private struct TransactData
        {
            /// <summary>
            /// Gets or sets the recipient address for a transaction.
            /// </summary>
            public string recipient { get; set; }

            public string privateKey { get; set; }

            /// <summary>
            /// Gets or sets the amount of cryptocurrency for a transaction.
            /// </summary>
            public int amount { get; set; }
        }

        private struct WalletData
        {
            public int Balance { get; set; }
            public string PublicKey { get; set; }

            public string PrivateKey { get; set; }

            [JsonConstructor]
            public WalletData(int balance, string publicKey, string privateKey)
            {
                this.Balance = balance;
                this.PublicKey = publicKey;
                this.PrivateKey = privateKey;
            }
        }

        private struct Key
        {
            public string PrivateKey { get; set; }
        }

        private WebApplication app;
        private Blockchain bc;
        private readonly string url;
        private Wallet wallet;
        private TransactionPool tp;
        private PubSub publisher;
        private Miner miner;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApp"/> class with the specified parameters.
        /// </summary>
        /// <param name="bc">The blockchain instance to be used by the web application.</param>
        /// <param name="publisher">The Redis Clientinstance for network communication.</param>
        /// <param name="url">The URL on which the web application will run.</param>
        /// <param name="wallet">The wallet instance for creating transactions.</param>
        /// <param name="tp">The transaction pool instance for managing transactions.</param>
        public WebApp(Blockchain bc, PubSub publisher, string url, Wallet wallet, TransactionPool tp, Miner miner)
        {
            this.bc = bc;
            this.publisher = publisher;
            this.url = url;
            this.wallet = wallet;
            this.tp = tp;
            this.miner = miner;
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
            app.MapGet("/", async context =>
            {
                context.Response.Redirect("/wallet");
            });

            app.MapGet("/blocks", context =>
            {
                var chain = JsonSerializer.Serialize(bc);
                context.Response.ContentType = "text/html";
                return context.Response.WriteAsync(HTMLRender.Render("Chain.html", new { chain }));
            });

            app.MapGet("/mine-transactions", async context =>
            {
                miner.Mine();
                context.Response.Redirect("/blocks");
            });

            app.MapGet("/transactions", context =>
            {
                var pool = JsonSerializer.Serialize(tp);
                context.Response.ContentType = "text/html";
                return context.Response.WriteAsync(HTMLRender.Render("TransactionPool.html", new { pool }));
            });

            app.MapGet("/wallet", context => {
                var wallet = JsonSerializer.Serialize(
                    new WalletData(
                        this.wallet.CalculateBalance(bc, this.wallet.PublicKey), 
                        this.wallet.PublicKey, 
                        this.wallet.GetPrivateKey())
                    );
                context.Response.ContentType = "text/html";
                return context.Response.WriteAsync(HTMLRender.Render("Wallet.html", new { wallet }));
            });

            app.MapGet("/set-wallet", context =>
            {
                return null;
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
            app.MapPost("/create-transaction", async context =>
            {
                TransactData data = await JsonSerializer.DeserializeAsync<TransactData>(context.Request.Body);
                Transaction? transaction = wallet.CreateTransaction(data.recipient, data.amount, tp, this.bc);

                if (transaction == null)
                {
                    context.Response.WriteAsync($"Amount: {data.amount} exceed balance");
                    return;
                }
                publisher.BroadcastTransaction(transaction);
                context.Response.Redirect("/transactions");
            });

            app.MapPost("/set-wallet", async context =>
            {
                Key key = await JsonSerializer.DeserializeAsync<Key>(context.Request.Body);
                wallet.KeyPair.SetECDsa(key.PrivateKey);
                wallet.SetPublicKey(key.PrivateKey);
                context.Response.Redirect("/wallet");
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
