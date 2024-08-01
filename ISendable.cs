using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlockChain
{
    /// <summary>
    /// Defines a method to serialize an object.
    /// </summary>
    public interface ISendable
    {
        /// <summary>
        /// Serializes the object to a string.
        /// </summary>
        /// <returns>A string representation of the object.</returns>
        public string Serialize();
    }
}

