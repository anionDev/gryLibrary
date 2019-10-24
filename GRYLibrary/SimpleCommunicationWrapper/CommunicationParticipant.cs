using System.Text;

namespace GRYLibrary.SimpleCommunicationWrapper
{
    public abstract class CommunicationParticipant
    {
        protected Encoding Encoding = new UTF8Encoding(false);
        public string ServerAddress { get; set; }
        public int ServerPort { get; set; }
        public CommunicationParticipant(string serverAddress, int serverPort)
        {
            this.ServerAddress = serverAddress;
            this.ServerPort = serverPort;
        }

        protected byte[] DecryptAsymmetrical(byte[] content, string privateKey)
        {
            return DecryptAsymmetrical(content, privateKey);
        }
        protected byte[] EncryptAsymmetrical(byte[] content, string publicKey)
        {
            return DecryptAsymmetrical(content, publicKey);
        }
        protected byte[] DecryptSymmetrical(byte[] content, string key)
        {
            return Utilities.DecryptSymmetrical(content, key);
        }
        protected byte[] EncryptSymmetrical(byte[] content, string key)
        {
            return Utilities.EncryptSymmetrical(content, key);
        }

    }
}
