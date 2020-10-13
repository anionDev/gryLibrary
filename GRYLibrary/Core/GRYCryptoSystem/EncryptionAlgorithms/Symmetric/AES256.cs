using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Core.GRYCryptoSystem.EncryptionAlgorithms.Symmetric
{
    public class AES256 : ISymmetricEncryptionAlgorithm
    {
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
