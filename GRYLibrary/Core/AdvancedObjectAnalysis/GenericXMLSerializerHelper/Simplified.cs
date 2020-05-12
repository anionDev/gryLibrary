using GRYLibrary.Core.AdvancedObjectAnalysis.GenericXMLSerializerHelper.ConcreteSimplifiedObjects;
using System;
using System.Xml.Serialization;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.GenericXMLSerializerHelper
{
    [XmlInclude(typeof(SEnumerable))]
    [XmlInclude(typeof(SObject))]
    [XmlInclude(typeof(SPrimitive))]
    public abstract class Simplified
    {
        public Guid ObjectId { get; set; }
        public string TypeName { get; set; }
        public override bool Equals(object obj)
        {
            SObject typedObject = obj as SObject;
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
            void Handle(SObject simplifiedObject);
            void Handle(SEnumerable simplifiedEnumerable);
            void Handle(SPrimitive simplifiedPrimitive);
        }
        public interface ISimplifiedVisitor<T>
        {
            T Handle(SObject simplifiedObject);
            T Handle(SEnumerable simplifiedEnumerable);
            T Handle(SPrimitive simplifiedPrimitive);
        }
    }

}
