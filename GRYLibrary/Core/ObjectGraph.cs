using GRYLibrary.Core.GraphOperations;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GRYLibrary.Core
{
    public class ObjectGraph
    {
        public IDictionary<Guid, Vertex> IdWithTheirVertices { get; }
        public IDictionary<Guid, object> IdWithTheirObjects { get; }
        public IDictionary<DirectedEdge, PropertyInfo> EdgesInformation { get; }
        public IDictionary<object, Guid> ObjectsWithTheirId { get; }
        public Graph Graph { get; }
        public object @Object { get; }
        public object NullRepresenter { get; } = new object();
        public ObjectGraph(object @object)
        {
            this.IdWithTheirObjects = new Dictionary<Guid, object>();
            this.ObjectsWithTheirId = new Dictionary<object, Guid>(new ReferenceComparer());
            this.IdWithTheirVertices = new Dictionary<Guid, Vertex>();
            this.Object = this.HandleNull(@object);
        }

        private object HandleNull(object @object)
        {
            if (@object == null)
            {
                return this.NullRepresenter;
            }
            else
            {
                return @object;
            }
        }

        public void BuildObjectGraph()
        {
            this.Handle(this.@Object);
        }
        private Guid Handle(object @object)
        {
            @object = this.HandleNull(@object);
            Guid idOfCurrentObject;
            if (this.GetsSpecialTreatment(@object.GetType()))
            {

            }
            else
            {
                idOfCurrentObject = this.AddToDictionaryIfNotExist(@object);
                foreach (PropertyInfo property in Utilities.GetPropertiesWhichHaveGetterAndSetter(@object.GetType()))
                {
                    object propertyValue = property.GetValue(@object, null);
                    Guid idOfProperty;
                    if (this.ObjectsWithTheirId.ContainsKey(@object))
                    {
                        idOfProperty = this.ObjectsWithTheirId[@object];
                    }
                    else
                    {
                        idOfProperty = this.Handle(propertyValue);
                    }
                    DirectedEdge edge = new DirectedEdge(this.IdWithTheirVertices[idOfCurrentObject], this.IdWithTheirVertices[idOfProperty], property.Name);
                    this.EdgesInformation.Add(edge, property);
                    this.Graph.AddEdge(edge);
                }
            }
            return idOfCurrentObject;
        }

        private bool GetsSpecialTreatment(Type propertyType)
        {
            throw new NotImplementedException();
        }

        private Guid AddToDictionaryIfNotExist(object @object)
        {
            if (this.ObjectsWithTheirId.ContainsKey(@object))
            {
                return this.ObjectsWithTheirId[@object];
            }
            else
            {
                Guid objectId = Guid.NewGuid();
                this.IdWithTheirObjects.Add(objectId, @object);
                this.ObjectsWithTheirId.Add(@object, objectId);
                Vertex vertex = new Vertex(objectId.ToString());
                this.Graph.AddVertex(vertex);
                this.IdWithTheirVertices.Add(objectId, vertex);
                return objectId;
            }
        }
        public override int GetHashCode()
        {
            return this.Object.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            ObjectGraph typedObject = obj as ObjectGraph;
            if (typedObject == null)
            {
                return false;
            }
            return this.EqualsAdvanced(this.IdWithTheirVertices[this.ObjectsWithTheirId[this.Object]], typedObject);
        }

        private bool EqualsAdvanced(Vertex startVertex, ObjectGraph otherGraph)
        {
            throw new NotImplementedException();
        }

        private class ReferenceComparer : IEqualityComparer<object>
        {
            public new bool Equals(object x, object y)
            {
                return object.ReferenceEquals(x, y);
            }

            public int GetHashCode(object obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
