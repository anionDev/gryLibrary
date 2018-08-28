using System;

namespace GRYLibrary.Property
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
