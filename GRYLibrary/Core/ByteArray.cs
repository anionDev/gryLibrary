using GRYLibrary.Core.XMLSerializer;
using System;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GRYLibrary.Core
{
    public class ByteArray:IXmlSerializable
    {
        public static Encoding DefaultEncoding { get; } = new UTF8Encoding(false);
        public byte[] Data { get; }
        public string DataAsHexString { get; }
        public ByteArray(byte[] value)
        {
            this.Data = value ?? throw new Exception("No data for byte-array available.");
            this.DataAsHexString = Utilities.ByteArrayToHexString(this.Data);
        }
        public ByteArray CreateByHexString(string value)
        {
            return new ByteArray(Utilities.HexStringToByteArray(value));
        }
        public ByteArray CreateByInteger(BigInteger value)
        {
            return this.CreateByHexString(Utilities.IntegerToHexString(value));
        }
        public ByteArray CreateByString(string value)
        {
            return this.CreateByString(value, DefaultEncoding);
        }
        public ByteArray CreateByString(string value, Encoding encoding)
        {
            return new ByteArray(encoding.GetBytes(value));
        }
        public string DecodeToString()
        {
            return this.DecodeToString(DefaultEncoding);
        }
        public string DecodeToString(Encoding encoding)
        {
            return encoding.GetString(this.Data);
        }
        public static ByteArray operator +(ByteArray first, ByteArray second)
        {
            return new ByteArray(Utilities.Concat(first.Data, second.Data));
        }
        public static bool operator ==(ByteArray first, ByteArray second)
        {
            return first.Equals(second);
        }
        public static bool operator !=(ByteArray first, ByteArray second)
        {
            return !(first == second);
        }
        public override bool Equals(object obj)
        {
            ByteArray other = obj as ByteArray;
            return other != null && Enumerable.SequenceEqual(this.Data, other.Data);
        }
        public override int GetHashCode()
        {
            return this.Data.GetHashCode();
        }
        public bool StartsWith(ByteArray value)
        {
            if (value.Data.LongLength > this.Data.LongLength)
            {
                return false;
            }
            for (long i = 0; i < value.Data.LongLength; i++)
            {
                if (this.Data[i] != value.Data[i])
                {
                    return false;
                }
            }
            return true;
        }
        public bool Contains(ByteArray value)
        {
            if (value.Data.LongLength > this.Data.LongLength)
            {
                return false;
            }
            return Encoding.ASCII.GetString(this.Data).Contains(Encoding.ASCII.GetString(value.Data));
        }
        public XmlSchema GetSchema()
        {
            return new CustomizableXMLSerializer().GenericGetXMLSchema(this.GetType());
        }

        public void ReadXml(XmlReader reader)
        {
            new CustomizableXMLSerializer().GenericXMLDeserializer(this, reader);
        }

        public void WriteXml(XmlWriter writer)
        {
            new CustomizableXMLSerializer().GenericXMLSerializer(this, writer);
        }
    }
}
