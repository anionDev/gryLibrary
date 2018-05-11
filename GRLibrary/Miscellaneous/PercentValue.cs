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
                return (int)Math.Round(this._Value * 100);
            }
        }
        public decimal Value
        {
            get
            {
                return this._Value;
            }
        }
        private readonly decimal _Value;
        public PercentValue(decimal value)
        {
            if (value < 0)
            {
                this._Value = 0;
            }
            else if (value > 1)
            {
                this._Value = 1;
            }
            else
            {
                this._Value = value;
            }
        }
        public PercentValue(int percentValue)
        {
            if (percentValue < 0)
            {
                this._Value = 0;
            }
            else if (percentValue > 100)
            {
                this._Value = 1;
            }
            else
            {
                this._Value = (decimal)percentValue / 100;
            }
        }

    }
}
