using System;
using System.Runtime.InteropServices;

namespace AillieoUtils.CSFixedPoint
{
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct fp : IComparable, IComparable<fp>, IEquatable<fp>
    {
        private static readonly int FracBits = 32;
        private static readonly long FracMask = (1l << FracBits) - 1;
        private static readonly long IntMask = -1l & ~FracMask;

        private static readonly long Denominator = FracMask + 1;
        private static readonly double DenominatorD = (double)Denominator;
        private static readonly float DenominatorF = (float)Denominator;

        public static readonly fp Epsilon = new fp() { raw = 1 };
        public static readonly fp One = new fp() { raw = 1l << FracBits };
        public static readonly fp MinusOne = new fp() { raw = (-1l) << FracBits };
        public static readonly fp Zero = new fp() { raw = 0 };
        public static readonly fp MinValue = new fp() { raw = long.MinValue };
        public static readonly fp MaxValue = new fp() { raw = long.MaxValue };

        internal long raw;

        internal static fp Nearest(double value)
        {
            if (value > (double)fp.MaxValue)
            {
                throw new ArgumentOutOfRangeException($"arg greater than maxValue: {nameof(value)}={value}");
            }

            if (value < (double)fp.MinValue)
            {
                throw new ArgumentOutOfRangeException($"arg less than minValue: {nameof(value)}={value}");
            }

            long int32 = (long)Math.Floor(value);
            long frac32 = (long)((value - int32) * DenominatorD);
            long raw = (int32 << FracBits) + frac32;
            return new fp() { raw = raw};
        }

        internal static fp CreateWithRaw(long raw)
        {
            return new fp(){ raw = raw};
        }
        
        public static explicit operator double(fp value)
        {
            return (double)(value.raw >> FracBits) + ((value.raw & FracMask) / DenominatorD);
        }

        public static explicit operator float(fp value)
        {
            return (float)(value.raw >> FracBits) + ((value.raw & FracMask) / DenominatorF);
        }

        public static explicit operator int(fp value)
        {
            return (int)((value.raw + FracMask) >> FracBits);
        }

        public static implicit operator fp(int value)
        {
            return new fp { raw = (long)value << FracBits };
        }

        public static bool operator ==(fp lhs, fp rhs)
        {
            return lhs.raw == rhs.raw;
        }

        public static bool operator !=(fp lhs, fp rhs)
        {
            return lhs.raw != rhs.raw;
        }

        public static bool operator >(fp lhs, fp rhs)
        {
            return lhs.raw > rhs.raw;
        }

        public static bool operator >=(fp lhs, fp rhs)
        {
            return lhs.raw >= rhs.raw;
        }

        public static bool operator <(fp lhs, fp rhs)
        {
            return lhs.raw < rhs.raw;
        }

        public static bool operator <=(fp lhs, fp rhs)
        {
            return lhs.raw <= rhs.raw;
        }

        public static fp operator +(fp value)
        {
            return value;
        }

        public static fp operator -(fp value)
        {
            return new fp() { raw = -value.raw };
        }

        public static fp operator ++(fp value)
        {
            return new fp() { raw = value.raw + One.raw };
        }

        public static fp operator --(fp value)
        {
            return new fp() { raw = value.raw - One.raw };
        }

        public static fp operator +(fp lhs, fp rhs)
        {
            return new fp() { raw = lhs.raw + rhs.raw };
        }

        public static fp operator -(fp lhs, fp rhs)
        {
            return new fp() { raw = lhs.raw - rhs.raw };
        }

        public static fp operator *(fp lhs, fp rhs)
        {
            long lhsHi = lhs.raw >> FracBits;
            long lhsLo = lhs.raw & FracMask;
            long rhsHi = rhs.raw >> FracBits;
            long rhsLo = rhs.raw & FracMask;
            long raw =
                ((lhsHi * rhsHi) << FracBits) +
                lhsHi * rhsLo +
                lhsLo * rhsHi + 
                ((lhsLo * rhsLo) >> FracBits);
            return new fp(){raw = raw};
        }

        public static fp operator /(fp lhs, fp rhs)
        {
            return new fp() { raw = (int)((((long)lhs.raw << (FracBits + 1)) / (long)rhs.raw + 1) >> 1) };
        }

        public static fp operator %(fp lhs, fp rhs)
        {
            return new fp() { raw = lhs.raw % rhs.raw };
        }

        public static fp operator <<(fp lhs, int rhs)
        {
            return new fp() { raw = lhs.raw << rhs };
        }

        public static fp operator >>(fp lhs, int rhs)
        {
            return new fp() { raw = lhs.raw >> rhs };
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            if (!(obj is fp))
            {
                throw new ArgumentException($"arg must be {typeof(fp)}");
            }

            fp other = (fp)obj;
            return CompareTo(other);
        }

        public int CompareTo(fp other)
        {
            return this.raw.CompareTo(other.raw);
        }

        public bool Equals(fp other)
        {
            return this.raw.Equals(other.raw);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is fp))
            {
                return false;
            }

            return this.Equals((fp)obj);
        }

        public override int GetHashCode()
        {
            return raw.GetHashCode();
        }

        public override string ToString()
        {
            return $"{(double)this} <int={raw >> FracBits} frac={raw & FracMask}/{Denominator} raw={raw}>";
        }
    }
}
