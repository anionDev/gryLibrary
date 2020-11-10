using GRYLibrary.Core.CryptoSystems.EncryptionAlgorithms.ConcreteCommonAlgorithms.ConcreteAsymmetricAlgorithms.Helper;
using System;

namespace GRYLibrary.Core.CryptoSystems.EncryptionAlgorithms.ConcreteCommonAlgorithms.ConcreteAsymmetricAlgorithms
{
    public class ECC : AsymmetricEncryptionAlgorithm
    {
        public Curve Curve { get; set; } = Curve.Curve25519;
        /// <inheritdoc/>
        public override byte[] Decrypt(byte[] encryptedData, byte[] password)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override byte[] Encrypt(byte[] unencryptedData, byte[] password)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc/>
        public override (byte[]/*Private key*/, byte[]/*Public key*/) GenerateRandomKeyPair()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override byte[] GetIdentifier()
        {
            return Utilities.PadLeft(System.Text.Encoding.ASCII.GetBytes(nameof(ECC)), 10);
        }
    }
}
