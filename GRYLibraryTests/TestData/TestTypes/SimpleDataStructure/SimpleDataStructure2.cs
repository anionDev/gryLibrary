using System;

namespace GRYLibrary.TestData.TestTypes.SimpleDataStructure
{
    public class SimpleDataStructure2
    {
        public Guid Guid { get; set; }

        public override bool Equals(object obj)
        {
            return obj is SimpleDataStructure2 structure &&
                   this.Guid.Equals(structure.Guid);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Guid);
        }
        public override string ToString()
        {
            return $"{nameof(SimpleDataStructure2)}{{{nameof(Guid)}={Guid}}}";
        }
    }

}
