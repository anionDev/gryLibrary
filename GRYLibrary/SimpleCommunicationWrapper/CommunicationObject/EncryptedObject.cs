namespace GRYLibrary.SimpleCommunicationWrapper.CommunicationObject
{
    internal class EncryptedObject : CommunicationObject
    {
        public EncryptedObject(byte[] data)
        {
            this.UnencryptedContent = data;
        }
        public byte[] UnencryptedContent { get; set; }

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
