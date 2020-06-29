﻿using GRYLibrary.Core.AdvancedObjectAnalysis;
using System;
using System.Collections.Generic;

namespace GRYLibrary.TestData.TestTypes.SimpleDataStructure
{
    public class SimpleDataStructure3
    {
        public string Property4 { get; set; }
        public List<SimpleDataStructure2> Property5 { get; set; }

        internal static SimpleDataStructure3 GetTestObject()
        {
            SimpleDataStructure3 result = new SimpleDataStructure3();

            result.Property4 = "Property4_e7df34db-bb6f-4a11-8c6d-66bccafbd041";
            result.Property5 = new List<SimpleDataStructure2>();
            result.Property5.Add(new SimpleDataStructure2() { Guid = Guid.Parse("a54f4945-e928-4296-bf9b-e9ae16b35744") });
            result.Property5.Add(new SimpleDataStructure2() { Guid = Guid.Parse("1735ece2-942f-4380-aec4-27aaa4021ed5") });
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