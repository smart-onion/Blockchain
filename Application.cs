using System.Text.RegularExpressions;
using Serilog;

namespace BlockChain
{
    /// <summary>
    /// Provides the main entry point for the blockchain application and manages initialization and startup.
    /// </summary>
    public static class Application
    {
        private static Wallet wallet = new Wallet();
        private static Blockchain blockchain = new Blockchain();
        private static TransactionPool tp = new TransactionPool();
        private static P2PServer p2p;
        private static P2PClient client;
        private static WebApp app;
        private static P2PSharedData sharedData;

        /// <summary>
        /// Initializes the static properties and starts the application.
        /// </summary>
        /// <param name="args">Command-line arguments for application configuration.</param>
        public static void Start(string[] args)
        {
            InitVars(args);
            p2p.Listen();
            client.ConnectToPeers();
            app.Run();
        }

        /// <summary>
        /// Initializes variables and configurations for the application.
        /// </summary>
        /// <param name="args">Command-line arguments for application configuration.</param>
        private static void InitVars(string[] args)
        {
            var webAppURL = GetArgs(args[0], @"--web:([^:]+:\d+)");
            var p2pURL = GetArgs(args[1], @"--p2p:([^:]+:\d+)");
            var addrs = new List<string>();

            for (int i = 2; i < args.Length; i++)
            {
                addrs.Add(args[i]);
            }

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            sharedData = new P2PSharedData(blockchain, tp, addrs, p2pURL);

            client = new P2PClient(sharedData);
            p2p = new P2PServer(sharedData);

            app = new WebApp(blockchain, p2p, client, webAppURL, wallet, tp);
        }

        /// <summary>
        /// Extracts a specific value from a string using a regular expression pattern.
        /// </summary>
        /// <param name="input">The input string to search.</param>
        /// <param name="pattern">The regular expression pattern to use for extraction.</param>
        /// <returns>The extracted value if the pattern matches; otherwise, throws an exception.</returns>
        /// <exception cref="Exception">Thrown when the input string does not match the pattern.</exception>
        private static string GetArgs(string input, string pattern)
        {
            Regex regex = new Regex(pattern);
            Match match = regex.Match(input);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                throw new Exception("Wrong input! Usage: dotnet run WEB_APP_PORT P2P_PORT ADDRESS1 ADDRESS2 .... ");
            }
        }
    }
}
