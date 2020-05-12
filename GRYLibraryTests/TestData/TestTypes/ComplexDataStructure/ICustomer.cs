using System.Collections.Generic;

namespace GRYLibrary.TestData.TestTypes.ComplexDataStructure
{
    public interface ICustomer
    {
        public string ContactPhoneNumber { get; set; }
        public Address MainResidence { get; set; }
    }
}
