using GRYLibrary.SimpleCommunicationWrapper.CommunicationObject;
using System;

namespace GRYLibrary.SimpleCommunicationWrapper
{
    public class Client : CommunicationParticipant
    {
        private string _SharedSecret = null;
        public string PublicKeyOfServer { get; set; }
        public Client(string serverAddress, int serverPort, string publicKeyOfServer) : base(serverAddress, serverPort)
        {
            this.PublicKeyOfServer = publicKeyOfServer;
        }
        public byte[] Send(byte[] content)
        {
            return this.SendDataToServer(new EncryptedObject(this.EncryptSymmetrical(content, this.GetSharedSecret()))).Accept(this._AnswerVisitor);
        }
        private readonly ICommunicationObjectVisitor<byte[]> _AnswerVisitor = new AnswerVisitor();

        private class AnswerVisitor : ICommunicationObjectVisitor<byte[]>
        {
            public byte[] Handle(EncryptedObject communicationObject)
            {
                return communicationObject.UnencryptedContent;
            }

            public byte[] Handle(InizializationAnswer communicationObject)
            {
                throw new NotImplementedException();
            }

            public byte[] Handle(InitializationRequest communicationObject)
            {
                throw new NotImplementedException();
            }
        }
        private CommunicationObject.CommunicationObject SendDataToServer(CommunicationObject.CommunicationObject content)
        {
            byte[] data = CommunicationObject.CommunicationObject.ToBytes(content);
            throw new NotImplementedException();
        }
        private string GetSharedSecret()
        {
            if (this._SharedSecret == null)
            {
                this.InitializeSharedSecret();
            }
            return this._SharedSecret;
        }

        private void InitializeSharedSecret()
        {
            this._SharedSecret = Guid.NewGuid().ToString();
            this.SendDataToServer(new InitializationRequest(this.Encoding.GetString(this.EncryptAsymmetrical(this.Encoding.GetBytes(this._SharedSecret), this.PublicKeyOfServer))));
        }
    }
}
