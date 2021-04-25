using GRYLibrary.Core.Miscellaneous;

namespace GRYLibrary.Core.CryptoSystems
{
    public abstract class HashAlgorithm : CryptographyAlgorithm
    {
        public abstract byte[] Hash(byte[] data);
        public string Hash(string data)
        {
            return Utilities.ByteArrayToHexString(this.Hash(this.Encoding.GetBytes(data)));
        }
    }
}
