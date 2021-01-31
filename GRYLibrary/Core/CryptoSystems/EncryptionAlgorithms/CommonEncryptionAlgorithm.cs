﻿namespace GRYLibrary.Core.CryptoSystems.EncryptionAlgorithms
{
    public abstract class CommonEncryptionAlgorithm: EncryptionAlgorithm
    {
        /// <summary>
         /// Use this function to encrypt data.
         /// </summary>
        public abstract byte[] Encrypt(byte[] unencryptedData, byte[] password);
        public string Encrypt(string unencryptedData, string password)
        {
            return Utilities.ByteArrayToHexString(Encrypt(this.Encoding.GetBytes(unencryptedData), this.Encoding.GetBytes(password)));
        }
        /// <summary>
        /// Use this function to decrypt data.
        /// </summary>
        public abstract byte[] Decrypt(byte[] encryptedData, byte[] password);
        public string Decrypt(string encryptedData, string password)
        {
            return Utilities.ByteArrayToHexString(Decrypt(this.Encoding.GetBytes(encryptedData), this.Encoding.GetBytes(password)));
        }
    }
}
