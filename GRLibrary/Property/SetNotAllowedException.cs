namespace GRLibrary.Property
{
    public class SetNotAllowedException : System.Exception
    {
        public SetNotAllowedException(string message) : base(message)
        {
        }
        public SetNotAllowedException() : base()
        {
        }
    }
}
