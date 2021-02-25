using GRYLibrary.Core.Miscellaneous;

namespace GRYLibrary.Core.CryptoSystems.ConcreteHashAlgorithms
{
    public class SHA256 : HashAlgorithm
    {
        public override byte[] Hash(byte[] data)
        {
            using System.Security.Cryptography.SHA256 algorithmImplementation = System.Security.Cryptography.SHA256.Create();
            return algorithmImplementation.ComputeHash(data);
        }
        public override byte[] GetIdentifier()
        {
            return Utilities.PadLeft(System.Text.Encoding.ASCII.GetBytes(nameof(SHA256)), 10);
        }
    }
}
