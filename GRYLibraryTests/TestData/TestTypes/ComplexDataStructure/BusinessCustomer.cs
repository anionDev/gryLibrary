using System;

namespace GRYLibrary.TestData.TestTypes.ComplexDataStructure
{
    public class BusinessCustomer : ICustomer
    {
        public string CompanyName { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string Website { get; set; }
        public Address MainResidence { get; set; }

        internal static ICustomer GetRandom()
        {
            BusinessCustomer result = new BusinessCustomer();
            result.CompanyName = Utilities.GetRandomName();
            result.Website = Utilities.GetRandomWebsite();
            result.ContactPhoneNumber = Utilities.GetRandomPhoneNumber();
            result.MainResidence = Address.GetRandom();
            return result;
        }
    }
}
