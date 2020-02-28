using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.GraphOperations
{
    [Serializable]
    public class Vertex
    {
        public string Name { get; set; }
        internal ISet<Edge> ConnectedEdges { get; set; } = new HashSet<Edge>();
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

        public int Degree { get { return this.ConnectedEdges.Count; } }
        public override bool Equals(object obj)
        {
            Vertex typedObject = obj as Vertex;
            if (typedObject == null)
            {
                return false;
            }
            return this.Name.Equals(typedObject.Name);
        }
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
        public override string ToString()
        {
            return this.Name;
        }
        public ISet<Edge> GetConnectedEdges()
        {
            return new HashSet<Edge>(this.ConnectedEdges);
        }

    }
}
