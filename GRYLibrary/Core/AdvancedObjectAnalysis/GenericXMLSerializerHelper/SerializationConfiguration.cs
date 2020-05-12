using System;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.GenericXMLSerializerHelper
{
    public class SerializationConfiguration
    {
        public Func<PropertyInfo, bool> PropertySelector { get; set; }
        public Func<FieldInfo, bool> FieldSelector { get; set; }
        public XmlSerializer XmlSerializer { get; set; }
        public Encoding Encoding { get; set; }
        public bool Indent { get; set; } = true;
    }
}
