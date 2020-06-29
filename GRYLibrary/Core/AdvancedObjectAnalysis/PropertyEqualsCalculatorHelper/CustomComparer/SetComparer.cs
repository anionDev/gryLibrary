﻿using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer
{
    public class SetComparer : AbstractCustomComparer
    {
        internal SetComparer(PropertyEqualsCalculatorConfiguration cacheAndConfiguration) : base(cacheAndConfiguration)
        {
            this.Configuration = cacheAndConfiguration;
        }

        internal override bool DefaultEquals(object item1, object item2)
        {
            bool result = this.EqualsTyped(Utilities.ObjectToSet<object>(item1), Utilities.ObjectToSet<object>(item2));
            return result;
        }
        internal bool EqualsTyped<T>(ISet<T> set1, ISet<T> set2)
        {
            if (set1.Count != set2.Count)
            {
                return false;
            }
            foreach (T obj in set1)
            {
                if (!this.Contains(set2, obj))
                {
                    return false;
                }
            }
            return true;
        }

        private bool Contains<T>(ISet<T> set, T obj)
        {
            foreach (T item in set)
            {
                if (this._PropertyEqualsCalculator.Equals(item, obj))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool IsApplicable(Type typeOfObject1, Type typeOfObject2)
        {
            return Utilities.TypeIsSet(typeOfObject1) && Utilities.TypeIsSet(typeOfObject2);
        }

        internal override int DefaultGetHashCode(object obj)
        {
            return this.Configuration.GetHashCode(obj);
        }
    }
}