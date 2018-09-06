using GRYLibrary.GRYObjectSystem.Meta.Property;
using System.Collections.Generic;

namespace GRYLibrary.GRYObjectSystem.Meta
{
    internal sealed class Object
    {
        public IList<IProperty> Properties { get; } = new List<IProperty>();
        public IList<Object> ParentTypeObjects { get; } = new List<Object>();
        public Type Type { get; }
        public Object(Type type)
        {
            this.Type = type;
        }
    }
}
