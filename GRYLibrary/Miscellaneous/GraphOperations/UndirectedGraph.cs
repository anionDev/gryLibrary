using System;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Miscellaneous.GraphOperations
{
    public class UndirectedGraph : Graph
    {
        public static UndirectedGraph CreateByAdjacencyMatrix(double[,] adjacencyMatrix)
        {
            UndirectedGraph result = new UndirectedGraph();
            Tuple<IList<Edge>, IList<Vertex>> items = result.ParseAdjacencyMatrix(adjacencyMatrix);
            foreach (Vertex item in items.Item2)
            {
                result._Vertices.Add(item);
            }
            foreach (Edge item in items.Item1)
            {
                result._Edges.Add(item);
            }
            return result;
        }
        public override void Accept(IGraphVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(IGraphVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }

        public bool IsConnected()
        {
            if (this._Vertices.Count == 0)
            {
                throw new Exception("No vertices available.");
            }
            Vertex startVertex = this._Vertices.First();
            Dictionary<Vertex, bool> visited = new Dictionary<Vertex, bool>();
            foreach (Vertex vertex in this._Vertices)
            {
                visited.Add(vertex, false);
            }
            List<Vertex> nextOnes = new List<Vertex>();
            nextOnes.Add(startVertex);
            visited[startVertex] = true;
            while (nextOnes.Count != 0)
            {
                List<Vertex> nextNextOnes = new List<Vertex>();
                foreach (Vertex nextOne in nextOnes)
                {
                    if (!visited[nextOne])
                    {
                        visited[nextOne] = true;
                        nextNextOnes.AddRange(this.GetDirectSuccessors(nextOne).Where(s => !visited[s]).ToList());
                    }
                }
                nextOnes = nextNextOnes;
            }
            return visited.ContainsValue(false);
        }
        /// <returns>Returns a set of all vertices which have a connection to this vertex in this graph.</returns>
        /// <remarks>
        /// If this graph contains a selfloop with this vertex then the result-set will also contain this vertex.
        /// The runtime of this function is &lt;=O(|this.Edges|).
        /// </remarks>
        public override ISet<Vertex> GetDirectSuccessors(Vertex vertex)
        {
            HashSet<Vertex> result = new HashSet<Vertex>();
            foreach (Edge edge in vertex.ConnectedEdges)
            {
                bool sourceIsThis = edge.Source.Equals(vertex);
                bool targetIsThis = edge.Target.Equals(vertex);
                if (sourceIsThis && targetIsThis)
                {
                    result.Add(vertex);
                }
                else
                {
                    if (sourceIsThis)
                    {
                        result.Add(edge.Target);
                    }
                    if (targetIsThis)
                    {
                        result.Add(edge.Source);
                    }
                }
            }
            return result;
        }
        public override bool TryGetConnectionBetween(Vertex vertex1, Vertex vertex2, out Edge connection)
        {
            foreach (Edge edge in this._Edges)
            {
                if ((edge.Source.Equals(vertex1) && edge.Target.Equals(vertex2)) || (edge.Target.Equals(vertex1) && edge.Source.Equals(vertex2)))
                {
                    connection = edge;
                    return true;
                }
            }
            connection = null;
            return false;
        }
    }
}
