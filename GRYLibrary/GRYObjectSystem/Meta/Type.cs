using System.Collections.Generic;

namespace GRYLibrary.GRYObjectSystem.Meta
{
    public sealed class Type
    {
        public IList<Type> SuperTypes { get; } = new List<Type>();
        public IList<Attribute> Attributes { get; } = new List<Attribute>();
        public IList<Operation> Operations { get; } = new List<Operation>();
        public string Name { get; }
        public Type(string name)
        {
            this.Name = name;
        }
    }
}
