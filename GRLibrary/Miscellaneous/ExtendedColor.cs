namespace GRLibrary
{
    public class ExtendedColor
    {
        public static readonly ExtendedColor Black = new ExtendedColor(255, 0, 0, 0);
        public static readonly ExtendedColor White = new ExtendedColor(255, 255, 255, 255);
        public static readonly ExtendedColor Transparency = new ExtendedColor(0, 0, 0, 0);

        private int _EncodingValue;
        private int _A;
        private int _R;
        private int _G;
        private int _B;
        private System.Drawing.Color _ColorValue;
        private System.Windows.Media.Color _MediaColorValue;
        private string _ARGBStringValue;
        private string _RGBStringValue;
        private System.Windows.Media.Brush _BrushValue;
        public int Encoding
        {
            get { return this._EncodingValue; }
        }
        public ExtendedColor() : this(0)
        {
        }
        public ExtendedColor(int a, int r, int g, int b) : this(System.Drawing.Color.FromArgb(a, r, g, b).ToArgb())
        {
        }
        public ExtendedColor(int r, int g, int b) : this(System.Drawing.Color.FromArgb(r, g, b).ToArgb())
        {
        }
        public ExtendedColor(int colorEncodingValue)
        {
            //TODO: add variants without transparency
            this._EncodingValue = colorEncodingValue;
            this._ColorValue = System.Drawing.Color.FromArgb(this.Encoding);
            this._MediaColorValue = System.Windows.Media.Color.FromArgb(this.DrawingColor.A, this.DrawingColor.R, this.DrawingColor.G, this.DrawingColor.B);
            this._ARGBStringValue = this._EncodingValue.ToString("X8");
            this._RGBStringValue = this.DrawingColor.R.ToString("X2") + this.DrawingColor.G.ToString("X2") + this.DrawingColor.B.ToString("X2");
            this._BrushValue = new System.Windows.Media.SolidColorBrush(this.MediaColor);
            this._A = this.DrawingColor.A;
            this._R = this.DrawingColor.R;
            this._G = this.DrawingColor.G;
            this._B = this.DrawingColor.B;
        }
        public System.Drawing.Color DrawingColor { get { return this._ColorValue; } }
        public System.Windows.Media.Brush Brush { get { return this._BrushValue; } }
        public System.Windows.Media.Color MediaColor { get { return this._MediaColorValue; } }
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
        public override bool Equals(object obj)
        {
            if (obj is ExtendedColor)
            {
                return this.Encoding == ((ExtendedColor)obj).Encoding;
            }
            else
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            return this.Encoding;
        }
        public int A
        {
            get { return this._A; }
        }
        public int R
        {
            get { return this._R; }
        }
        public int G
        {
            get { return this._G; }
        }
        public int B
        {
            get { return this._B; }
        }
    }
}