using System;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core.GraphOperations
{
    [Serializable]
    public class Vertex
    {
        public string Name { get; set; }
        public IEnumerable<Edge> ConnectedEdges { get { return this._ConnectedEdges.ToList().AsReadOnly(); } }
        internal ISet<Edge> _ConnectedEdges = new HashSet<Edge>();
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

        public int Degree { get { return this._ConnectedEdges.Count; } }
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


    }
}
