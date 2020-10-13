using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Core.GRYCryptoSystem
{
    public interface IEncryptionAlgorithm
    {
        public byte[] Encrypt(byte[] unencryptedData, byte[] password);
        public byte[] Decrypt(byte[] encryptedData, byte[] password);
    }
}
