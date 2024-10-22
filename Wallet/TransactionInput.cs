﻿using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace BlockChain
{
    /// <summary>
    /// Represents an input for a blockchain transaction.
    /// </summary>
    public class TransactionInput
    {
        private double timestamp;
        private int amount;
        private string address;
        private byte[] signature;

        /// <summary>
        /// Gets the amount associated with the transaction input.
        /// </summary>
        public int Amount { get => this.amount; }

        /// <summary>
        /// Gets the address associated with the transaction input.
        /// </summary>
        public string Address { get => this.address; }

        /// <summary>
        /// Gets the signature associated with the transaction input.
        /// </summary>
        public byte[] Signature { get => this.signature; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionInput"/> class.
        /// </summary>
        public TransactionInput() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionInput"/> class with the specified amount, address, and signature.
        /// </summary>
        /// <param name="amount">The amount associated with the transaction input.</param>
        /// <param name="address">The address associated with the transaction input.</param>
        /// <param name="signature">The signature associated with the transaction input.</param>
        [JsonConstructor]
        public TransactionInput(int amount, string address, byte[] signature)
        {
            this.timestamp = DateTime.Now.Ticks / TimeSpan.TicksPerMicrosecond;
            this.amount = amount;
            this.address = address;
            this.signature = signature;
        }
    }
}
