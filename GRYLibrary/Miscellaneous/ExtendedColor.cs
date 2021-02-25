
namespace GRYLibrary.Core.Miscellaneous
{
    public class ExtendedColor
    {
        public static readonly ExtendedColor Black = new ExtendedColor(255, 0, 0, 0);
        public static readonly ExtendedColor White = new ExtendedColor(255, 255, 255, 255);
        public static readonly ExtendedColor Transparency = new ExtendedColor(0, 0, 0, 0);
        private readonly string _ARGBStringValue;
        private readonly string _RGBStringValue;
        public byte A { get; }
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }
        public System.Drawing.Color DrawingColor { get; }
        public int ColorCode { get; }

        public ExtendedColor() : this(0)
        {
        }
        public ExtendedColor(byte a, byte r, byte g, byte b) : this(System.Drawing.Color.FromArgb(a, r, g, b).ToArgb())
        {
        }
        public ExtendedColor(byte r, byte g, byte b) : this(System.Drawing.Color.FromArgb(r, g, b).ToArgb())
        {
        }
        public ExtendedColor(int colorCode)
        {
            this.ColorCode = colorCode;
            this.DrawingColor = System.Drawing.Color.FromArgb(this.ColorCode);
            this._ARGBStringValue = this.ColorCode.ToString("X8");
            this._RGBStringValue = this.DrawingColor.R.ToString("X2") + this.DrawingColor.G.ToString("X2") + this.DrawingColor.B.ToString("X2");
            this.A = this.DrawingColor.A;
            this.R = this.DrawingColor.R;
            this.G = this.DrawingColor.G;
            this.B = this.DrawingColor.B;
        }

        public string GetARGBString(bool withNumberSign = false)
        {
            if (withNumberSign)
            {
                return "#" + this._ARGBStringValue;
            }
            else
            {
                return this._ARGBStringValue;
            }
        }
        public string GetRGBString(bool withNumberSign = false)
        {
            if (withNumberSign)
            {
                return "#" + this._RGBStringValue;
            }
            else
            {
                return this._RGBStringValue;
            }
        }
        public override bool Equals(object @object)
        {
            if (@object is ExtendedColor color)
            {
                return this.ColorCode == color.ColorCode;
            }
            else
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            return this.ColorCode;
        }
        public override string ToString()
        {
            return $"{nameof(ExtendedColor)}({nameof(this.A)}={this.A},{nameof(this.R)}={this.R},{nameof(this.G)}={this.G},{nameof(this.B)}={this.B})";
        }

    }
}