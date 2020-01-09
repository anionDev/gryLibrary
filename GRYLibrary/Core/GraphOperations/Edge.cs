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

        public static bool operator <(Edge obj1, Edge obj2)
        {
            return string.Compare(obj1.Name, obj2.Name) < 0;
        }
        public static bool operator >(Edge obj1, Edge obj2)
        {
            return !(obj1 == obj2) & !(obj1 < obj2);
        }
        public static bool operator ==(Edge obj1, Edge obj2)
        {
            return obj1.Name.Equals(obj2.Name);
        }
        public static bool operator !=(Edge obj1, Edge obj2)
        {
            return !(obj1 == obj2);
        }
    }
}
