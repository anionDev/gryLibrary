using System;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core.Graph
{
    public class Vertex
    {
        public string Name { get; set; }
        public Vertex() : this(CalculateVertexName())
        {
        }
        public Vertex(string name)
        {
            this.Name = name;
        }
        private static string CalculateVertexName()
        {
            return $"{nameof(Vertex)}_{Guid.NewGuid().ToString().Substring(0, 8)}";
        }

        public int Degree()
        {
            return this.GetConnectedEdges().Count;
        }
        /// <returns>Returns true if and only if the <see cref="Name"/> of the vertices are equal.</returns>
        public override bool Equals(object obj)
        {
            Vertex typedObject = obj as Vertex;
            if (typedObject == null)
            {
                return false;
            }
            return this.Name.Equals(typedObject.Name);
        }
        internal List<Edge> ConnectedEdges { get; } = new List<Edge>();
        public List<Edge> GetConnectedEdges()
        {
            return new List<Edge>(this.ConnectedEdges);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Name);
        }

        public override string ToString()
        {
            return this.Name;
        }

        internal IEnumerable<Vertex> GetSuccessorVertices()
        {
            List<Vertex> result = new();
            foreach (Edge edge in this.ConnectedEdges)
            {
                if (edge.GetInputs().Contains(this))
                {
                    result.AddRange(edge.GetOutputs());
                }
            }
            return result;
        }
    }
}
