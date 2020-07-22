using GRYLibrary.Core.AdvancedObjectAnalysis;
using System;
using System.Xml;
using System.Xml.Schema;

namespace GRYLibrary.TestData.TestTypes.SimpleDataStructure
{
    public class SimpleDataStructure2
    {
        public Guid Guid { get; set; }


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
