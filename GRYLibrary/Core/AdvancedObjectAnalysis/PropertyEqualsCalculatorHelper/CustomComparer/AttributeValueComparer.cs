﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer
{
    public class AttributeValueComparer : AbstractCustomComparer
    {
        internal AttributeValueComparer(PropertyEqualsCalculatorConfiguration cacheAndConfiguration) : base(cacheAndConfiguration)
        {
        }

        public override bool IsApplicable(Type typeOfObject1, Type typeOfObject2)
        {
            return typeOfObject1.Equals(typeOfObject2);
        }

        internal override bool DefaultEquals(object object1, object object2)
        {
            Type type = object1.GetType();
            List<WriteableTuple<object, object>> attributeValues = new List<WriteableTuple<object, object>>();
            foreach (FieldInfo field in type.GetFields())
            {
                if (this.Configuration.FieldSelector(field))
                {
                    attributeValues.Add(new WriteableTuple<object, object>(field.GetValue(object1), field.GetValue(object2)));
                }
            }
            foreach (PropertyInfo property in type.GetProperties())
            {
                if (this.Configuration.PropertySelector(property))
                {
                    attributeValues.Add(new WriteableTuple<object, object>(property.GetValue(object1), property.GetValue(object2)));
                }
            }
            foreach (WriteableTuple<object, object> entry in attributeValues)
            {
                if (this._PropertyEqualsCalculator.Equals(entry.Item1, entry.Item2))
                {
                    this.Configuration.AddEqualObjectsToEquivalenceClasses(object1, object2);
                }
            }
            return true;
        }

        internal override int DefaultGetHashCode(object @object)
        {
            return this.Configuration.GetHashCode(@object);
        }
    }
}