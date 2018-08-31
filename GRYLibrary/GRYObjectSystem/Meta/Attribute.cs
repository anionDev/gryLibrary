namespace GRYLibrary.GRYObjectSystem.Meta
{
    internal sealed class Attribute
    {
        public Type TargetObjectType { get; }
        public string Name { get; }
        public Attribute(string name, Type targetObjectType)
        {
            this.Name = name;
            this.TargetObjectType = targetObjectType;
        }

    }
}
