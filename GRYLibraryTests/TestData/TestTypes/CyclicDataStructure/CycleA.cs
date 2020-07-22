﻿using GRYLibrary.Core.AdvancedObjectAnalysis;
using System;
using System.Xml;
using System.Xml.Schema;

namespace GRYLibrary.TestData.TestTypes.CyclicDataStructure
{
    [Serializable]
    public class CycleA
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public CycleB B { get; set; }

        internal static CycleA GetRandom()
        {
            CycleA a1 = new CycleA();
            CycleB b1 = new CycleB();
            CycleC c1 = new CycleC();
            CycleA a2 = new CycleA();
            CycleB b2 = new CycleB();
            CycleC c2 = new CycleC();

            a1.B = b1;
            b1.C = c1;
            c1.A.Add(a1);
            c1.A.Add(a2);
            a2.B = b2;
            b2.C = c2;
            c2.A.Add(a1);

            return a1;
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

        public XmlSchema GetSchema()
        {
            return Generic.GenericGetSchema(this);
        }

        public void ReadXml(XmlReader reader)
        {
            Generic.GenericReadXml(this, reader);
        }

        public void WriteXml(XmlWriter writer)
        {
            Generic.GenericWriteXml(this, writer);
        }
        #endregion
    }
}
