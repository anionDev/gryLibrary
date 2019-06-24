using System;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Miscellaneous.GraphOperations
{
    public class Vertex
    {
        public string Name { get; set; } = string.Empty;
        public IEnumerable<Edge> ConnectedEdges { get { return this._ConnectedEdges.ToList().AsReadOnly(); } }
        internal ISet<Edge> _ConnectedEdges = new HashSet<Edge>();
        public Vertex(string name = "")
        {
            this.Name = name;
        }
        public ISet<Vertex> GetDirectSuccessors(Graph graph)
        {
            return graph.Accept(new GetDirectSuccessorsVisitor(this));
        }
        private class GetDirectSuccessorsVisitor : IGraphVisitor<ISet<Vertex>>
        {
            private readonly Vertex _Vertex;

            public GetDirectSuccessorsVisitor(Vertex vertex)
            {
                this._Vertex = vertex;
            }

            public ISet<Vertex> Handle(UndirectedGraph graph)
            {
                this.CheckContains(graph);
                HashSet<Vertex> result = new HashSet<Vertex>();
                foreach (Edge edge in this._Vertex.ConnectedEdges)
                {
                    if (edge.Source.Equals(edge.Target))
                    {
                        result.Add(this._Vertex);
                        continue;
                    }
                    if (edge.Source.Equals(this._Vertex))
                    {
                        result.Add(edge.Target);
                    }
                    else
                    {
                        result.Add(edge.Source);
                    }
                }
                return result;
            }

            public ISet<Vertex> Handle(DirectedGraph graph)
            {
                this.CheckContains(graph);
                HashSet<Vertex> result = new HashSet<Vertex>();
                foreach (Edge edge in this._Vertex.ConnectedEdges)
                {
                    if (edge.Source.Equals(edge.Target))
                    {
                        result.Add(this._Vertex);
                        continue;
                    }
                    if (!edge.Target.Equals(this._Vertex))
                    {
                        result.Add(edge.Target);
                    }
                }
                return result;
            }
            private void CheckContains(Graph graph)
            {
                if (!graph.Vertices.Contains(this._Vertex))
                {
                    throw new Exception($"The given {nameof(Vertex)} does not belong to the given {nameof(Graph)}.");
                }
            }

        }
    }
}
