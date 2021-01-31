﻿namespace GRYLibrary.Core.CryptoSystems.EncryptionAlgorithms.ConcreteOtherAlgorithms
{
    public abstract class EncryptUsingPropertiesAlgorithm : OtherEncryptionAlgorithm
    {
        /// <summary>
        /// Use this function to encrypt data.
        /// </summary>
        public abstract byte[] Encrypt(byte[] unencryptedData);
        public string Encrypt(string unencryptedData)
        {
            return Utilities.ByteArrayToHexString(Encrypt(this.Encoding.GetBytes(unencryptedData)));
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
