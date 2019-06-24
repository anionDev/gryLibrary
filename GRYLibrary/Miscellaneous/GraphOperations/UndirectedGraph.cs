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
                        nextNextOnes.AddRange(nextOne.GetDirectSuccessors(this).Where(s => !visited[s]).ToList());
                    }
                }
                nextOnes = nextNextOnes;
            }
            return visited.ContainsValue(false);
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
