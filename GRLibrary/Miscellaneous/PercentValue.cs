using System;

namespace GRLibrary
{
    /// <summary>
    /// Represents a number between 0 and 1.
    /// </summary>
    public class PercentValue
    {
        public int ValueInPercent
        {
            get
            {
                return (int)Math.Round(this.Value * 100);
            }
        }
        public decimal Value { get; }

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
        public PercentValue(int percentValue)
        {
            if (percentValue < 0)
            {
                this.Value = 0;
            }
            else if (percentValue > 100)
            {
                this.Value = 1;
            }
            else
            {
                this.Value = (decimal)percentValue / 100;
            }
        }

    }
}
