namespace GRYLibrary.Core.GRYCryptoSystem.EncryptionAlgorithms
{
    public interface ISymmetricEncryptionAlgorithm : IEncryptionAlgorithm
    {
        /// <summary>
        /// Creates a new random key for encryption with this symmetric encryption algorithm.
        /// </summary>
        public byte[] GenerateRandomKey();
    }
}
