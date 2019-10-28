using System;
using System.Collections.Generic;
using System.Linq;

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
        public IList<Vertex> Vertices { get; private set; }

        protected static Graph FillByAdjacencyMatrix(Graph grpah, double[,] adjacencyMatrix)
        {
            Tuple<IList<Edge>, IList<Vertex>> items = grpah.ParseAdjacencyMatrix(adjacencyMatrix);
            foreach (Vertex item in items.Item2)
            {
                grpah._Vertices.Add(item);
            }
            foreach (Edge item in items.Item1)
            {
                grpah._Edges.Add(item);
            }
            grpah.SortVertices();
            return grpah;
        }

        protected ISet<Vertex> _Vertices = new HashSet<Vertex>();
        public IEnumerable<Edge> Edges { get { return this._Edges.ToList().AsReadOnly(); } }
        protected ISet<Edge> _Edges = new HashSet<Edge>();

        public abstract void Accept(IGraphVisitor visitor);
        public abstract T Accept<T>(IGraphVisitor<T> visitor);
        public Graph()
        {
            this.SortVertices();
        }
        public abstract ISet<Vertex> GetDirectSuccessors(Vertex vertex);
        public bool SelfLoopIsAllowed
        {
            get
            {
                return this._SelfLoopIsAllowed;
            }
            set
            {
                if (!value && this.ContainsOneOrMoreSelfLoops())
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
            if (this.Edges.Where(existingEdge => existingEdge.Name.Equals(edge.Name)).Count() > 0)
            {
                throw new Exception($"This graph does already have an edge with the name {edge.Name}.");
            }
            if (this.TryGetConnectionBetween(edge.Source, edge.Target, out _))
            {
                throw new Exception($"This graph does already have an edge which connects {edge.Source.Name} and {edge.Target.Name}.");
            }
            this._Vertices.Add(edge.Source);
            this._Vertices.Add(edge.Target);
            this.SortVertices();
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
            if (this.Vertices.Contains(vertex))
            {
                throw new Exception($"This graph does already have a vertex with the name {vertex.Name}.");
            }
            this._Vertices.Add(vertex);
            this.SortVertices();
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
            this.SortVertices();
        }
        protected void SortVertices()
        {
            this.Vertices = this._Vertices.OrderBy(vertex => vertex.Name).ToList().AsReadOnly();
        }

        public bool ContainsOneOrMoreSelfLoops()
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
        public abstract bool ContainsOneOrMoreCycles();
        public bool IsConnected()
        {
            if (this._Vertices.Count == 0)
            {
                throw new InvalidOperationException("No vertices available.");
            }
            if (this._Vertices.Count == 1)
            {
                return true;
            }
            Vertex startVertex = this._Vertices.First();
            Dictionary<Vertex, bool> visited = new Dictionary<Vertex, bool>();
            foreach (Vertex vertex in this._Vertices)
            {
                visited.Add(vertex, false);
            }
            HashSet<Vertex> nextOnes = new HashSet<Vertex>();
            nextOnes.Add(startVertex);
            visited[startVertex] = true;
            while (nextOnes.Count != 0)
            {
                HashSet<Vertex> nextNextOnes = new HashSet<Vertex>();
                foreach (Vertex nextOne in nextOnes)
                {
                    if (!visited[nextOne])
                    {
                        visited[nextOne] = true;
                        nextNextOnes.UnionWith(this.GetDirectSuccessors(nextOne).Where(s => !visited[s]));
                    }
                }
                nextOnes = nextNextOnes;
            }
            return !visited.ContainsValue(false);
        }

        public double[,] ToAdjacencyMatrix()
        {
            IList<Vertex> vertices = this.Vertices.ToList();
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
                Vertex newVertex = new Vertex("Vertex_" + (i + 1).ToString());
                vertices.Add(newVertex);
            }
            IList<Edge> edges = new List<Edge>();
            for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < adjacencyMatrix.GetLength(1); j++)
                {
                    Edge newEdge = new Edge(vertices[i], vertices[j], "Edge_" + (i + 1).ToString() + "_" + (j + 1).ToString());
                    newEdge.Weight = adjacencyMatrix[i, j];
                    edges.Add(newEdge);
                }
            }
            return new Tuple<IList<Edge>, IList<Vertex>>(edges, vertices);
        }
        public ISet<Cycle> GetAllCycles()
        {
            ISet<Cycle> result = new HashSet<Cycle>();
            foreach (Vertex vertex in this.Vertices)
            {
                result.UnionWith(GetAllCyclesThroughASpecificVertex(vertex));
            }
            return result;
        }

        public ISet<Cycle> GetAllCyclesThroughASpecificVertex(Vertex vertex)
        {
            ISet<Cycle> result = new HashSet<Cycle>();
            throw new NotImplementedException();
            return result;
        }
        public void DepthFirstSearch(Action<IList<Edge>> customAction)
        {
            DepthFirstSearch(customAction, this.Vertices.First());
        }
        public void DepthFirstSearch(Action<IList<Edge>> customAction, Vertex startVertex)
        {
            if (!this.Vertices.Contains(startVertex))
            {
                throw new Exception($"Vertex '{startVertex}' is not contained in this graph.");
            }
            Dictionary<Vertex, bool> dictionary = new Dictionary<Vertex, bool>();
            foreach (Vertex vertex in this.Vertices)
            {
                dictionary.Add(vertex, false);
            }
            DepthFirstSearch(customAction, startVertex, dictionary, new List<Edge>());
        }
        private void DepthFirstSearch(Action<IList<Edge>> customAction, Vertex currentVertex, IDictionary<Vertex, bool> visitedMap, IList<Edge> currentPath)
        {
            if (!visitedMap[currentVertex])
            {
                visitedMap[currentVertex] = true;
                foreach (Edge edge in this.Edges)
                {
                    if (edge.Source.Equals(currentVertex) && !visitedMap[edge.Target])
                    {
                        List<Edge> path = new List<Edge>(currentPath);
                        path.Add(edge);
                        customAction(path);
                        DepthFirstSearch(customAction, edge.Target, visitedMap, path);
                    }
                }
            }
        }
        /// <returns>
        /// Returns true if and only if the adjacency-matrices of this and <paramref name="obj"/> are equal.
        /// </returns>
        /// <remarks>
        /// This function ignores properties like <see cref="Graph.SelfLoopIsAllowed"/> or the name of the edges and vertices.
        /// </remarks>
        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            Graph typedObj = obj as Graph;
            double[,] adjacencyMatrix1 = this.ToAdjacencyMatrix();
            double[,] adjacencyMatrix2 = typedObj.ToAdjacencyMatrix();
            bool arraysEquals = Utilities.TwoDimensionalArrayEquals(adjacencyMatrix1, adjacencyMatrix2);
            bool verticesEquals = this.Vertices.SequenceEqual(typedObj.Vertices);
            return arraysEquals && verticesEquals;
        }
        public override int GetHashCode()
        {
            return this.Vertices.Count().GetHashCode();
        }
        public bool IsSubgraph(Graph subgraph, out IDictionary<Vertex, Vertex> mappingFromSubgraphToThisGraph)
        {
            throw new NotImplementedException();
        }
        public bool IsSubgraphOf(Graph graph, out IDictionary<Vertex, Vertex> mappingFromgraphToThisGraph)
        {
            IDictionary<Vertex, Vertex> mappingFromSubgraphToThisGraph;
            bool result = graph.IsSubgraph(this, out mappingFromSubgraphToThisGraph);
            if (result)
            {
                mappingFromgraphToThisGraph = mappingFromSubgraphToThisGraph.ToDictionary(keyValuePair => keyValuePair.Value, keyValuePair => keyValuePair.Key);
            }
            else
            {
                mappingFromgraphToThisGraph = null;
            }
            return result;
        }
        public bool IsIsomorphic(Graph otherGraph, out IDictionary<Vertex, Vertex> bijectionFromOtherGraphToThisGraph)
        {
            if (!this.GetType().Equals(otherGraph.GetType()))
            {
                throw new Exception($"Graphs of types {this.GetType().FullName} and {otherGraph.GetType().FullName} are not comparable.");
            }
            if (this._Vertices.Count != otherGraph._Vertices.Count || this.Edges.Count() != otherGraph.Edges.Count())
            {
                bijectionFromOtherGraphToThisGraph = null;
                return false;
            }
            throw new NotImplementedException();
        }
        public bool HasHamiltonianCycle(out Cycle result)
        {
            //TODO implement an algorithm which is more performant
            foreach (Cycle cycle in this.GetAllCycles())
            {
                if (cycle.Edges.Count == this._Vertices.Count)
                {
                    result = cycle;
                    return true;
                }
            }
            result = null;
            return false;
        }
        public int GetMinimumDegree()
        {
            return this.Vertices.Min(vertex => vertex.Degree);
        }
        public int GetMaximumDegree()
        {
            return this.Vertices.Max(vertex => vertex.Degree);
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
