using System;

namespace GRLibrary//test
{
    public class ColorGradient
    {
        public ExtendedColor StartColor { get; private set; }
        public ExtendedColor DestinationColor { get; private set; }
        public ColorGradient(ExtendedColor startColor, ExtendedColor destinationColor)
        {
            this.DestinationColor = destinationColor;
            this.StartColor = startColor;
        }
        public ExtendedColor GetColorGradientValue(double gradient)
        {
            PercentValue percentValue = new PercentValue((decimal)gradient);
            int a = GetGradient(this.StartColor.A, this.DestinationColor.A, percentValue);
            int r = GetGradient(this.StartColor.R, this.DestinationColor.R, percentValue);
            int g = GetGradient(this.StartColor.G, this.DestinationColor.G, percentValue);
            int b = GetGradient(this.StartColor.B, this.DestinationColor.B, percentValue);
            return new ExtendedColor(a, r, g, b);
        }

        private int GetGradient(int startValue, int destinationValue, PercentValue gradient)
        {
            return (int)Math.Round(startValue + (destinationValue - startValue) * gradient.Value, 0);
        }
    }
}
