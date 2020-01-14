using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GRYLibrary.Core
{
    /// <summary>
    /// Represents a number between 0 and 1.
    /// </summary>
    public class PercentValue : IXmlSerializable
    {
        public int ValueInPercent
        {
            get
            {
                return (int)Math.Round(this.Value * 100);
            }
        }
        public decimal Value { get; }

        public PercentValue(decimal value)
        {
            if (value < 0)
            {
                this.Value = 0;
            }
            else if (value > 1)
            {
                this.Value = 1;
            }
            else
            {
                this.Value = value;
            }
        }
        public PercentValue(int percentValue)
        {
            if (percentValue < 0)
            {
                this.Value = 0;
            }
            else if (percentValue > 100)
            {
                this.Value = 1;
            }
            else
            {
                this.Value = (decimal)percentValue / 100;
            }
        }
        public override bool Equals(object obj)
        {
            return this.Value == ((PercentValue)obj).Value;
        }
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }
        public XmlSchema GetSchema()
        {
            return Utilities.GenericGetXMLSchema(this.GetType());
        }

        public void ReadXml(XmlReader reader)
        {
            Utilities.GenericXMLDeserializer(this, reader);
        }

        public void WriteXml(XmlWriter writer)
        {
            Utilities.GenericXMLSerializer(this, writer);
        }
    }
}
