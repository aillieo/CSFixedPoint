using System;
using System.Runtime.InteropServices;

namespace AillieoUtils.CSFixedPoint
{
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct Vector3 : IEquatable<Vector3>
    {
        public fp x;

        public fp y;

        public fp z;

        public Vector3(fp x, fp y, fp z) { this.x = x; this.y = y; this.z = z; }

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
                    case 2:
                        return z;
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
                    case 2:
                        z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException($"invalid index: {index}");
                }
            }
        }

        public static Vector3 operator +(Vector3 a, Vector3 b) { return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z); }

        public static Vector3 operator -(Vector3 a, Vector3 b) { return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z); }

        public static Vector3 operator -(Vector3 a) { return new Vector3(-a.x, -a.y, -a.z); }

        public static Vector3 operator *(Vector3 a, fp d) { return new Vector3(a.x * d, a.y * d, a.z * d); }

        public static Vector3 operator *(fp d, Vector3 a) { return new Vector3(a.x * d, a.y * d, a.z * d); }

        public static Vector3 operator /(Vector3 a, fp d) { return new Vector3(a.x / d, a.y / d, a.z / d); }

        public override bool Equals(object other)
        {
            if (!(other is Vector3))
            {
                return false;
            }

            return Equals((Vector3)other);
        }

        public bool Equals(Vector3 other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public fp Magnitude
        {
            get
            {
                return Mathfp.Sqrt(x * x + y * y + z * z);
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
                z *= inv;
            }
        }
    }
}
