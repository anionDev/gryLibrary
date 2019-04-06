using GRYLibrary.GRYObjectSystem.Meta.Property;
using System;

namespace GRYLibrary.Miscellaneous
{
    public sealed class Semaphore : Property<long>
    {
        public Semaphore(string propertyName, bool addValuesToHistory = false) : base(0, propertyName, addValuesToHistory)
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

    }
}
