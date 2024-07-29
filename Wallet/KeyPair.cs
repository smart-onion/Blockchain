using System.Security.Cryptography;
using System.Text.Json;

namespace BlockChain
{
    public class KeyPair
    {
        ECDsa keys;

        public ECDsa Keys { get => this.keys; }

        public KeyPair()
        {
            this.keys = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        }

        public static string GetPublicKey(ECParameters key)
        {
            string curveName = key.Curve.Oid.FriendlyName;
            string x = Convert.ToBase64String(key.Q.X);
            string y = Convert.ToBase64String(key.Q.Y);
            return $"{curveName}:{x}:{y}";
        }

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

        public static ECParameters GetPublicKey(KeyPair keys)
        {
            return keys.Keys.ExportParameters(false);

        }

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

        public override string ToString()
        {
            return base.ToString();
        }

    }
}
