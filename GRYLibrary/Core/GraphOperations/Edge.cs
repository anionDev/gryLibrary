using System;

namespace GRYLibrary.Core.GraphOperations
{
    [Serializable]
    public class Edge
    {
        public string Name { get; set; }
        public double Weight { get; set; }
        /// <remarks>
        /// In the context of <see cref="UndirectedGraph"/>-objects it does not matter which <see cref="Vertex"/>- object is marked as <see cref="Source"/> and which is marked as <see cref="Target"/>.
        /// </remarks>
        public Vertex Source { get; internal set; }
        /// <remarks>
        /// In the context of <see cref="UndirectedGraph"/>-objects it does not matter which <see cref="Vertex"/>- object is marked as <see cref="Source"/> and which is marked as <see cref="Target"/>.
        /// </remarks>
        public Vertex Target { get; internal set; }
        public Edge() : this(CalculateEdgeName())
        {
        }
        public Edge(string name)
        {
            this.Name = name;
        }
        public Edge(Vertex source, Vertex target, double weight = 1) : this(source, target, CalculateEdgeName(), weight)
        {
        }

        private static string CalculateEdgeName()
        {
            return $"{nameof(Edge)}_{Guid.NewGuid().ToString().Substring(0, 8)}";
        }

        public Edge(Vertex source, Vertex target, string name, double weight = 1)
        {
            this.Source = source;
            this.Target = target;
            this.Name = name;
            this.Weight = weight;
        }
        public override bool Equals(object obj)
        {
            Edge typedObject = obj as Edge;
            if (typedObject is null)
            {
                return false;
            }
            return this == typedObject;
        }
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Two edges are equal if and only if their <see cref="Name"/>s, <see cref="Source"/>s and <see cref="Target"/>s are equal.
        /// </summary>
        public static bool operator ==(Edge obj1, Edge obj2)
        {
            if (!obj1.Name.NullSafeEquals(obj2.Name))
            {
                return false;
            }
            if (!obj1.Source.NullSafeEquals(obj2.Source))
            {
                return false;
            }
            if (!obj1.Target.NullSafeEquals(obj2.Target))
            {
                return false;
            }
            return true;
        }
        public static bool operator !=(Edge obj1, Edge obj2)
        {
            return !(obj1 == obj2);
        }
    }
}
