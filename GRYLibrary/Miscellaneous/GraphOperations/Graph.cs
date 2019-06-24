using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Miscellaneous.GraphOperations
{
    /// <summary>
    /// Represents a graph
    /// </summary>
    /// <remarks>
    /// This graph does not support two edges between the same two vertices.
    /// </remarks>
    public abstract class Graph
    {
        public IEnumerable<Vertex> Vertices { get { return this._Vertices.ToList().AsReadOnly(); } }
        protected ISet<Vertex> _Vertices = new HashSet<Vertex>();
        public IEnumerable<Edge> Edges { get { return this._Edges.ToList().AsReadOnly(); } }
        protected ISet<Edge> _Edges = new HashSet<Edge>();

        public abstract void Accept(IGraphVisitor visitor);
        public abstract T Accept<T>(IGraphVisitor<T> visitor);
        public bool SelfLoopIsAllowed
        {
            get
            {
                return this._SelfLoopIsAllowed;
            }
            set
            {
                if (!value && this.ContainsSelfLoops())
                {
                    throw new Exception("Setting ContainsSelfLoops to false is not possible while the graph contains self-loops");
                }
                this._SelfLoopIsAllowed = value;
            }
        }
        private bool _SelfLoopIsAllowed = true;
        /// <remarks>
        /// This function will also add the vertices which are connected by <paramref name="edge"/> to the list of vertices of this graph.
        /// </remarks>
        public void AddEdge(Edge edge)
        {
            if (!this.SelfLoopIsAllowed && edge.Source.Equals(edge.Target))
            {
                throw new Exception($"Self-loops are not allowed. Change the value of the {nameof(this.SelfLoopIsAllowed)}-property to allow this.");
            }
            if (this.TryGetConnectionBetween(edge.Source, edge.Target, out _))
            {
                throw new Exception("This graph does already have a connection which connects this vertices.");
            }
            this._Vertices.Add(edge.Source);
            this._Vertices.Add(edge.Target);
            this._Edges.Add(edge);
            edge.Source._ConnectedEdges.Add(edge);
            edge.Target._ConnectedEdges.Add(edge);
        }
        public void RemoveEdge(Edge edge)
        {
            this._Edges.Remove(edge);
            edge.Source._ConnectedEdges.Remove(edge);
            edge.Target._ConnectedEdges.Remove(edge);
        }
        public void AddVertex(Vertex vertex)
        {
            this._Vertices.Add(vertex);
        }
        public void RemoveAllEdgesWithoutWeight()
        {
            foreach (Edge edge in this._Edges)
            {
                if (edge.Weight == 0)
                {
                    this.RemoveEdge(edge);
                }
            }
        }
        /// <remarks>
        /// This function will also remove the edges which have a connection to this <paramref name="vertex"/> from the list of edges of this graph.
        /// </remarks>
        public void RemoveVertex(Vertex vertex)
        {
            this._Vertices.Remove(vertex);
            foreach (Edge edge in vertex.ConnectedEdges)
            {
                this.RemoveEdge(edge);
            }
        }
        public bool ContainsSelfLoops()
        {
            foreach (Edge edge in this.Edges)
            {
                if (edge.Source.Equals(edge.Target))
                {
                    return true;
                }
            }
            return false;
        }
        public double[,] ToAdjacencyMatrix()
        {
            IList<Vertex> vertices = this._Vertices.ToList();
            int verticesCount = vertices.Count;
            double[,] result = new double[verticesCount, verticesCount];
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    if (this.TryGetConnectionBetween(vertices[i], vertices[j], out Edge connection))
                    {
                        result[i, j] = connection.Weight;
                    }
                    else
                    {
                        result[i, j] = 0;
                    }
                }
            }
            return result;
        }

        public abstract bool TryGetConnectionBetween(Vertex vertex1, Vertex vertex2, out Edge connection);

        protected Tuple<IList<Edge>, IList<Vertex>> ParseAdjacencyMatrix(double[,] adjacencyMatrix)
        {
            if (adjacencyMatrix.GetLength(0) != adjacencyMatrix.GetLength(1))
            {
                throw new Exception("The row-count must be equal to the column-count.");
            }
            IList<Vertex> vertices = new List<Vertex>();
            for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
            {
                Vertex newVertex = new Vertex("Vertex" + (i + 1).ToString());
                vertices.Add(newVertex);
            }
            IList<Edge> edges = new List<Edge>();
            for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < adjacencyMatrix.GetLength(1); j++)
                {
                    Edge newEdge = new Edge(vertices[i], vertices[j]);
                    newEdge.Weight = adjacencyMatrix[i, j];
                    edges.Add(newEdge);
                }
            }
            return new Tuple<IList<Edge>, IList<Vertex>>(edges, vertices);
        }
        public ISet<Cycle> GetCycles()
        {
            throw new NotImplementedException();
        }
    }
    public interface IGraphVisitor
    {
        void Handle(UndirectedGraph graph);
        void Handle(DirectedGraph graph);
    }
    public interface IGraphVisitor<T>
    {
        T Handle(UndirectedGraph graph);
        T Handle(DirectedGraph graph);
    }
}
