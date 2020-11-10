﻿namespace GRYLibrary.Core.CryptoSystems.EncryptionAlgorithms.ConcreteCommonAlgorithms
{
    public abstract class AsymmetricEncryptionAlgorithm : CommonEncryptionAlgorithm
    {
        /// <summary>
        /// Generates a valid key-pair for this asymmetric encryption algorithm.
        /// </summary>
        /// <remarks>
        /// The data are [=must be] decryptable with the private key when they were encrypted with the public key.
        /// </remarks>
        /// <returns>
        /// Returns a tuple with two byte-arrays.
        /// The first tuple-item represents the private-key.
        /// The second tuple-item represents the public-key.
        /// </returns>
        public abstract (byte[]/*Private key*/, byte[]/*Public key*/) GenerateRandomKeyPair();
    }
}
