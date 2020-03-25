using System;
namespace GRYLibrary.Core
{
    /// <summary>
    /// Represents a property whose value can not be set directly but updated by an update-function. You can always get the value from the last run of the update-function.
    /// </summary>
    public abstract class ReadOnlyProperty
    {
        public abstract void UpdateValue();
        public abstract object GetValue();
    }
    public class ReadOnlyProperty<T> : ReadOnlyProperty
    {
        private Property<T> _Property { get; set; }
        private readonly Func<Tuple<bool/*calculateValueWasSuccessful*/, T/*value*/>> _SetValueFunction;
        public DateTime LastUpdate() { return this._Property.LastWriteTime; }
        public ReadOnlyProperty(Func<Tuple<bool, T>> SetValueFunction)
        {
            this._Property = new Property<T>(default, string.Empty, false);
            this._SetValueFunction = SetValueFunction;
        }
        public override void UpdateValue()
        {
            try
            {
                Tuple<bool, T> result = this._SetValueFunction();
                if (result.Item1)
                {
                    this._Property.Value = result.Item2;
                }
                else
                {
                    this._Property.UnsetValue();
                }
            }
            catch
            {
                this._Property.UnsetValue();
            }
        }

        public override object GetValue()
        {
            return this.Value;
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
