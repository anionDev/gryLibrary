using GRYLibrary.Core.Graph.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core.Graph
{
    public class UndirectedGraph : Graph
    {
        public override ISet<Edge> Edges { get { return new HashSet<Edge>(_UndirectedEdges); } }
        public ISet<Edge> UndirectedEdges { get { return new HashSet<Edge>(_UndirectedEdges); } }
        private ISet<UndirectedEdge> _UndirectedEdges = new HashSet<UndirectedEdge>();
        public override void Accept(IGraphVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(IGraphVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
        /// <returns>Returns a set of all vertices which have a connection to this vertex in this graph.</returns>
        /// <remarks>
        /// If this graph contains a selfloop with this vertex then the result-set will also contain this vertex.
        /// The runtime of this function is &lt;=O(|this.Edges|).
        /// </remarks>
        public override ISet<Vertex> GetDirectSuccessors(Vertex vertex)
        {
            HashSet<Vertex> result = new HashSet<Vertex>();
            foreach (UndirectedEdge edge in vertex.GetConnectedEdges())
            {
                var vs = edge.ConnectedVertices.ToList();
                if (vs[0].Equals(vs[1]) && vs[0].Equals(vertex))
                {
                    result.Add(vertex);
                }
                else
                {
                    if (vs[0].Equals(vertex))
                    {
                        result.Add(vs[1]);
                    }
                    if (vs[1].Equals(vertex))
                    {
                        result.Add(vs[0]);
                    }
                }
            }
            return result;
        }
        public override bool TryGetEdge(Vertex vertex1, Vertex vertex2, out Edge result)
        {
            foreach (UndirectedEdge edge in this.Edges)
            {
                if (edge.Connects(vertex1, vertex2))
                {
                    result = edge;
                    return true;
                }
            }
            result = null;
            return false;
        }

        public override ISet<Edge> GetDirectSuccessorEdges(Vertex vertex)
        {
            HashSet<Edge> result = new HashSet<Edge>();
            foreach (Edge edge in vertex.GetConnectedEdges())
            {
                UndirectedEdge undirectedEdge = (UndirectedEdge)edge;
                IList<Vertex> connectedVertices = undirectedEdge.ConnectedVertices;
                bool sourceIsThis = connectedVertices[0].Equals(vertex);
                bool targetIsThis = connectedVertices[1].Equals(vertex);
                if (sourceIsThis && targetIsThis)
                {
                    result.Add(edge);
                }
                else
                {
                    if (sourceIsThis)
                    {
                        result.Add(edge);
                    }
                    if (targetIsThis)
                    {
                        result.Add(edge);
                    }
                }
            }
            return result;
        }

        public override void AddEdge(Edge edge)
        {
            if (!(edge is UndirectedEdge))
            {
                throw new InvalidEdgeTypeException($"{nameof(UndirectedGraph)}-objects can only have edges of type {nameof(UndirectedEdge)}");
            }
            UndirectedEdge undirectedEdge = (UndirectedEdge)edge;
            List<Vertex> connectedVertices = undirectedEdge.ConnectedVertices.ToList();
            this.AddCheck(edge, connectedVertices[0], connectedVertices[1]);
            this._UndirectedEdges.Add((UndirectedEdge)edge);
            OnEdgeAdded(edge);
        }

        public override void RemoveEdge(Edge edge)
        {
            if (edge is UndirectedEdge)
            {
                this._UndirectedEdges.Remove((UndirectedEdge)edge);
                OnEdgeRemoved(edge);
            }
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
