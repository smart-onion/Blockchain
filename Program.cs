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
            args[0] = "--web:localhost:3000";
        }

        Application.Start(args);
        Console.ReadLine();
        /*
                Wallet w = new Wallet();
                Console.WriteLine(w);
                TransactionPool pool = new TransactionPool();
                var amount = 50;

                var recepient = new KeyPair();
                var k = KeyPair.GetPublicKey(recepient.Keys.ExportParameters(false));

                var tran = Transaction.NewTransaction(w, k, amount);
                Console.WriteLine(Transaction.VerifyTransaction(tran));
                pool.UpdateOrAddTransaction(tran);

                amount = 80;
                tran = tran.Update(w, k, amount);
                Console.WriteLine(Transaction.VerifyTransaction(tran));
                pool.UpdateOrAddTransaction(tran);

                w.CreateTransaction(k, 10, pool);
                w.CreateTransaction(w.PublicKey, 10, pool);*/


    }
}
