namespace GRYLibrary.SimpleCommunicationWrapper.CommunicationObject
{
    internal abstract class CommunicationObject
    {
        public string SenderAddress { get; set; }
        public string ReceiverAddress { get; set; }
        internal abstract void Accept(ICommunicationObjectVisitor visitor);
        internal abstract T Accept<T>(ICommunicationObjectVisitor<T> visitor);
        internal static byte[] ToBytes(CommunicationObject communicationObject)
        {
            throw new System.NotImplementedException();
        }
        internal static CommunicationObject FromBytes(byte[] communicationObject)
        {
            throw new System.NotImplementedException();
        }
    }
    internal interface ICommunicationObjectVisitor
    {
        void Handle(EncryptedObject communicationObject);
        void Handle(InizializationAnswer communicationObject);
        void Handle(InitializationRequest communicationObject);
    }
    internal interface ICommunicationObjectVisitor<T>
    {
        T Handle(EncryptedObject communicationObject);
        T Handle(InizializationAnswer communicationObject);
        T Handle(InitializationRequest communicationObject);
    }
}
