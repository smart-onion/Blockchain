using BlockChain;
using StackExchange.Redis;

internal class Program
{

    /// <summary>
    /// Test docfx
    /// </summary>
    /// <param name="args"></param>
    private static void Main(string[] args)
    {

        if (args.Length == 0)
        {
            args = new string[3];
            args[0] = "--web:localhost:3002";
        }

        Application.Start(args);
    }
}
