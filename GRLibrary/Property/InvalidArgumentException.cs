using System;

namespace GRLibrary.Property
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
