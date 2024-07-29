namespace BlockChain
{
    public class WsAddresses : ISendable
    {
        public List<string> Addresses { get; set; }

        public WsAddresses(List<string> addresses)
        {
            Addresses = addresses;
        }

        public void AddAddress(WsAddresses addrs)
        {
            foreach (string address in addrs.Addresses)
            {
                if (!Addresses.Contains(address))
                {
                    Addresses.Add(address);
                }
            }
        }
    }

   
}
