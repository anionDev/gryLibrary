using System;
namespace GRYLibrary.SimpleCommunicationWrapper
{
    public abstract class CommunicationParticipant
    {
        public string ServerAddress { get; set; }
        public int ServerPort { get; set; }
        public string PublicKeyOfCounterpart { get; set; }
        public string OwnPrivateKey { get; set; }
        public CommunicationParticipant(string serverAddress, int serverPort, string publicKeyOfCounterpart, string ownPrivateKey)
        {
            this.ServerAddress = serverAddress;
            this.ServerPort = serverPort;
            this.PublicKeyOfCounterpart = publicKeyOfCounterpart;
            this.OwnPrivateKey = ownPrivateKey;
        }
        protected byte[] Decrypt(byte[] content, string privateKey)
        {
            throw new NotImplementedException();
        }
        protected byte[] Encrypt(byte[] content, string publicKe)
        {
            throw new NotImplementedException();
        }

    }
}
