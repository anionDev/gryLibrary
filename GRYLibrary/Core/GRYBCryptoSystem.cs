using System;

namespace GRYLibrary.Core
{
    /// <summary>
    /// The <see cref="GRYBCryptoSystem"/> is a cryptosystem to encrypt data with an optional masterkey to be able to decrypt the data without the password.
    /// The "B" in the name stands for "backdoor" since the masterpassword is obviously a backdoor.
    /// The purpose of this cryptosystem is to be able to encrypt data conformable to law when you have the requirement that a third party (and only this third party) must be able to decrypt the data.
    /// </summary>
    /// <example>
    /// Example-usage:
    /// Company c want to encrypt userdata with a user-specific password p. p can be defined by the user or a user-client-side generated password. c does neither have to know p nor mp.
    /// It is a statutory requirement that the Secret service s can also decrypt the data.
    /// So s creates a master password mp (for example with <see cref="Utilities.GetRandomByteArray(long)"/>.
    /// Then s calls <see cref="GenerateMasterPasswordHash"/>(mp) to generate the master password hash mph.
    /// Then s gives mph to c and c calls <see cref="GRYBCryptoSystem"/>(mph) to create an <see cref="GRYBCryptoSystem"/>-object gcs.
    /// Now c can encrypt and decrypt data with gcs.<see cref="Encrypt(byte[], byte[])"/> and gcs.<see cref="Decrypt(byte[], byte[])"/>.
    /// The encrypted data can be decrypted either with the password used when calling <see cref="Encrypt(byte[], byte[])"/> or (by s) using mp as password.
    /// </example>
    public class GRYBCryptoSystem
    {
        private readonly byte[] _MasterPasswordHash;

        /// <summary>
        /// Use this constructor when you do not want to be able to decrypt the data with a master password
        /// </summary>
        public GRYBCryptoSystem() : this(GenerateMasterPasswordHash(Utilities.GetRandomByteArray()))
        {
        }

        /// <summary>
        /// Use this constructor when you do not to be able to decrypt the data with a master password
        /// </summary>
        public GRYBCryptoSystem(byte[] masterPasswordHash)
        {
            this._MasterPasswordHash = masterPasswordHash;
        }
        public static byte[] GenerateMasterPasswordHash(byte[] masterPassword)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Use this function to encrypt data.
        /// </summary>
        public byte[] Encrypt(byte[] unencryptedData, byte[] password)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Use this function to decrypt data.
        /// </summary>
        /// <param name="password">
        /// As password you can either use the password which was used when calling <see cref="GRYBCryptoSystem.Encrypt"/> or you can use the result of <see cref="GenerateMasterPasswordHash"/>(masterPassword).
        /// </param>
        public byte[] Decrypt(byte[] encryptedData, byte[] password)
        {
            throw new NotImplementedException();
        }

    }

}
