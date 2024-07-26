using System.Text.Json.Serialization;

namespace BlockChain
{
    public struct Data
    {
        private Blockchain bc;
        private List<string> connections;

        public Blockchain Bc { get => this.bc; }
        public List<string> Connections { get => this.connections; }


        public Data()
        {
            this.bc = new Blockchain();
            this.connections = new List<string>();
        }
        [JsonConstructor]
        public Data(Blockchain bc, List<string> connections)
        {
            this.bc = bc;
            this.connections = connections;
        }
    }
}
