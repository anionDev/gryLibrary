using GRYLibrary.Core.AdvancedObjectAnalysis;

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
            result.CompanyName = ComplexDataStructureUtilities.GetRandomName();
            result.Website = ComplexDataStructureUtilities.GetRandomWebsite();
            result.ContactPhoneNumber = ComplexDataStructureUtilities.GetRandomPhoneNumber();
            result.MainResidence = Address.GetRandom();
            return result;
        }
        #region Overhead
        public override bool Equals(object @object)
        {
            return Generic.GenericEquals(this, @object);
        }
        public override int GetHashCode()
        {
            return Generic.GenericGetHashCode(this);
        }
        public override string ToString()
        {
            return Generic.GenericToString(this);
        }
        #endregion
    }
}
