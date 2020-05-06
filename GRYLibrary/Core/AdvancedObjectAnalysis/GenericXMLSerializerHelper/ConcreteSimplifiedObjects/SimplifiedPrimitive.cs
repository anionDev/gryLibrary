namespace GRYLibrary.Core.AdvancedObjectAnalysis.GenericXMLSerializerHelper.ConcreteSimplifiedObjects
{
    public class SimplifiedPrimitive : Simplified
    {
        public object Value { get; set; }
        public override void Accept(ISimplifiedVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(ISimplifiedVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
    }
}
