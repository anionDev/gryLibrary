using System;

namespace GRYLibrary.Core
{
    /// <summary>
    /// Represents a <see cref="decimal"/>-number between 0 and 1.
    /// </summary>
    public struct PercentValue
    {
        public static PercentValue ZeroPercent { get; } = new PercentValue((decimal)0);
        public static PercentValue HundredPercent { get; } = new PercentValue((decimal)1);
        public int ValueInPercent
        {
            get
            {
                return (int)Math.Round(this.Value * 100);
            }
        }
        public decimal Value { get; }

        public PercentValue(double value) : this((decimal)value)
        {
        }
        public PercentValue(decimal value)
        {
            if (value < 0)
            {
                this.Value = 0;
            }
            else if (value > 1)
            {
                this.Value = 1;
            }
            else
            {
                this.Value = value;
            }
        }
        public static PercentValue CreateByPercentValue(int percentValue)
        {
            if (percentValue < 0)
            {
                return ZeroPercent;
            }
            else if (percentValue > 100)
            {
                return HundredPercent;
            }
            else
            {
                return new PercentValue((decimal)percentValue / 100);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is PercentValue value && Value == value.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public static bool operator ==(PercentValue left, PercentValue right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PercentValue left, PercentValue right)
        {
            return !(left == right);
        }
    }
}
