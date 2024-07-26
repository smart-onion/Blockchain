using BlockChain;

internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            args = new string[3];
            args[0] = "--web:localhost:3002";
            args[1] = "--p2p:localhost:5002";
            args[2] = "ws://localhost:5001";
        }
        
        Application.Start(args);
        Console.ReadLine();

    }
}
