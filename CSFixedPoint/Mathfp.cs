using System;

namespace AillieoUtils.CSFixedPoint
{
    public static class Mathfp
    {
        public static readonly fp PI = fp.Nearest(Math.PI);

        public static readonly fp Deg2Rad = PI / (fp)180;

        public static readonly fp Rad2Deg = (fp)180 / PI;

        public static readonly fp E = fp.Nearest(Math.E);
        
        private static readonly fp Ln2 = fp.Nearest(Math.Log(2));

        private static readonly fp Log10_2 = fp.Nearest(Math.Log(2, 10));

        public static fp Abs(fp f)
        {
            if (f <= int.MinValue)
            {
                return -(f + 1);
            }
            return f > fp.Zero ? f : -f;
        }

        public static int Sign(fp f) { return f >= fp.Zero ? 1 : -1; }

        public static fp Min(fp a, fp b) { return a < b ? a : b; }

        public static fp Min(params fp[] values)
        {
            int len = values.Length;
            if (len == 0)
            {
                throw new ArgumentException();
            }

            fp m = values[0];
            for (int i = 1; i < len; i++)
            {
                if (values[i] < m)
                {
                    m = values[i];
                }
            }

            return m;
        }

        public static fp Max(fp a, fp b) { return a > b ? a : b; }

        public static fp Max(params fp[] values)
        {
            int len = values.Length;
            if (len == 0)
            {
                throw new ArgumentException();
            }

            fp m = values[0];
            for (int i = 1; i < len; i++)
            {
                if (values[i] > m)
                {
                    m = values[i];
                }
            }

            return m;
        }

        public static fp Clamp(fp value, fp min, fp max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }

            return value;
        }

        public static fp Clamp01(fp value)
        {
            if (value < fp.Zero)
            {
                return fp.Zero;
            }
            else if (value > fp.One)
            {
                return fp.One;
            }
            else
            {
                return value;
            }
        }

        public static fp Lerp(fp a, fp b, fp t)
        {
            return a + (b - a) * Clamp01(t);
        }

        public static fp LerpUnclamped(fp a, fp b, fp t)
        {
            return a + (b - a) * t;
        }

        public static fp Sqrt(fp f)
        {
            if (f == fp.One)
            {
                return fp.One;
            }
            if (f == fp.Zero)
            {
                return fp.Zero;
            }

            long r0 = f.raw;
            int safeShift = 0;

            // 尝试左移 当fp比较小的时候 放大以提高精度
            while ((r0 & fp.Hi2Mask) == 0 && safeShift + 2 <= fp.LeftShiftMax)
            {
                r0 <<= 2;
                safeShift += 2;
            }

            long result;
            if (fp.FracBits - safeShift > 0)
            {
                result = SqrtL(r0) << ((fp.FracBits - safeShift) / 2);
            }
            else
            {
                result = SqrtL(r0) >> ((safeShift - fp.FracBits) / 2);
            }

            return new fp() { raw = result };
        }

        //// 牛顿迭代法
        //public static fp Sqrt(fp f)
        //{
        //    if (f == fp.One)
        //    {
        //        return fp.One;
        //    }
        //    if (f == fp.Zero)
        //    {
        //        return fp.Zero;
        //    }

        //    fp t = f;
        //    fp x0 = f;
        //    fp two = (fp)2;
        //    x0 = x0 / two + t / (two * x0);
        //    // 注意需要防止溢出 x0*x0
        //    while (x0 * x0 - t > fp.Epsilon * 10)
        //    {
        //        x0 = x0 / two + t / (two * x0);
        //    }
        //    return x0;
        //}

        public static fp Sin(fp f) 
        {
            fp f0 = f % (2 * PI);
            if (f0 < fp.Zero)
            {
                f0 += 2 * PI;
            }
            bool flip = false;
            if (f0 > PI)
            {
                f0 = f0 - PI;
                flip = true;
            }
            if (f0 > PI / 2)
            {
                f0 = PI - f0;
            }

            fp p = f0 * FPSinLut.oneOverStep;
            int left = FloorToInt(p);
            int right = CeilToInt(p);

            fp lf = FPSinLut.Get(left);
            fp rf = FPSinLut.Get(right);
            fp result = Lerp(lf, rf, p);
            return flip ? -result : result;
        }

        public static fp Cos(fp f) 
        {
            return Sin(PI / 2 - f);
        }

        public static fp Tan(fp f)
        {
            fp f0 = f % PI;
            if (f0 < fp.Zero)
            {
                f0 += PI;
            }
            bool flip = false;
            if (f0 > PI / 2)
            {
                f0 = PI - f0;
                flip = true;
            }

            fp p = f0 * FPTanLut.oneOverStep;
            int left = FloorToInt(p);
            int right = CeilToInt(p);

            // tan在 PI/2 附近的精度很底 需要做一些特殊处理
            // 待优化
            if (right == FPTanLut.length)
            {
                return Sin(f) / Cos(f);
            }

            fp lf = FPTanLut.Get(left);
            fp rf = FPTanLut.Get(right);
            fp result = Lerp(lf, rf, p);
            return flip ? -result : result;
        }

