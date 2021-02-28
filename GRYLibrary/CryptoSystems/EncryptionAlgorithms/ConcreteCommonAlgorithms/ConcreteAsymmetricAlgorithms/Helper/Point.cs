using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.CryptoSystems.EncryptionAlgorithms.ConcreteCommonAlgorithms.ConcreteAsymmetricAlgorithms.Helper
{
    public struct Point : IEquatable<Point>
    {
        public Curve UsedCurve { get; }
        public BigInteger X { get; }
        public BigInteger Y { get; }
        public Point(BigInteger x, BigInteger y, Curve usedCurve)
        {
            this.X = x;
            this.Y = y;
            this.UsedCurve = usedCurve;
        }

        public override bool Equals(object obj)
        {
            return obj is Point point && Equals(point);
        }

        public bool Equals(Point other)
        {
            return X.Equals(other.X) &&
                   Y.Equals(other.Y);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(Point left, Point right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Point left, Point right)
        {
            return !(left == right);
        }
        public static bool operator +(Point left, Point right)
        {
            throw new NotImplementedException();//see https://en.wikipedia.org/wiki/Elliptic_curve_point_multiplication#Point_addition
        }
        public static bool operator *(Point left, Point right)
        {
            throw new NotImplementedException();//see https://en.wikipedia.org/wiki/Elliptic_curve_point_multiplication#Point_doubling
        }
        public static bool operator -(Point left, Point right)
        {
            throw new NotImplementedException();//see https://en.wikipedia.org/wiki/Elliptic_curve_point_multiplication#Point_negation
        }
    }
}
