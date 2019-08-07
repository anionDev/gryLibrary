using System.Linq;
using System.Numerics;
using System.Text;

namespace GRYLibrary.Miscellaneous
{
    public class ByteArray
    {
        public static Encoding DefaultEncoding { get; } = new UTF8Encoding(false);
        public byte[] Data { get; }
        public string DataAsHexString { get; }
        public ByteArray(byte[] value)
        {
            this.Data = value;
            this.DataAsHexString = Utilities.ByteArrayToHexString(this.Data);
        }
        public ByteArray CreateByHexString(string value)
        {
            return new ByteArray(Utilities.HexStringToByteArray(value));
        }
        public ByteArray CreateByInteger(BigInteger value)
        {
            return this.CreateByHexString(Utilities.IntegerToHexString(value));
        }
        public ByteArray CreateByString(string value)
        {
            return this.CreateByString(value, DefaultEncoding);
        }
        public ByteArray CreateByString(string value, Encoding encoding)
        {
            return new ByteArray(encoding.GetBytes(value));
        }
        public string DataAsString()
        {
            return this.DataAsString(DefaultEncoding);
        }
        public string DataAsString(Encoding encoding)
        {
            return encoding.GetString(this.Data);
        }
        public static ByteArray operator +(ByteArray first, ByteArray second)
        {
            return new ByteArray(Utilities.Concat(first.Data, second.Data));
        }
        public static bool operator ==(ByteArray first, ByteArray second)
        {
            return first.Equals(second);
        }
        public static bool operator !=(ByteArray first, ByteArray second)
        {
            return !(first == second);
        }
        public override bool Equals(object obj)
        {
            ByteArray other = obj as ByteArray;
            return other != null && Enumerable.SequenceEqual(this.Data, other.Data);
        }
        public override int GetHashCode()
        {
            return this.Data.GetHashCode();
        }
    }
}
