using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRYLibrary.Core.GraphOperations
{
    public class UndirectedEdge : Edge
    {
        public IEnumerable<Vertex> ConnectedVertices { get; internal set; }
        public UndirectedEdge(IEnumerable<Vertex> connectedVertices, string name, double weight = 1) : base(name, weight)
        {
            if (connectedVertices.Count() != 2)
            {
                throw new Exception($"An {nameof(UndirectedEdge)}-object must have exactly 2 connected vertices");
            }
            this.ConnectedVertices = connectedVertices.ToList().AsReadOnly();
        }

        internal DirectedEdge ToDirectedEdge()
        {
            var connectedVertices = ConnectedVertices.ToList();
            return new DirectedEdge(connectedVertices[0], connectedVertices[1], this.Name, this.Weight);
        }
    }
}
