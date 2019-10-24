using System;
using System.Collections.Generic;
using GRYLibrary.SimpleCommunicationWrapper.CommunicationObject;

namespace GRYLibrary.SimpleCommunicationWrapper
{
    public class Server : CommunicationParticipant
    {
        private readonly IDictionary<string/*clientAddress*/, string/*secretSharedWithThisClient*/> _SharedSecrets = new Dictionary<string, string>();
        public Func<byte[], byte[]> CalculateAnswerFunction { get; set; }
        public string PrivateKey { get; set; }
        public Server(string serverAddress, int serverPort, Func<byte[], byte[]> calculateAnswerFunction, string privateKey) : base(serverAddress, serverPort)
        {
            this.CalculateAnswerFunction = calculateAnswerFunction;
            this.PrivateKey = privateKey;
            this._AnswerVisitor = new AnswerVisitor(this);
        }
        private void Receive(byte[] content)
        {
            try
            {
                this.SendDataBackToClient(CommunicationObject.CommunicationObject.FromBytes(content).Accept(this._AnswerVisitor));
            }
            catch
            {
                throw new NotImplementedException();
            }
        }
        private readonly ICommunicationObjectVisitor<CommunicationObject.CommunicationObject> _AnswerVisitor = null;
        private class AnswerVisitor : ICommunicationObjectVisitor<CommunicationObject.CommunicationObject/*Answer-object*/>
        {
            private readonly Server _Server;

            public AnswerVisitor(Server server)
            {
                this._Server = server;
            }

            public CommunicationObject.CommunicationObject Handle(EncryptedObject communicationObject)
            {
                string sharedSecret = this._Server.GetSharedSecret(communicationObject.SenderAddress);
                return new EncryptedObject(this._Server.EncryptSymmetrical(this._Server.CalculateAnswerFunction(this._Server.DecryptSymmetrical(communicationObject.UnencryptedContent, sharedSecret)), sharedSecret));
            }

            public CommunicationObject.CommunicationObject Handle(InizializationAnswer communicationObject)
            {
                throw new NotImplementedException();
            }

            public CommunicationObject.CommunicationObject Handle(InitializationRequest communicationObject)
            {
                this._Server._SharedSecrets.Add(communicationObject.SenderAddress, this._Server.Encoding.GetString(this._Server.DecryptAsymmetrical(this._Server.Encoding.GetBytes(communicationObject.EncryptedSecret), this._Server.PrivateKey)));
                return new InizializationAnswer();
            }
        }

        private string GetSharedSecret(string cliendAddress)
        {
            return this._SharedSecrets[cliendAddress];
        }
        private void SendDataBackToClient(CommunicationObject.CommunicationObject content)
        {
            byte[] data = CommunicationObject.CommunicationObject.ToBytes(content);
            throw new NotImplementedException();
        }
    }
}
