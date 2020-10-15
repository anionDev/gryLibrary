﻿using System;
using GRYLibrary.Core.AdvancedObjectAnalysis;


namespace GRYLibrary.TestData.TestTypes.ComplexDataStructure
{
    public class OrganizationUnit
    {
        public Employee Boss { get; set; }
        public OrganizationUnit SuperiorOrganizationUnit { get; set; }

        public string Name { get; set; }

        public static OrganizationUnit GetRandom(Employee boss, OrganizationUnit superiorOrganizationUnit = null)
        {
            OrganizationUnit result = new OrganizationUnit();
            result.Name = GetRandomName();
            result.Boss = boss;
            result.SuperiorOrganizationUnit = superiorOrganizationUnit;
            return result;
        }

        private static string GetRandomName()
        {
            return Guid.NewGuid().ToString();
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