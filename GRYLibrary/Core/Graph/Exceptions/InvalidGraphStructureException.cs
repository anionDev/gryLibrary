using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Core.Graph.Exceptions
{
    public class InvalidGraphStructureException : Exception
    {
        public InvalidGraphStructureException(string message) : base(message)
        {
        }
    }
}
