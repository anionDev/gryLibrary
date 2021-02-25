namespace GRYLibrary.Core.CryptoSystems.EncryptionAlgorithms.ConcreteCommonAlgorithms
{
    public abstract class SymmetricEncryptionAlgorithm : CommonEncryptionAlgorithm
    {
        /// <summary>
        /// Creates a new random key for encryption with this symmetric encryption algorithm.
        /// </summary>
        public abstract byte[] GenerateRandomKey();
    }
}
