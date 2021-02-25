using System;
using System.Xml.Serialization;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.GenericXMLSerializerHelper.ConcreteSimplifiedObjects
{
    /// <summary>
    /// Represents a SimplifiedAttribute for <see cref="GRYSObject"/>
    /// </summary>
    [XmlRoot(ElementName = "FA")]
    public class FlatAttribute
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public Guid ObjectId { get; set; }
    }

}
