using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.Graph
{
    public class DirectedEdge : Edge
    {
        public Vertex Source { get; internal set; }
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
        public override bool Connects(Vertex fromVertex, Vertex toVertex)
        {
            return this.Source.Equals(fromVertex) && this.Target.Equals(toVertex);
        }
        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
            {
                return false;
            }
            DirectedEdge typedObject = (DirectedEdge)obj;
            if (!this.Source.Equals(typedObject.Source))
            {
                return false;
            }
            if (!this.Target.Equals(typedObject.Target))
            {
                return false;
            }
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override IEnumerable<Vertex> GetConnectedVertices()
        {
            return new Vertex[] { this.Source, this.Target };
        }
    }
}
