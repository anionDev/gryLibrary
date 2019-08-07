using System;

namespace GRYLibrary.Miscellaneous.GraphOperations
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
            if (typedObject == null)
            {
                return false;
            }
            return this.Name.Equals(typedObject.Name) && this.Weight.Equals(typedObject.Weight) && (this.Source.Equals(typedObject.Source) && this.Target.Equals(typedObject.Target)|| this.Source.Equals(typedObject.Target) && this.Target.Equals(typedObject.Source));
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
