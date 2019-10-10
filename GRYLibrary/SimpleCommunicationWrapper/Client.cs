using System;

namespace GRYLibrary.SimpleCommunicationWrapper
{
    public class Client : CommunicationParticipant
    {
        public Client(string serverAddress, int serverPort, string publicKeyOfCounterpart, string ownPrivateKey) : base(serverAddress, serverPort, publicKeyOfCounterpart, ownPrivateKey)
        {
        }
        public byte[] Send(byte[] content)
        {
            return this.Decrypt(this.SendDataToServer(this.Encrypt(content, this.PublicKeyOfCounterpart)), this.OwnPrivateKey);
        }

        private byte[] SendDataToServer(byte[] content)
        {
            throw new NotImplementedException();
        }
    }
}
