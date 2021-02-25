using GRYLibrary.Core.Miscellaneous;
using System.IO;
using System.Security.Cryptography;

namespace GRYLibrary.Core.CryptoSystems.EncryptionAlgorithms.ConcreteCommonAlgorithms.ConcreteSymmetricAlgorithms
{
    public class AES256 : SymmetricEncryptionAlgorithm
    {
        private const int _AESBlockLength = 16;
        /// <inheritdoc/>
        public override byte[] Decrypt(byte[] encryptedData, byte[] password)
        {
            (byte[], byte[]) splitted = Utilities.Split(encryptedData, _AESBlockLength);
            byte[] iv = splitted.Item1;
            encryptedData = splitted.Item2;
            using MemoryStream result = new MemoryStream();
            using (RijndaelManaged algorithmImplementation = new RijndaelManaged())
            {
                algorithmImplementation.Key = password;
                algorithmImplementation.IV = iv;
                algorithmImplementation.Mode = CipherMode.CBC;
                algorithmImplementation.Padding = PaddingMode.Zeros;
                ICryptoTransform decryptor = algorithmImplementation.CreateDecryptor(algorithmImplementation.Key, algorithmImplementation.IV);
                using MemoryStream memoryStream = new MemoryStream(encryptedData);
                using CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                byte[] buffer = new byte[_AESBlockLength];
                int read = cryptoStream.Read(buffer, 0, buffer.Length);
                while (0 < read)
                {
                    result.Write(buffer, 0, read);
                    read = cryptoStream.Read(buffer, 0, buffer.Length);
                }
                cryptoStream.Flush();
            }
            return result.ToArray();
        }

        /// <inheritdoc/>
        public override byte[] Encrypt(byte[] unencryptedData, byte[] password)
        {
            byte[] iv = GetIV();
            byte[] encrypted;
            using (RijndaelManaged algorithmImplementation = new RijndaelManaged())
            {
                algorithmImplementation.Key = password;
                algorithmImplementation.IV = iv;
                algorithmImplementation.Mode = CipherMode.CBC;
                algorithmImplementation.Padding = PaddingMode.Zeros;
                ICryptoTransform encryptor = algorithmImplementation.CreateEncryptor(algorithmImplementation.Key, algorithmImplementation.IV);
                using MemoryStream memoryStream = new MemoryStream();
                using CryptoStream csEncrypt = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
                using (StreamWriter streamWriter = new StreamWriter(csEncrypt))
                {
                    streamWriter.Write(unencryptedData);
                }
                encrypted = memoryStream.ToArray();
            }
            return Utilities.Concat(iv, encrypted);
        }

        private byte[] GetIV()
        {
            using RijndaelManaged algorithmImplementation = new RijndaelManaged();
            algorithmImplementation.GenerateIV();
            if (algorithmImplementation.IV.Length != _AESBlockLength)
            {
                throw new InvalidDataException($"Expected length of IV to be {_AESBlockLength} but was {algorithmImplementation.IV.Length }");
            }
            return algorithmImplementation.IV;
        }

        /// <inheritdoc/>
        public override byte[] GenerateRandomKey()
        {
            using RijndaelManaged algorithmImplementation = new RijndaelManaged();
            algorithmImplementation.GenerateKey();
            return algorithmImplementation.Key;
        }
        public override byte[] GetIdentifier()
        {
            return Utilities.PadLeft(System.Text.Encoding.ASCII.GetBytes(nameof(AES256)), 10);
        }
    }
}
