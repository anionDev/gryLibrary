using System;

namespace GRYLibrary.Core
{
    /// <summary>
    /// Represents a threadsafe semaphore
    /// </summary>
    public sealed class Semaphore : Property<long>
    {
        public string Name { get; set; }
        public Semaphore(string propertyName = "") : base(0, propertyName, false)
        {
            this.LockEnabled = true;
        }
        public override long Value { get => base.Value; set => throw new InvalidOperationException($"Please use the {nameof(Semaphore.Increment)}- and {nameof(Semaphore.Decrement)}-operation to modify the value."); }
        public void Increment()
        {
            base.Value += 1;
        }
        public void Decrement()
        {
            if (this.Value == 0)
            {
                throw new InvalidOperationException($"The value of the {nameof(Semaphore)} can not be decremented if the {nameof(this.Value)} is 0.");
            }
            base.Value -= 1;
        }
        public override bool Equals(object @object)
        {
            if (!(@object is Semaphore typedObject))
            {
                return false;
            }
            else
            {
                return typedObject.Name.Equals(this.Name);
            }
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Name);
        }
    }
}
