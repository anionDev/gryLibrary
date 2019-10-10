namespace GRYLibrary.SimpleCommunicationWrapper.CommunicationObject
{
    internal class InizializationAnswer: CommunicationObject
    {
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
