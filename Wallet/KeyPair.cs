using System.Security.Cryptography;
using System.Text.Json;

namespace BlockChain
{
    /// <summary>
    /// Represents a pair of elliptic curve cryptographic (ECC) keys.
    /// </summary>
    public class KeyPair
    {
        ECDsa keys;

        /// <summary>
        /// Gets the ECDsa keys associated with this KeyPair.
        /// </summary>
        public ECDsa Keys { get => this.keys; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyPair"/> class with a new set of ECC keys.
        /// </summary>
        public KeyPair()
        {
            this.keys = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        }

        /// <summary>
        /// Gets the public key in a serialized string format.
        /// </summary>
        /// <param name="key">The ECParameters representing the key.</param>
        /// <returns>A string representation of the public key.</returns>
        public static string GetPublicKey(ECParameters key)
        {
            string curveName = key.Curve.Oid.FriendlyName;
            string x = Convert.ToBase64String(key.Q.X);
            string y = Convert.ToBase64String(key.Q.Y);
            return $"{curveName}:{x}:{y}";
        }

        /// <summary>
        /// Parses a JSON-encoded public key and returns its ECParameters representation.
        /// </summary>
        /// <param name="jsonKey">The JSON-encoded public key.</param>
        /// <returns>The ECParameters representing the key.</returns>
        /// <exception cref="ArgumentException">Thrown when the key format is invalid.</exception>
        public static ECParameters GetECParameters(string jsonKey)
        {
            string[] parts = jsonKey.Split(':');
            if (parts.Length != 3)
            {
                throw new ArgumentException("Invalid key format");
            }

            string curveName = parts[0];
            byte[] x = Convert.FromBase64String(parts[1]);
            byte[] y = Convert.FromBase64String(parts[2]);

            return new ECParameters
            {
                Curve = ECCurve.CreateFromFriendlyName(curveName),
                Q = new ECPoint
                {
                    X = x,
                    Y = y
                }
            };
        }

        /// <summary>
        /// Verifies a digital signature using the given public key and data hash.
        /// </summary>
        /// <param name="keys">The ECParameters representing the public key.</param>
        /// <param name="signature">The digital signature to verify.</param>
        /// <param name="dataHash">The hash of the data that was signed.</param>
        /// <returns><c>true</c> if the signature is valid; otherwise, <c>false</c>.</returns>
        public static bool VerifySignature(ECParameters keys, byte[] signature, byte[] dataHash)
        {
            try
            {
                // Import public key into ECDsa
                using (ECDsa ecdsa = ECDsa.Create(keys))
                {
                    // Verify the signature
                    return ecdsa.VerifyHash(dataHash, signature);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets the public key as ECParameters from a <see cref="KeyPair"/> instance.
        /// </summary>
        /// <param name="keys">The KeyPair instance.</param>
        /// <returns>The ECParameters representing the public key.</returns>
        public static ECParameters GetPublicKey(KeyPair keys)
        {
            return keys.Keys.ExportParameters(false);

        }

        /// <summary>
        /// Compares two ECParameters for equality.
        /// </summary>
        /// <param name="left">The first ECParameters to compare.</param>
        /// <param name="right">The second ECParameters to compare.</param>
        /// <returns><c>true</c> if both ECParameters are equal; otherwise, <c>false</c>.</returns>
        public static bool KeyAreEqual(ECParameters left, ECParameters right)
        {
            // Compare curve
            if (left.Curve.Oid.Value != right.Curve.Oid.Value)
                return false;

            // Compare Q.X coordinates
            if (!left.Q.X.SequenceEqual(right.Q.X))
                return false;

            // Compare Q.Y coordinates
            if (!left.Q.Y.SequenceEqual(right.Q.Y))
                return false;

            return true;
        }

        /// <summary>
        /// Returns a string representation of the <see cref="KeyPair"/> object.
        /// </summary>
        /// <returns>A string representation of the <see cref="KeyPair"/> object.</returns>
        public override string ToString()
        {
            return base.ToString();
        }

    }
}
