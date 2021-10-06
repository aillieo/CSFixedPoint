using System;
using System.Runtime.InteropServices;

namespace AillieoUtils.CSFixedPoint
{
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct Vector2 : IEquatable<Vector2>
    {
        public fp x;

        public fp y;

        public Vector2(fp x, fp y) { this.x = x; this.y = y; }

        public fp this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    default:
                        throw new IndexOutOfRangeException($"invalid index: {index}");
                }
            }

            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException($"invalid index: {index}");
                }
            }
        }

        public static Vector2 operator +(Vector2 a, Vector2 b) { return new Vector2(a.x + b.x, a.y + b.y); }

        public static Vector2 operator -(Vector2 a, Vector2 b) { return new Vector2(a.x - b.x, a.y - b.y); }

        public static Vector2 operator *(Vector2 a, Vector2 b) { return new Vector2(a.x * b.x, a.y * b.y); }

        public static Vector2 operator /(Vector2 a, Vector2 b) { return new Vector2(a.x / b.x, a.y / b.y); }

        public static Vector2 operator -(Vector2 a) { return new Vector2(-a.x, -a.y); }

        public static Vector2 operator *(Vector2 a, fp d) { return new Vector2(a.x * d, a.y * d); }

        public static Vector2 operator *(fp d, Vector2 a) { return new Vector2(a.x * d, a.y * d); }

        public static Vector2 operator /(Vector2 a, fp d) { return new Vector2(a.x / d, a.y / d); }

        public override bool Equals(object other)
        {
            if (!(other is Vector2))
            {
                return false;
            }

            return Equals((Vector2)other);
        }

        public bool Equals(Vector2 other)
        {
            return x == other.x && y == other.y;
        }

        public fp Magnitude
        {
            get
            {
                return Mathfp.Sqrt(x * x + y * y);
            }
        }

        public void Normalize()
        {
            fp mag = Magnitude;
            if (mag > fp.EpsilonSqrt)
            {
                fp inv = fp.One / mag;
                x *= inv;
                y *= inv;
            }
        }
    }
}
