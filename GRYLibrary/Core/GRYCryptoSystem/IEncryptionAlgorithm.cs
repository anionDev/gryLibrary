namespace GRYLibrary.Core.GRYCryptoSystem
{
    public interface IEncryptionAlgorithm
    {
        /// <summary>
        /// Use this function to encrypt data.
        /// </summary>
        public byte[] Encrypt(byte[] unencryptedData, byte[] password);
        /// <summary>
        /// Use this function to decrypt data.
        /// </summary>
        public byte[] Decrypt(byte[] encryptedData, byte[] password);
    }
}
