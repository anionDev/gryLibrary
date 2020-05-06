using System;

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
            Address result = new Address();

            result.AddressLine1 = Utilities.GetRandomAddressLine1();
            result.AddressLine2 = Utilities.GetRandomAddressLine2();
            result.Building = Utilities.GetRandomBuilding();
            result.City = Utilities.GetRandomCity();
            result.CountryRegion = Utilities.GetRandomCountryRegion();
            result.FloorLevel = Utilities.GetRandomFloorLevel();
            result.PostalCode = Utilities.GetRandomPostalCode();
            result.StateProvince = Utilities.GetRandomStateProvince();
            result.Country = Utilities.GetRandomCountry();

            return result;
        }


    }
}
