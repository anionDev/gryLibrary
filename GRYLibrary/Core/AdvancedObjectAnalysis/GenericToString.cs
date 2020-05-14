using GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper;
using GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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

        public string ToString(object @object)
        {
            return this.ToString(@object, new PropertyEqualsCalculator(), 0);
        }
        private string ToString(object @object, PropertyEqualsCalculator propertyEqualsCalculator, int currentIndentationLevel)
        {
            if (@object == null)
            {
                return this.GetIndentation(currentIndentationLevel) + "null";
            }
            Type type = @object.GetType();
            if (new PrimitiveComparer(PropertyEqualsCalculator.Configuration).IsApplicable(type))
            {
                return this.GetIndentation(currentIndentationLevel) + $"(Type: {@object.GetType().Name}, Value: \"{@object.ToString().Replace("\"", "\\\"")}\")";
            }
            if (visitedObjects.ContainsKey(@object))
            {
                return this.GetIndentation(currentIndentationLevel) + $"[Object {visitedObjects[@object]}]";
            }
            try
            {
                Guid id = Guid.NewGuid();
                visitedObjects.Add(@object, id);

                if (Utilities.ObjectIsEnumerable(@object))
                {
                    IList<object> objectAsEnumerable = Utilities.ObjectToEnumerable<object>(@object).ToList();
                    string result = this.GetIndentation(currentIndentationLevel) + "[" + Environment.NewLine;
                    int count = objectAsEnumerable.Count();
                    for (int i = 0; i < count; i++)
                    {
                        var current = objectAsEnumerable[i];
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
