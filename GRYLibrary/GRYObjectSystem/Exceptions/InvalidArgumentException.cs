using System;

namespace GRYLibrary.GRYObjectSystem.Exceptions
{
    public class InvalidArgumentException : Exception
    {
        internal InvalidArgumentException(string message) : base(message)
        {
        }
        internal InvalidArgumentException() : base()
        {
        }
    }
}
