namespace GRYLibrary.Miscellaneous.GraphOperations
{
    public class Edge
    {
        public string Name { get; set; }
        public double Weight { get; set; }
        /// <remarks>
        /// In the context of <see cref="UndirectedGraph"/>-objects it does not matter which <see cref="Vertex"/>- object is marked as <see cref="Source"/> and which is marked as <see cref="Target"/>.
        /// </remarks>
        public Vertex Source { get; internal set; }
        /// <remarks>
        /// In the context of <see cref="UndirectedGraph"/>-objects it does not matter which <see cref="Vertex"/>- object is marked as <see cref="Source"/> and which is marked as <see cref="Target"/>.
        /// </remarks>
        public Vertex Target { get; internal set; }
        public Edge(Vertex source, Vertex target, string name = "", double weight = 1)
        {
            this.Source = source;
            this.Target = target;
            this.Name = name;
            this.Weight = weight;
        }
    }
}

