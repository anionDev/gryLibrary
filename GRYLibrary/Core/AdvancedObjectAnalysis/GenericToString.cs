using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Core.AdvancedObjectAnalysis
{
    public class GenericToString
    {
        private GenericToString() { }
        public static GenericToString Instance { get; } = new GenericToString();

        internal string ToString(object @object)
        {
            throw new NotImplementedException();
        }
    }
}
