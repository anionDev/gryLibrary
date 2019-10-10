using System;

namespace GRYLibrary.SimpleCommunicationWrapper
{
    public class Server : CommunicationParticipant
    {
        public Func<byte[], byte[]> CalculateAnswerFunction { get; set; }
        public Server(string serverAddress, int serverPort, string publicKeyOfCounterpart, string ownPrivateKey, Func<byte[], byte[]> calculateAnswerFunction) : base(serverAddress, serverPort, publicKeyOfCounterpart, ownPrivateKey)
        {
            this.CalculateAnswerFunction = calculateAnswerFunction;
        }
        public void Receive(byte[] content)
        {
            this.AnswerDataToClient(this.Encrypt(this.CalculateAnswerFunction(this.Decrypt(content, this.OwnPrivateKey)), this.PublicKeyOfCounterpart));
        }

        private void AnswerDataToClient(byte[] content)
        {
            throw new NotImplementedException();
        }
    }
}
