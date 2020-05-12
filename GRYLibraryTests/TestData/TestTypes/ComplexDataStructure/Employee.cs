using GRYLibrary.Core;
using System;

namespace GRYLibrary.TestData.TestTypes.ComplexDataStructure
{
    public class Employee : IPerson
    {
        public string Forename { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Address MainResidence { get; set; }
        public OrganizationUnit OrganizationUnit { get; set; }

        public static Employee GetRandom(OrganizationUnit organizationUnit = null)
        {
            Employee result = new Employee();
            result.OrganizationUnit = organizationUnit;
            result.Forename =Utilities. GetRandomName();
            result.Surname = Utilities.GetRandomName();
            result.DateOfBirth = Utilities.GetRandomDateOfBirth();
            result.MainResidence = Address.GetRandom();
            return result;
        }

  
    }
}
