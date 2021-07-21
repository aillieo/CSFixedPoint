using System;
using System.Runtime.InteropServices;

namespace AillieoUtils.CSFixedPoint
{
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct fp : IComparable, IComparable<fp>, IEquatable<fp>
    {
#if FP_FRAC_BITS_8
        private static readonly int FracBits = 8;

#elif FP_FRAC_BITS_16
        private static readonly int FracBits = 16;
#elif FP_FRAC_BITS_24
        private static readonly int FracBits = 24;
#else
        private static readonly int FracBits = 16;
#endif
        private static readonly int FracMask = (1 << FracBits) - 1;
        private static readonly int IntMask = -1 & ~FracMask;

        private static readonly int Denominator = FracMask + 1;
        private static readonly double DenominatorD = (double)Denominator;
        private static readonly float DenominatorF = (float)Denominator;

        public static readonly fp Epsilon = new fp() { raw = 1 };
        public static readonly fp One = new fp() { raw = 1 << FracBits };
        public static readonly fp MinusOne = new fp() { raw = (-1) << FracBits };
        public static readonly fp Zero = new fp() { raw = 0 };
        public static readonly fp MinValue = new fp() { raw = int.MinValue };
        public static readonly fp MaxValue = new fp() { raw = int.MaxValue };

        private int raw;

        internal fp(int int32, uint frac32)
        {
            if (int32 > fp.MaxValue)
            {
                throw new ArgumentOutOfRangeException($"arg greater than maxValue: {nameof(int32)}={int32}");
            }

            if (int32 < fp.MinValue)
            {
                throw new ArgumentOutOfRangeException($"arg less than minValue: {nameof(int32)}={int32}");
            }

            raw = (int32 << FracBits) + (int)(frac32 / (((long)uint.MaxValue + 1) / Denominator));
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
            return (value.raw + FracMask) >> FracBits;
        }

        public static implicit operator fp(int value)
        {
            return new fp { raw = value << FracBits };
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
            return new fp() { raw = (int)(((long)lhs.raw * (long)rhs.raw + (Denominator >> 1)) >> FracBits) };
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
