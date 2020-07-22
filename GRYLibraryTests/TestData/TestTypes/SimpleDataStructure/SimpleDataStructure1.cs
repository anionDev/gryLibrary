using GRYLibrary.Core.AdvancedObjectAnalysis;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace GRYLibrary.TestData.TestTypes.SimpleDataStructure
{
    public class SimpleDataStructure1
    {
        public ISet<SimpleDataStructure3> Property1 { get; set; }
        public SimpleDataStructure2 Property2 { get; set; }
        public int Property3 { get; set; }

        public static SimpleDataStructure1 GetRandom()
        {
            SimpleDataStructure1 result = new SimpleDataStructure1
            {
                Property1 = new HashSet<SimpleDataStructure3>
                {
                    SimpleDataStructure3.GetRandom(),
                    SimpleDataStructure3.GetRandom(),
                    SimpleDataStructure3.GetRandom()
                },
                Property2 = new SimpleDataStructure2() { Guid = Guid.Parse("3735ece2-942f-4380-aec4-27aaa4021ed5") },
                Property3 = 21
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
