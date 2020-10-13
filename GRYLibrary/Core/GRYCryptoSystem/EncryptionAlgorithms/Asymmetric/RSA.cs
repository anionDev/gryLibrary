using System;

namespace GRYLibrary.Core.GRYCryptoSystem.EncryptionAlgorithms.Asymmetric
{
    public class RSA : IAsymmetricEncryptionAlgorithm
    {
        public int KeyLength { get; set; } = 4096;
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
        public (byte[]/*Private key*/, byte[]/*Public key*/) GenerateRandomKeyPair()
        {
            throw new NotImplementedException();
        }
    }
}
