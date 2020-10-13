using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Core.GRYCryptoSystem.EncryptionAlgorithms.Asymmetric
{
    public class RSA : IAsymmetricEncryptionAlgorithm
    {
        public int KeyLength { get; set; } = 4096;
        public byte[] Decrypt(byte[] encryptedData, byte[] password)
        {
            throw new NotImplementedException();
        }

        public byte[] Encrypt(byte[] unencryptedData, byte[] password)
        {
            throw new NotImplementedException();
        }
    }
}
