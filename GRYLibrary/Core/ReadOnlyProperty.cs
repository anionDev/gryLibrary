using System;
namespace GRYLibrary.Core
{
    public abstract class ReadOnlyProperty
    {
        public abstract void UpdateValue();
    }
    public class ReadOnlyProperty<T> : ReadOnlyProperty
    {
        private Property<T> _Property { get; set; }
        private readonly Func<Tuple<bool/*calculateValueWasSuccessful*/, T/*value*/>> _SetValueFunction;
        public DateTime LastUpdate() { return this._Property.LastWriteTime; }
        public ReadOnlyProperty(Func<Tuple<bool, T>> SetValueFunction)
        {
            _Property = new Property<T>(default, string.Empty, false);
            this._SetValueFunction = SetValueFunction;
        }
        public override void UpdateValue()
        {
            Tuple<bool, T> result = this._SetValueFunction();
            if (result.Item1)
            {
                _Property.Value = result.Item2;
            }
            else
            {
                _Property.UnsetValue();
            }
        }
        public T Value
        {
            get
            {
                return this._Property.Value;
            }
        }
    }
}
