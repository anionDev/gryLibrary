using GRYLibrary.Core.AdvancedObjectAnalysis.GenericXMLSerializerHelper.ConcreteSimplifiedObjects;
using System;
using System.Xml.Serialization;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.GenericXMLSerializerHelper
{
    [XmlInclude(typeof(SimplifiedEnumerable))]
    [XmlInclude(typeof(SimplifiedObject))]
    [XmlInclude(typeof(SimplifiedPrimitive))]
    public abstract class Simplified
    {
        public Guid ObjectId { get; set; }
        public string TypeName { get; set; }
        public override bool Equals(object obj)
        {
            SimplifiedObject typedObject = obj as SimplifiedObject;
            if (typedObject == null)
            {
                return false;
            }
            else
            {
                return this.ObjectId.Equals(typedObject.ObjectId);
            }
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(this.ObjectId);
        }
        public abstract void Accept(ISimplifiedVisitor visitor);
        public abstract T Accept<T>(ISimplifiedVisitor<T> visitor);
        public interface ISimplifiedVisitor
        {
            void Handle(SimplifiedObject simplifiedObject);
            void Handle(SimplifiedEnumerable simplifiedEnumerable);
            void Handle(SimplifiedPrimitive simplifiedPrimitive);
        }
        public interface ISimplifiedVisitor<T>
        {
            T Handle(SimplifiedObject simplifiedObject);
            T Handle(SimplifiedEnumerable simplifiedEnumerable);
            T Handle(SimplifiedPrimitive simplifiedPrimitive);
        }
    }

}
