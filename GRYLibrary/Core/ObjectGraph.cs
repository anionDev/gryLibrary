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
        public IDictionary<Edge, PropertyInfo> EdgesInformation { get; }
        public IDictionary<object, Guid> ObjectsWithTheirId { get; }
        public Graph Graph { get; }
        public object @Object { get; }
        public object NullRepresenter { get; } = new object();
        public ObjectGraph(object @object)
        {
            IdWithTheirObjects = new Dictionary<Guid, object>();
            ObjectsWithTheirId = new Dictionary<object, Guid>(new ReferenceComparer());
            this.IdWithTheirVertices = new Dictionary<Guid, Vertex>();
            this.Object = HandleNull(@object);
        }

        private object HandleNull(object @object)
        {
            if (@object == null)
            {
                return NullRepresenter;
            }
            else
            {
                return @object;
            }
        }

        public void BuildObjectGraph()
        {
            Handle(@Object);
        }
        private Guid Handle(object @object)
        {
            @object = HandleNull(@object);
            Guid idOfCurrentObject;
            if (GetsSpecialTreatment(@object.GetType()))
            {

            }
            else
            {
                idOfCurrentObject = AddToDictionaryIfNotExist(@object);
                foreach (PropertyInfo property in Utilities.GetPropertiesWhichHaveGetterAndSetter(@object.GetType()))
                {
                    object propertyValue = property.GetValue(@object, null);
                    Guid idOfProperty;
                    if (ObjectsWithTheirId.ContainsKey(@object))
                    {
                        idOfProperty = ObjectsWithTheirId[@object];
                    }
                    else
                    {
                        idOfProperty = Handle(propertyValue);
                    }
                    Edge edge = new Edge(IdWithTheirVertices[idOfCurrentObject], IdWithTheirVertices[idOfProperty], property.Name);
                    EdgesInformation.Add(edge, property);
                    Graph.AddEdge(edge);
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
            if (ObjectsWithTheirId.ContainsKey(@object))
            {
                return ObjectsWithTheirId[@object];
            }
            else
            {
                Guid objectId = Guid.NewGuid();
                IdWithTheirObjects.Add(objectId, @object);
                ObjectsWithTheirId.Add(@object, objectId);
                Vertex vertex = new Vertex(objectId.ToString());
                Graph.AddVertex(vertex);
                IdWithTheirVertices.Add(objectId, vertex);
                return objectId;
            }
        }
        public override int GetHashCode()
        {
            return Object.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            ObjectGraph typedObject = obj as ObjectGraph;
            if (typedObject == null)
            {
                return false;
            }
            return EqualsAdvanced(IdWithTheirVertices[ObjectsWithTheirId[this.Object]], typedObject);
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
