using System.Text.RegularExpressions;

namespace BlockChain
{
    public static class Application
    {
        private static Wallet wallet = new Wallet();
        private static Blockchain blockchain = new Blockchain();
        private static TransactionPool tp = new TransactionPool();
        private static P2PServer p2p;
        private static WebApp app;
        private static P2PSharedData sharedData;

        static Application() { }

        public static void Start(string[] args)
        {
            InitVars(args);
            p2p.Listen();
            sharedData.Client.ConnectToPeers();
            app.Run();
        }

        private static void InitVars(string[] args)
        {

            var webAppURL = GetArgs(args[0], @"--web:([^:]+:\d+)");
            var p2pURL = GetArgs(args[1], @"--p2p:([^:]+:\d+)");
            var addrs = new List<string>();

            for (int i = 2; i < args.Length; i++)
            {
                addrs.Add(args[i]);
            }

            sharedData = new P2PSharedData(blockchain, addrs, p2pURL);

            sharedData.Client = new P2PClient(sharedData);
            p2p = new P2PServer(sharedData);

            app = new WebApp(blockchain, p2p, sharedData.Client, webAppURL, wallet, tp);
        }

        private static string GetArgs(string input, string pattern)
        {
            Regex regex = new Regex(pattern);
            Match match = regex.Match(input);

            if (match.Success)
            {
                return  match.Groups[1].Value;
            }
            else
            {
                throw new Exception("Wrong input! Usage: dotnet run WEB_APP_PORT P2P_PORT ADDRESS1 ADDRESS2 .... ");
            }
        }
    }
}
