﻿using GRYLibrary.GRYObjectSystem.Meta.Property;
using System;

namespace GRYLibrary.Miscellaneous
{
    public sealed class Semaphore : Property<int>
    {
        public Semaphore(string propertyName, bool addValuesToHistory = false) : base(0, propertyName, addValuesToHistory)
        {
            this.LockEnabled = true;
        }
        public override int Value { get => base.Value; set => throw new InvalidOperationException($"Please use the {nameof(Semaphore.Increment)}- and {nameof(Semaphore.Decrement)}-operation to modify the value."); }
        public void Increment()
        {
            base.Value = base.Value + 1;
        }
        public void Decrement()
        {
            base.Value = base.Value - 1;
        }

    }
}
