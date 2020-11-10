namespace GRYLibrary.Core.CryptoSystems
{
    public abstract class HashAlgorithm : CryptographyAlgorithm
    {
        public abstract byte[] Hash(byte[] data);
        public string Hash(string data)
        {
            return Utilities.ByteArrayToHexString(Hash(Encoding.GetBytes(data)));
        }
    }
}
