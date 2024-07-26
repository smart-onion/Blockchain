using System.Text.RegularExpressions;

namespace BlockChain
{
    public static class Application
    {
        private static Blockchain blockchain = new Blockchain();
        private static List<string> addrs = new List<string>();
        private static P2PServer p2p;
        private static P2PClient client;
        private static WebApp app;
        private static string p2pURL;
        private static string webAppURL;
        static Application()
        {
            
        }

        public static void Start(string[] args)
        {
            InitVars(args);
            p2p.Listen();
            client.ConnectToPeers();
            ChainUpdatedThread();
            app.Run();
        }

        private static void InitVars(string[] args)
        {
            webAppURL = GetArgs(args[0], @"--web:([^:]+:\d+)");
            p2pURL = GetArgs(args[1], @"--p2p:([^:]+:\d+)");

            for (int i = 2; i < args.Length; i++)
            {
                addrs.Add(args[i]);
            }

            p2p = new P2PServer(blockchain, p2pURL);
            client = new P2PClient(blockchain, addrs);
            app = new WebApp(blockchain, p2p, client, webAppURL);
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

        private static void ChainUpdatedThread()
        {
            Thread update = new Thread(new ThreadStart(UpdateChain));
            update.Start();
        }

        private static void UpdateChain()
        {
            int len = blockchain.Chain.Count;
            while (true)
            {
                if (blockchain.Chain.Count > len)
                {
                    len = blockchain.Chain.Count;
                    p2p.SendToClients();
                    client.SyncChains();
                }
            }
        }
    }
}
