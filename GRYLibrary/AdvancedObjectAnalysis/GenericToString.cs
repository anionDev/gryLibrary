using GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper;
using GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer;
using GRYLibrary.Core.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GRYLibrary.Core.AdvancedObjectAnalysis
{
    public class GenericToString
    {
        public PropertyEqualsCalculator PropertyEqualsCalculator { get; set; } = new PropertyEqualsCalculator();
        public Func<PropertyInfo, bool> PropertySelector { get; set; } = (PropertyInfo propertyInfo) =>
        {
            return propertyInfo.CanWrite && propertyInfo.GetMethod.IsPublic;
        };
        public Func<FieldInfo, bool> FieldSelector { get; set; } = (FieldInfo propertyInfo) =>
        {
            return false;
        };
        public static GenericToString Instance { get; } = new GenericToString();
        private GenericToString() { }
        /// <summary>
        /// Represents a generic ToString-function which can handle cyclic references.
        /// </summary>
        /// <param name="object">The object which should be converted to a string</param>
        /// <param name="maxOutputLength">Maximal length of the output</param>
        /// <returns></returns>
        public string ToString(object @object, int maxOutputLength = int.MaxValue)
        {
            int minimalOutputLength = 4;
            if (maxOutputLength < minimalOutputLength)
            {
                throw new Exception($"The value of '{nameof(maxOutputLength)}' is {maxOutputLength} but must be {minimalOutputLength} or greater.");
            }
            string result = this.ToString(@object, new Dictionary<object, int>(new ReferenceEqualsComparer()), 0);
            if (result.Length > maxOutputLength)
            {
                result = result.Substring(0, maxOutputLength - 3) + "...";
            }
            return result;
        }
        private string ToString(object @object, IDictionary<object, int> visitedObjects, int currentIndentationLevel)
        {
            if (@object == null)
            {
                return this.GetIndentation(currentIndentationLevel) + "null";
            }
            Type type = @object.GetType();
            if (PrimitiveComparer.TypeIsTreatedAsPrimitive(type))
            {
                return this.GetIndentation(currentIndentationLevel) + $"(Type: {@object.GetType().Name}, Value: \"{@object.ToString().Replace("\"", "\\\"")}\")";
            }
            if (visitedObjects.ContainsKey(@object))
            {
                return this.GetIndentation(currentIndentationLevel) + $"[Object {visitedObjects[@object]}]";
            }
            try
            {
                int id = this.PropertyEqualsCalculator.GetHashCode(@object);
                visitedObjects.Add(@object, id);

                if (EnumerableTools.ObjectIsEnumerable(@object))
                {
                    IList<object> objectAsEnumerable = EnumerableTools.ObjectToEnumerable<object>(@object).ToList();
                    string result = this.GetIndentation(currentIndentationLevel) + "[" + Environment.NewLine;
                    int count = objectAsEnumerable.Count;
                    for (int i = 0; i < count; i++)
                    {
                        object current = objectAsEnumerable[i];
                        result += this.ToString(current, visitedObjects, currentIndentationLevel + 1);
                        if (i < count - 1)
                        {
                            result = result + "," + Environment.NewLine;
                        }
                    }
                    return result + Environment.NewLine + this.GetIndentation(currentIndentationLevel) + "]";
                }
                else
                {
                    List<(string/*Propertyname*/, object)> propertyValues = new List<(string, object)>();
                    foreach (FieldInfo field in type.GetFields())
                    {
                        if (this.FieldSelector(field))
                        {
                            propertyValues.Add((field.Name, field.GetValue(@object)));
                        }
                    }
                    foreach (PropertyInfo property in type.GetProperties())
                    {
                        if (this.PropertySelector(property))
                        {
                            propertyValues.Add((property.Name, property.GetValue(@object)));
                        }
                    }
                    string result = this.GetIndentation(currentIndentationLevel) + $"{{ (ObjectId: {id}, Type: {type.FullName}) ";
                    foreach ((string, object) entry in propertyValues)
                    {
                        result = result + Environment.NewLine + this.GetIndentation(currentIndentationLevel + 1) + entry.Item1 + ": " + Environment.NewLine + this.ToString(entry.Item2, visitedObjects, currentIndentationLevel + 1);
                    }
                    return result + Environment.NewLine + this.GetIndentation(currentIndentationLevel) + "}";
                }
            }
            catch
            {
                return $"[Error while executing {nameof(ToString)} for object of type {type.FullName}]";
            }
        }

        private string GetIndentation(int currentIndentationLevel)
        {
            return string.Empty.PadRight(currentIndentationLevel * 2);
        }
    }
}
