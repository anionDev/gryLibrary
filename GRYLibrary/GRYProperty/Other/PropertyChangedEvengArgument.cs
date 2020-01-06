using System;

namespace GRYLibrary.GRYProperty.Other
{
    public class PropertyChangedEvengArgument<T>
    {
        internal PropertyChangedEvengArgument(T oldValue, T newValue, DateTime changeMoment)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
            this.ChangeMoment = changeMoment;
        }
        public T OldValue { get; }
        public T NewValue { get; }
        public DateTime ChangeMoment { get; }
    }

}
