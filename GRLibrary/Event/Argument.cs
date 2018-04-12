namespace GRLibrary.Event
{
    public class Argument<SenderType, EventArgumentType>
    {
        public EventArgumentType ArgumentObject { get; }
        public SenderType Sender { get; private set; }

        public Argument(SenderType sender, EventArgumentType argument)
        {
            this.ArgumentObject = argument;
            this.Sender = sender;
        }
    }
}
