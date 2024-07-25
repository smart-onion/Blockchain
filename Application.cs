using System.Text.RegularExpressions;

namespace BlockChain
{
    public static class Application
    {
        private static Blockchain blockchain = new Blockchain();
        private static List<string> addrs = new List<string>();
        private static int p2pPort = 5001;
        private static int webAppPort = 5000;
        private static P2PServer p2p;
        private static P2PClient client;
        private static WebApp app;

        static Application()
        {
            
        }

        public static void Start(string[] args)
        {
            InitVars(args);
            p2p.Listen();
            client.ConnectToPeers();
            app.Run();
        }

        private static void InitVars(string[] args)
        {
            webAppPort = Convert.ToInt32(GetArgs(args[0], @"--web:(\d+)"));
            p2pPort = Convert.ToInt32(GetArgs(args[1], @"--p2p:(\d+)"));


            for (int i = 2; i < args.Length; i++)
            {
                addrs.Add(args[i]);
            }

            p2p = new P2PServer(blockchain, p2pPort);
            client = new P2PClient(blockchain, addrs);
            app = new WebApp(blockchain, p2p, client, webAppPort);
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
