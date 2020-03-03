using System;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core.GraphOperations
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
        protected ISet<Vertex> _Vertices = new HashSet<Vertex>();
        public abstract ISet<Edge> Edges { get; }

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

        public Vertex GetVertexByName(string vertexName)
        {
            foreach (Vertex vertex in this.Vertices)
            {
                if (vertex.Name.Equals(vertexName))
                {
                    return vertex;
                }
            }
            throw new KeyNotFoundException();
        }
        /// <remarks>
        /// This function will also add the vertices which are connected by <paramref name="edge"/> to the list of vertices of this graph.
        /// </remarks>
        public abstract void AddEdge(Edge edge);
        public abstract void RemoveEdge(Edge edge);
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
            foreach (Edge edge in this.Edges)
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
            foreach (DirectedEdge edge in vertex.ConnectedEdges)
            {
                this.RemoveEdge(edge);
            }
            this.SortVertices();
        }
        protected void AddCheck(Edge edge, Vertex connectedVertex1, Vertex connectedVertex2)
        {
            if (!this.SelfLoopIsAllowed && connectedVertex1.Equals(connectedVertex2))
            {
                throw new Exception($"Self-loops are not allowed. Change the value of the {nameof(this.SelfLoopIsAllowed)}-property to allow this.");
            }
            if (this.Edges.Where(existingEdge => existingEdge.Name.Equals(edge.Name)).Count() > 0)
            {
                throw new Exception($"This graph does already have an edge with the name {edge.Name}.");
            }
            if (this.TryGetEdge(connectedVertex1, connectedVertex2, out _))
            {
                throw new Exception($"This graph does already have an edge which connects {connectedVertex1.Name} and {connectedVertex2.Name}.");
            }
        }
        protected void SortVertices()
        {
            this.Vertices = this._Vertices.OrderBy(vertex => vertex.Name).ToList().AsReadOnly();
        }

        public bool ContainsOneOrMoreSelfLoops()
        {
            foreach (DirectedEdge edge in this.Edges)
            {
                if (edge.Source.Equals(edge.Target))
                {
                    return true;
                }
            }
            return false;
        }
        public bool ContainsOneOrMoreCycles()
        {
            foreach (Vertex vertex in this.Vertices)
            {
                bool containsCycle = false;
                this.DepthFirstSearch((currentVertex, edges) =>
                {
                    if (currentVertex.Equals(vertex) && edges.Count > 0)
                    {
                        containsCycle = true;
                    }
                    return true;
                }, vertex);
                if (containsCycle)
                {
                    return true;
                }
            }
            return false;
        }
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
            this.DepthFirstSearch((v, l) => visited[v] = true);
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
                    if (this.TryGetEdge(vertices[i], vertices[j], out DirectedEdge connection))
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

        public ISet<Cycle> GetAllCycles()
        {
            ISet<Cycle> result = new HashSet<Cycle>();
            foreach (Vertex vertex in this.Vertices)
            {
                result.UnionWith(this.GetAllCyclesThroughASpecificVertex(vertex));
            }
            return result;
        }

        public ISet<Cycle> GetAllCyclesThroughASpecificVertex(Vertex vertex)
        {
            HashSet<Cycle> result = new HashSet<Cycle>();
            ISet<WriteableTuple<IList<DirectedEdge>, bool>> paths = new HashSet<WriteableTuple<IList<DirectedEdge>, bool>>();
            foreach (DirectedEdge edge in this.GetDirectSuccessorEdges(vertex))
            {
                paths.Add(new WriteableTuple<IList<DirectedEdge>, bool>(new List<DirectedEdge>(new DirectedEdge[] { edge }), false));
            }
            while (paths.Where(pathTuple => !pathTuple.Item2).Count() > 0)
            {
                foreach (WriteableTuple<IList<DirectedEdge>, bool> pathTuple in paths.ToList())
                {
                    if (!pathTuple.Item2)
                    {
                        IList<DirectedEdge> currentPath = pathTuple.Item1;
                        if (Cycle.RepresentsCycle(currentPath))
                        {
                            result.Add(new Cycle(currentPath));
                            pathTuple.Item2 = true;
                        }
                        else
                        {
                            foreach (DirectedEdge edge in this.GetDirectSuccessorEdges(currentPath.Last().Source).Concat(this.GetDirectSuccessorEdges(currentPath.Last().Target)))
                            {
                                if (!this.Contains(currentPath, edge.Source, true))
                                {
                                    List<DirectedEdge> newPath = new List<DirectedEdge>(currentPath);
                                    newPath.Add(edge);
                                    paths.Add(new WriteableTuple<IList<DirectedEdge>, bool>(newPath, false));
                                }
                                if (!this.Contains(currentPath, edge.Target, true))
                                {
                                    List<DirectedEdge> newPath = new List<DirectedEdge>(currentPath);
                                    newPath.Add(edge);
                                    paths.Add(new WriteableTuple<IList<DirectedEdge>, bool>(newPath, false));
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }
        public override string ToString()
        {
            double[,] matrix = this.ToAdjacencyMatrix();
            var table = TableGenerator.Generate(matrix, new TableGenerator.ASCIITable(), value => value.ToString());
            return string.Join(Environment.NewLine, table);
        }
        public abstract ISet<DirectedEdge> GetDirectSuccessorEdges(Vertex vertex);
        private bool Contains(IList<DirectedEdge> edges, Vertex vertex, bool excludeTargetOfLastEdge = false)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                DirectedEdge currentEdge = edges[i];
                if (currentEdge.Source.Equals(vertex))
                {
                    return true;
                }
                bool isLastEdge = i == edges.Count - 1;
                if (!(isLastEdge && excludeTargetOfLastEdge))
                {
                    if (currentEdge.Target.Equals(vertex))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void BreadthFirstSearch(Func<Vertex/*current vertex*/, IList<DirectedEdge>/*path*/, bool/*continue search*/> customAction)
        {
            this.BreadthFirstSearch(customAction, this.Vertices.First());
        }
        public void BreadthFirstSearch(Func<Vertex/*current vertex*/, IList<DirectedEdge>/*path*/, bool/*continue search*/> customAction, Vertex startVertex)
        {
            this.InitializeSearchAndDoSomeChecks(startVertex, out Dictionary<Vertex, bool> visitedMap);
            Queue<Tuple<Vertex, IList<DirectedEdge>>> queue = new Queue<Tuple<Vertex, IList<DirectedEdge>>>();
            visitedMap[startVertex] = true;
            List<DirectedEdge> initialList = new List<DirectedEdge>();
            if (!customAction(startVertex, initialList))
            {
                return;
            }
            queue.Enqueue(new Tuple<Vertex, IList<DirectedEdge>>(startVertex, initialList));
            while (queue.Count != 0)
            {
                Tuple<Vertex, IList<DirectedEdge>> currentVertex = queue.Dequeue();
                foreach (Vertex successor in this.GetDirectSuccessors(currentVertex.Item1))
                {
                    if (!visitedMap[successor])
                    {
                        visitedMap[successor] = true;
                        List<DirectedEdge> successorPath = currentVertex.Item2.ToList();
                        if (this.TryGetEdge(currentVertex.Item1, successor, out DirectedEdge edge))
                        {
                            successorPath.Add(edge);
                        }
                        else
                        {
                            throw new Exception($"Could not get edge with name '{edge.Name}'");
                        }
                        if (!customAction(successor, successorPath))
                        {
                            return;
                        }
                        queue.Enqueue(new Tuple<Vertex, IList<DirectedEdge>>(successor, successorPath));
                    }
                }
            }

        }

        public void DepthFirstSearch(Func<Vertex/*current vertex*/, IList<DirectedEdge>/*path*/, bool/*continue search*/> customAction)
        {
            this.DepthFirstSearch(customAction, this.Vertices.First());
        }
        public void DepthFirstSearch(Func<Vertex/*current vertex*/, IList<DirectedEdge>/*path*/, bool/*continue search*/> customAction, Vertex startVertex)
        {
            this.InitializeSearchAndDoSomeChecks(startVertex, out Dictionary<Vertex, bool> visitedMap);
            Stack<Tuple<Vertex, IList<DirectedEdge>>> stack = new Stack<Tuple<Vertex, IList<DirectedEdge>>>();
            stack.Push(new Tuple<Vertex, IList<DirectedEdge>>(startVertex, new List<DirectedEdge>()));
            while (stack.Count > 0)
            {
                Tuple<Vertex, IList<DirectedEdge>> currentVertex = stack.Pop();
                if (!visitedMap[currentVertex.Item1])
                {
                    visitedMap[currentVertex.Item1] = true;
                    if (!customAction(currentVertex.Item1, currentVertex.Item2))
                    {
                        return;
                    }
                    foreach (Vertex successor in this.GetDirectSuccessors(currentVertex.Item1))
                    {
                        List<DirectedEdge> successorPath = currentVertex.Item2.ToList();
                        if (this.TryGetEdge(currentVertex.Item1, successor, out DirectedEdge edge))
                        {
                            successorPath.Add(edge);
                        }
                        else
                        {
                            throw new Exception($"Could not get edge with name '{edge.Name}'");
                        }
                        stack.Push(new Tuple<Vertex, IList<DirectedEdge>>(successor, successorPath));
                    }
                }
            }
        }
        private void InitializeSearchAndDoSomeChecks(Vertex startVertex, out Dictionary<Vertex, bool> visitedMap)
        {
            if (!this.Vertices.Contains(startVertex))
            {
                throw new Exception($"Vertex '{startVertex}' is not contained in this graph.");
            }
            visitedMap = new Dictionary<Vertex, bool>();
            foreach (Vertex vertex in this.Vertices)
            {
                visitedMap.Add(vertex, false);
            }
        }

        public abstract bool TryGetEdge(Vertex source, Vertex target, out DirectedEdge edge);

        /// <returns>
        /// Returns true if and only if the adjacency-matrices of this and <paramref name="obj"/> are equal.
        /// </returns>
        /// <remarks>
        /// This function ignores properties like <see cref="SelfLoopIsAllowed"/> or the name of the edges and vertices.
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
