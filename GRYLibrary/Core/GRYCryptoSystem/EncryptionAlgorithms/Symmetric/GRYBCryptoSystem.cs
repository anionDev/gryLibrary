using GRYLibrary.Core.GRYCryptoSystem.EncryptionAlgorithms.Asymmetric;
using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.GRYCryptoSystem.EncryptionAlgorithms.Symmetric
{
    /// <summary>
    /// The <see cref="GRYBCryptoSystem"/> is a cryptosystem to encrypt data with an optional masterkey to be able to decrypt the data without the password.
    /// The "B" in the name stands for "backdoor" since the masterpassword is obviously a backdoor.
    /// The purpose of this cryptosystem is to be able to encrypt data conformable to law when you have the requirement that a third party (and only this third party) must be able to decrypt the data.
    /// </summary>
    /// <remarks>
    /// The size of the entire encrypted data is:
    /// length(encrypted payload data)+(little overhead)
    /// (little overhead)=sum(length(publickey) for each publickey in <see cref="PasswordEncryptionKeys"/>)+(some very small technical overhead)
    /// So if there are n keys available in <see cref="PasswordEncryptionKeys"/> then the size of the entire encrypted data is not n*(encrypted payload data)+(little overhead) but 1*(encrypted payload data)+(little overhead).
    /// </remarks>
    /// <example>
    /// Example-usage:
    /// It is a statutory requirement that the Secret service s can also decrypt the data.
    /// The secret service creates a master key mk (mk is technically a private key for an RSA-keypair).
    /// Company c want to encrypt userdata with a user-specific password p. p can be defined by the c or by the user or can be a user-client-side generated password. (p is technically a private key for an RSA-keypair.) c does neither have to know p nor mk.
    /// Then the public keys of mk and p will be added to <see cref="PasswordEncryptionKeys"/>.
    /// Now c can encrypt and decrypt data with gcs.<see cref="Encrypt(byte[], byte[])"/> and gcs.<see cref="Decrypt(byte[], byte[])"/>.
    /// The encrypted data can be decrypted either with the password used when calling <see cref="Encrypt(byte[], byte[])"/> or (by s) using mp as password.
    /// (In General: The data can be decrypted with all keys which are contained in <see cref="PasswordEncryptionKeys"/> when <see cref="Encrypt(byte[], byte[])"/> is called.)
    /// </example>
    public class GRYBCryptoSystem : ISymmetricEncryptionAlgorithm
    {
        public ISet<byte[]> PasswordEncryptionKeys { get; set; } = new HashSet<byte[]>();
        public ISymmetricEncryptionAlgorithm InternalEncryptionAlgorithmForPayload { get; set; } = new AES256();
        public IAsymmetricEncryptionAlgorithm InternalEncryptionAlgorithmForKeys { get; set; } = new RSA();

        /// <inheritdoc/>
        public byte[] Encrypt(byte[] unencryptedData, byte[] password)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        /// <param name="password">
        /// As password you can either use any private key of a public key which was contained in <see cref="PasswordEncryptionKeys"/> when calling <see cref="Encrypt(byte[], byte[])"/>.
        /// </param>
        public byte[] Decrypt(byte[] encryptedData, byte[] password)
        {
            throw new NotImplementedException();
        }
        public static GRYBCryptoSystem CreateGRYBCryptoSystemForCommonUsage(byte[] publicKeyOfMasterKey, byte[] publicKeyOfuserPassword)
        {
            GRYBCryptoSystem result = new GRYBCryptoSystem();
            result.PasswordEncryptionKeys.Add(publicKeyOfMasterKey);
            result.PasswordEncryptionKeys.Add(publicKeyOfuserPassword);
            return result;
        }
        /// <inheritdoc/>
        /// <remarks>
        /// <see cref="GRYBCryptoSystem"/> is a symmtric encryption algorithm which uses an asymmtric encryption algorithm internally.
        /// When calling this function only the private key of the internally used asymmtric encryption algorithm will be returned and the public key will be added to <see cref="PasswordEncryptionKeys"/>.
        /// Technically there is no need for this public key to be returned.
        /// </remarks>
        public byte[] GenerateRandomKey()
        {
            (byte[]/*Private key*/, byte[]/*Public key*/) keyPair= InternalEncryptionAlgorithmForKeys.GenerateRandomKeyPair();
            PasswordEncryptionKeys.Add(keyPair.Item2);
            return keyPair.Item1;

        }
    }

}
