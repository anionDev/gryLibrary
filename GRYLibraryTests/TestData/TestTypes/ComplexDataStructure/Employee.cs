using System;
using GRYLibrary.Core.AdvancedObjectAnalysis;


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
            result.Forename =ComplexDataStructureUtilities. GetRandomName();
            result.Surname = ComplexDataStructureUtilities.GetRandomName();
            result.DateOfBirth = ComplexDataStructureUtilities.GetRandomDateOfBirth();
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
