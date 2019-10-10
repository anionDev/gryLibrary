namespace GRYLibrary.SimpleCommunicationWrapper.CommunicationObject
{
    internal class InitializationRequest : CommunicationObject
    {
        public string EncryptedSecret { get; set; }
        public InitializationRequest(string encryptedSecret)
        {
            this.EncryptedSecret = encryptedSecret;
        }
        internal override void Accept(ICommunicationObjectVisitor visitor)
        {
            visitor.Handle(this);
        }

        internal override T Accept<T>(ICommunicationObjectVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
    }
}
