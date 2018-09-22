namespace GRYLibrary.GRYObjectSystem.Exceptions
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
