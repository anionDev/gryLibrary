using GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer;
using GRYLibrary.Core.AdvancedObjectAnalysis.PropertyIteratorHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GRYLibrary.Core.AdvancedObjectAnalysis
{
    public class PropertyIterator
    {
        public PropertyIteratorConfiguration Configuration { get; set; }
        public PropertyIterator() : this(new PropertyIteratorConfiguration())
        {
        }
        public PropertyIterator(PropertyIteratorConfiguration configuration)
        {
            this.Configuration = configuration;
        }
        public IEnumerable<(object, Type)> IterateOverObjectTransitively(object @object)
        {
            List<(object, Type)> result = new List<(object, Type)>();
            IterateOverObjectTransitively(@object, result);
            return result;
        }
        private void IterateOverObjectTransitively(object @object, IList<(object, Type)> visitedObjects)
        {
            if (Contains(visitedObjects, @object))
            {
                return;
            }
            bool objectIsNull = @object == null;
            if (objectIsNull)
            {
                visitedObjects.Add((@object, typeof(object)));
                return;
            }
            Type type = @object.GetType();
            visitedObjects.Add((@object, type));
            if (Utilities.TypeIsEnumerable(type))
            {
                foreach (object item in Utilities.ObjectToEnumerable(@object))
                {
                    IterateOverObjectTransitively(item, visitedObjects);
                }
            }
            else if (PrimitiveComparer.TypeIsTreatedAsPrimitive(type))
            {
                // TODO
            }
            else
            {

                foreach (FieldInfo field in type.GetFields().Where((field) => this.Configuration.FieldSelector(field)))
                {
                    IterateOverObjectTransitively(field.GetValue(@object), visitedObjects);
                }
                foreach (PropertyInfo property in type.GetProperties().Where((property) => this.Configuration.PropertySelector(property)))
                {
                    IterateOverObjectTransitively(property.GetValue(@object), visitedObjects);
                }
            }
        }

        private bool Contains(IList<(object, Type)> visitedObjects, object @object)
        {
            foreach ((object, Type) currentItem in visitedObjects)
            {
                if (Utilities.ImprovedReferenceEquals(currentItem.Item1, @object))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
