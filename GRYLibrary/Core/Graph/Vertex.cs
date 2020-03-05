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
            return GetConnectedEdges().Count;
        }
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
            return new List<Edge>(ConnectedEdges);
        }
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
        public override string ToString()
        {
            return this.Name;
        }

        internal IEnumerable<Vertex> GetSuccessorVertices()
        {
            List<Vertex> result = new List<Vertex>();
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
