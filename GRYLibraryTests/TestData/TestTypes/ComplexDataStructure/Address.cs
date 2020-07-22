using GRYLibrary.Core.AdvancedObjectAnalysis;

namespace GRYLibrary.TestData.TestTypes.ComplexDataStructure
{
    public class Address
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Building { get; set; }
        public string City { get; set; }
        public string CountryRegion { get; set; }
        public string FloorLevel { get; set; }
        public string PostalCode { get; set; }
        public string StateProvince { get; set; }
        public string Country { get; set; }

        public static Address GetRandom()
        {
            Address result = new Address
            {
                AddressLine1 = Utilities.GetRandomAddressLine1(),
                AddressLine2 = Utilities.GetRandomAddressLine2(),
                Building = Utilities.GetRandomBuilding(),
                City = Utilities.GetRandomCity(),
                CountryRegion = Utilities.GetRandomCountryRegion(),
                FloorLevel = Utilities.GetRandomFloorLevel(),
                PostalCode = Utilities.GetRandomPostalCode(),
                StateProvince = Utilities.GetRandomStateProvince(),
                Country = Utilities.GetRandomCountry()
            };

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
