using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyIteratorHelper
{
    public class PropertyIteratorConfiguration
    {
        public Func<PropertyInfo, bool> PropertySelector { get; set; } = (PropertyInfo propertyInfo) =>
        {
            try
            {
                return propertyInfo.GetMethod.IsPublic && propertyInfo.SetMethod.IsPublic && !propertyInfo.GetMethod.IsStatic;
            }
            catch
            {
                return false;
            }
        };
        public Func<FieldInfo, bool> FieldSelector { get; set; } = (FieldInfo fieldInfo) =>
        {
            return false;
        };
    }
}