        public static fp Asin(fp f) { throw new NotImplementedException(); }

        public static fp Acos(fp f) { throw new NotImplementedException(); }

        public static fp Atan(fp f) { throw new NotImplementedException(); }

        public static fp Atan(fp y, fp x) { throw new NotImplementedException(); }

        public static fp Pow(fp f, fp p) 
        {
            if (p == fp.Zero)
            {
                return fp.One;
            }

            if (f == fp.One)
            {
                return fp.One;
            }

            fp p0 = p;
            bool neg = false;
            if (p0 < fp.Zero)
            {
                p0 = -p0;
                neg = true;
            }

            if (p0 == fp.One)
            {
                return neg ? fp.One / f : f;
            }

            throw new NotImplementedException(); 
        }

        public static fp Exp(fp power) 
        {
            if (power == fp.Zero)
            {
                return fp.One;
            }

            // https://en.wikipedia.org/wiki/Generalized_continued_fraction
            throw new NotImplementedException(); 
        }

        public static fp Log(fp f, fp newBase) 
        {
            return Log2(f) / Log2(newBase);
        }

        public static fp Log(fp f) 
        {
            return Log2(f) * Ln2;
        }

        public static fp Log10(fp f) 
        {
            return Log2(f) * Log10_2;
        }

        public static fp Ceil(fp f) 
        {
            bool hasFrac = (f.raw & fp.FracMask) != 0;
            fp floor = Floor(f);
            return hasFrac ? floor + fp.One : floor;
        }

        public static fp Floor(fp f) 
        {
            return new fp() { raw = f.raw & (~fp.FracMask) };
        }

        public static fp Round(fp f)
        {
            long frac = f.raw & fp.FracMask;
            fp floor = Floor(f);
            if (frac < (fp.One.raw >> 1))
            {
                return floor;
            }
            else
            {
                return floor + fp.One;
            }
        }

        public static int CeilToInt(fp f)
        {
            bool hasFrac = (f.raw & fp.FracMask) != 0;
            int floor = FloorToInt(f);
            return hasFrac ? floor + 1 : floor;
        }

        public static int FloorToInt(fp f) 
        {
            return (int)f;
        }

        public static int RoundToInt(fp f) 
        {
            return (int)Round(f);
        }

        // https://en.wikipedia.org/wiki/Methods_of_computing_square_roots
        private static long SqrtL(long l)
        {
            long res = 0;
            long one = fp.Hi1Mask;
            long op = l;

            while (one > op)
            {
                one >>= 2;
            }

            while (one > 0)
            {
                long t = res + one;
                res >>= 1;
                if (op >= t)
                {
                    op -= t;
                    res += one;
                }
                one >>= 2;
            }

            return res;
        }

        //public static fp Log2(fp f)
        //{
        //    long raw = Log2L(f.raw) - fp.FracBits;
        //    // todo 最后再<<fp.FracBits    截断损失太大了
        //    return new fp() { raw = raw << fp.FracBits };
        //}

        // http://www.claysturner.com/dsp/BinaryLogarithm.pdf
        public static fp Log2(fp f) 
        {
            fp two = (fp)2;
            fp half = fp.One / two;
            fp err = fp.Epsilon * 10;

            // 1.
            fp result = fp.Zero;
            // 2.
            fp b = half;
            // 3.
            while (f < fp.One)
            {
                f <<= 1;
                result--;
            }
            // 4.
            while (f >= two)
            {
                f >>= 1;
                result++;
            }

            while(b >= err)
            {
                // 6.
                f = f * f;
                // 7.
                if (f >= two)
                {
                    f >>= 1;
                    result += b;
                }
                b >>= 1;
            }

            return result;
        }

        // http://graphics.stanford.edu/~seander/bithacks.html#IntegerLog
        private static readonly long[] l2bits = new long[] { 0x0FFFFFFF00000000, 0xFFFF0000, 0xFF00, 0xF0, 0xC, 0x2 };
        private static readonly int[] l2shifts = new int[] { 1 << 5, 1 << 4, 1 << 3, 1 << 2, 1 << 1, 1 << 0 };
        private static long Log2L(long l)
        {
            long result = 0;
            for (int i = 0; i < 6; i++)
            {
                if ((l & l2bits[i]) != 0)
                {
                    l >>= l2shifts[i];
                    result |= (long)l2shifts[i];
                }
            }

            return result;
        }

        private static fp Pow2(fp f) { throw new NotImplementedException(); }

    }
}
