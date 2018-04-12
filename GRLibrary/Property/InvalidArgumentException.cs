using System;

namespace GRLibrary.Property
{
    public class InvalidArgumentException : Exception
    {
        public InvalidArgumentException(string message) : base(message)
        {
        }
        public InvalidArgumentException() : base()
        {
        }
    }
}
