using System;

namespace GRYLibrary.Core.GraphOperations
{
    public class DirectedEdge : Edge
    {
        /// <remarks>
        /// In the context of <see cref="UndirectedGraph"/>-objects it does not matter which <see cref="Vertex"/>- object is marked as <see cref="Source"/> and which is marked as <see cref="Target"/>.
        /// </remarks>
        public Vertex Source { get; internal set; }
        /// <remarks>
        /// In the context of <see cref="UndirectedGraph"/>-objects it does not matter which <see cref="Vertex"/>- object is marked as <see cref="Source"/> and which is marked as <see cref="Target"/>.
        /// </remarks>
        public Vertex Target { get; internal set; }
        public DirectedEdge(Vertex source, Vertex target, string name, double weight = 1) : base(name, weight)
        {
            this.Source = source;
            this.Target = target;
        }

        internal UndirectedEdge ToUndirectedEdge()
        {
            return new UndirectedEdge(new Vertex[] { Source, Target }, this.Name, this.Weight);
        }
    }
}
