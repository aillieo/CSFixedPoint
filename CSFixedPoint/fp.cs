using System;
using System.Runtime.InteropServices;

namespace AillieoUtils.CSFixedPoint
{
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct fp : IComparable, IComparable<fp>, IEquatable<fp>
    {
        internal static readonly int FracBits = 32;
        internal static readonly long FracMask = (1l << FracBits) - 1;
        private static readonly int FracBitsMinusOne = FracBits - 1;
        private static readonly long FracMinusOneMask = (1l << FracBitsMinusOne) - 1;
        internal static readonly int LeftShiftMax = 62; // 64 - 1 - 1
        internal static readonly long Hi2Mask = 0x6000000000000000;  // 0b_110 0000 ...
        internal static readonly long Hi1Mask = 0x4000000000000000;  // 0b_100 0000 ...

        private static readonly long Denominator = FracMask + 1;
        private static readonly double DenominatorD = (double)Denominator;

        public static readonly fp Epsilon = new fp() { raw = 1 };
        public static readonly fp EpsilonSqrt = new fp() { raw = 1 << (FracBits / 2) };
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
            return new fp(){ raw = raw };
        }
        
        public static explicit operator double(fp value)
        {
            return (value.raw >> FracBits) + ((value.raw & FracMask) / DenominatorD);
        }

        public static explicit operator float(fp value)
        {
            return (float)(double)value;
        }

        public static explicit operator long(fp value)
        {
            return value.raw >> FracBits;
        }

        public static explicit operator int(fp value)
        {
            return (int)(long)value;
        }

        public static implicit operator fp(long value)
        {
            return new fp { raw = value << FracBits };
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
            int sign = ((lhs.raw >= 0) == (rhs.raw >= 0)) ? 1 : -1;
            long la = lhs.raw > 0 ? lhs.raw : -lhs.raw;
            long ra = rhs.raw > 0 ? rhs.raw : -rhs.raw;
            long lhsHi = la >> FracBits;
            long rhsHi = ra >> FracBits;
            // 两个low相乘 如果不使用ulong 也可能溢出
            // 为了和Java统一 把低32位再拆成1+31位计算
            //long lhsLo = la & FracMask;
            //long rhsLo = ra & FracMask;
            //long raw = sign * (
            //    ((lhsHi * rhsHi) << FracBits) +
            //    lhsHi * rhsLo +
            //    lhsLo * rhsHi +
            //    ((lhsLo * rhsLo) >> FracBits)
            //);

            long lhsMi = (la & FracMask) >> FracBitsMinusOne;
            long rhsMi = (ra & FracMask) >> FracBitsMinusOne;
            long lhsLo = la & FracMinusOneMask;
            long rhsLo = ra & FracMinusOneMask;
            long raw = sign * (
                ((lhsHi * rhsHi) << FracBits) +
                ((lhsHi * rhsMi) << FracBitsMinusOne) +
                lhsHi * rhsLo +
                ((lhsMi * rhsHi) << FracBitsMinusOne) +
                ((lhsMi * rhsMi) << (FracBitsMinusOne - 1)) +
                ((lhsMi * rhsLo) >> 1) +
                lhsLo * rhsHi +
                ((lhsLo * rhsMi) >> 1) +
                ((lhsLo * rhsLo) >> FracBits)
            );
            return new fp() { raw = raw };
        }

        public static fp operator /(fp lhs, fp rhs)
        {
            int sign = ((lhs.raw >= 0) == (rhs.raw >= 0)) ? 1 : -1;
            long la = lhs.raw > 0 ? lhs.raw : -lhs.raw;
            long ra = rhs.raw > 0 ? rhs.raw : -rhs.raw;
            long qu = la / ra;
            long re = la % ra;

            long qu0 = qu;
            long re0 = re;
            long ra0 = ra;
            int safeShift = 0;

            long result = qu0 << FracBits;

            // 当ra的最高位（不含符号）为1时 0b01__ ____ ...
            // 后边即使放大re 可能会一直被截断
            // 为了防止出现这种情况 直接把re和ra都右移一位
            // 损失一些精度 避免出现截断
            // 待优化
            if ((ra0 & Hi1Mask) != 0)
            {
                ra0 >>= 1;
                re0 >>= 1;
            }

            while (re0 != 0 && safeShift < LeftShiftMax)
            {
                while ((re0 & Hi2Mask) == 0 && safeShift + 2 <= LeftShiftMax)
                {
                    re0 <<= 2;
                    safeShift += 2;
                }
                while ((re0 & Hi1Mask) == 0 && safeShift + 1 <= LeftShiftMax)
                {
                    re0 <<= 1;
                    safeShift += 1;
                }

                if(FracBits - safeShift > 0)
                {
                    result = result + ((re0 / ra0) << (FracBits - safeShift));
                }
                else
                {
                    result = result + ((re0 / ra0) >> (safeShift - FracBits));
                }

                re0 = re0 % ra0;
            }

            return new fp() { raw = sign * result };
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
