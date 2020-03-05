using System;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core.Graph
{
    public class Cycle
    {
        public IList<Edge> Edges { get; private set; } = new List<Edge>();
        public Cycle(IList<Edge> edgesList)
        {
            List<Edge> edges = edgesList.ToList();
            if (edges.Count == 0)
            {
                throw new Exception("A cycle can not be empty.");
            }
            if (new HashSet<Edge>(edges).Count < edges.Count)
            {
                throw new Exception("A cycle can not contain two equal edges.");
            }
            if (!RepresentsCycle(edges))
            {
                throw new Exception("The given edge-list is not cyclic.");
            }
            this.Edges = edges;
        }
        public override bool Equals(object obj)
        {
            Cycle cycle = obj as Cycle;
            if (cycle == null)
            {
                return false;
            }
            int edgesCount = this.Edges.Count;
            if (this.Edges.Count != cycle.Edges.Count)
            {
                return false;
            }
            int indexOfStartItemInCycle2;
            if (cycle.Edges.Contains(this.Edges[0]))
            {
                indexOfStartItemInCycle2 = cycle.Edges.IndexOf(this.Edges[0]);
            }
            else
            {
                return false;
            }
            for (int indexOfStartItemInCycle1 = 0; indexOfStartItemInCycle1 < edgesCount; indexOfStartItemInCycle1++)
            {
                Edge edgeInCycle1 = this.Edges[indexOfStartItemInCycle1];
                Edge edgeInCycle2 = cycle.Edges[(indexOfStartItemInCycle2 + indexOfStartItemInCycle1) % edgesCount];
                if (!edgeInCycle1.Equals(edgeInCycle2))
                {
                    return false;
                }
            }
            return true;
        }
        public override int GetHashCode()
        {
            return this.Edges.Count.GetHashCode();
        }

        public static bool RepresentsCycle(IList<Edge> edges)
        {
            if (edges.Count > 0)
            {
                if (new HashSet<Edge>(edges).Count != edges.Count)
                {
                    return false;
                }
                return EdgesAreCyclic(edges);
            }
            else
            {
                return false;
            }
        }

        private static bool EdgesAreCyclic(IList<Edge> edges)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                Edge edge1 = edges[1];
                Edge edge2;
                if (i == edges.Count - 1)
                {
                    edge2 = edges[0];
                }
                else
                {
                    edge2 = edges[i + 1];
                }
                if (!AtLeastOneOutputOfEdge1LeadsToInputOfEdge1(edge1, edge2))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool AtLeastOneOutputOfEdge1LeadsToInputOfEdge1(Edge edge1, Edge edge2)
        {
            return edge1.GetOutputs().Intersect(edge2.GetInputs()).Count() > 0;
        }

        private IEnumerable<Vertex> GetIntersectionOfConnectedVertices(Edge edge1, Edge edge2)
        {
            return edge1.GetConnectedVertices().Intersect(edge2.GetConnectedVertices());
        }
        private class GetOtherConnectedVerticesVisitor : IEdgeVisitor<IEnumerable<Vertex>>
        {
            private readonly Vertex _Vertex;
            public GetOtherConnectedVerticesVisitor(Vertex vertex)
            {
                this._Vertex = vertex;
            }
            public IEnumerable<Vertex> Handle(UndirectedEdge edge)
            {
                if (edge.ConnectedVertices.Contains(this._Vertex))
                {
                    List<Vertex> result = edge.ConnectedVertices.ToList();
                    result.RemoveItemOnlyOnce(this._Vertex);
                    return result;
                }
                else
                {
                    throw new Exception();
                }
            }

            public IEnumerable<Vertex> Handle(DirectedEdge edge)
            {
                if (edge.Source.Equals(this._Vertex))
                {
                    return new Vertex[] { edge.Target };
                }
                if (edge.Target.Equals(this._Vertex))
                {
                    return new Vertex[] { edge.Source };
                }
                throw new Exception();
            }
        }
        public override string ToString()
        {
            return nameof(Cycle) + "(" + string.Join("->", this.Edges) + ")";
        }
    }
}
