using System;

namespace GRYLibrary.Core.GRYCryptoSystem.EncryptionAlgorithms.Symmetric
{
    public class AES256 : ISymmetricEncryptionAlgorithm
    {
        /// <inheritdoc/>
        public byte[] Decrypt(byte[] encryptedData, byte[] password)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public byte[] Encrypt(byte[] unencryptedData, byte[] password)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public byte[] GenerateRandomKey()
        {
            throw new NotImplementedException();
        }
    }
}
