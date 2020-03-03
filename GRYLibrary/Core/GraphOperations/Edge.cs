using System;

namespace GRYLibrary.Core.GraphOperations
{
    public abstract class Edge
    {
        public string Name { get; set; }
        public double Weight { get; set; }
        public Edge() : this(CalculateEdgeName())
        {
        }
        public Edge(string name, double weight = 1)
        {
            this.Name = name;
            this.Weight = weight;
        }

        private static string CalculateEdgeName()
        {
            return $"{nameof(Edge)}_{Guid.NewGuid().ToString().Substring(0, 8)}";
        }

        public override bool Equals(object obj)
        {
            Edge typedObject = obj as Edge;
            if (typedObject is null)
            {
                return false;
            }
            if (!this.Name.Equals(typedObject.Name))
            {
                return false;
            }
            if (!this.Weight.NullSafeEquals(typedObject.Weight))
            {
                return false;
            }
            return true;
        }
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
        public override string ToString()
        {
            return this.Name;
        }
    }
}
