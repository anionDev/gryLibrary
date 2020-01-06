namespace GRYLibrary.GRYProperty.Exceptions
{
    public class SetNotAllowedException : System.Exception
    {
        internal SetNotAllowedException(string message) : base(message)
        {
        }
        internal SetNotAllowedException() : base()
        {
        }
    }
}
