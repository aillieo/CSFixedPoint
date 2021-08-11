using System;

namespace AillieoUtils.CSFixedPoint
{
    public static class Mathfp
    {
        public static readonly fp PI = fp.Nearest(Math.PI);

        public static readonly fp Deg2Rad = PI / (fp)180;

        public static readonly fp Rad2Deg = (fp)180 / PI;

        public static readonly fp E = fp.Nearest(Math.E);

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

        public static fp Sin(fp f) { throw new NotImplementedException(); }

        public static fp Cos(fp f) { throw new NotImplementedException(); }

        public static fp Tan(fp f) { throw new NotImplementedException(); }

        public static fp Asin(fp f) { throw new NotImplementedException(); }

        public static fp Acos(fp f) { throw new NotImplementedException(); }

        public static fp Atan(fp f) { throw new NotImplementedException(); }

        public static fp Atan2(fp y, fp x) { throw new NotImplementedException(); }

        public static fp Pow(fp f, fp p) { throw new NotImplementedException(); }

        public static fp Exp(fp power) { throw new NotImplementedException(); }

        public static fp Log(fp f, fp p) { throw new NotImplementedException(); }

        public static fp Log(fp f) { throw new NotImplementedException(); }

        public static fp Log10(fp f) { throw new NotImplementedException(); }

        public static fp Ceil(fp f) { throw new NotImplementedException(); }

        public static fp Floor(fp f) { throw new NotImplementedException(); }

        public static fp Round(fp f) { throw new NotImplementedException(); }

        public static int CeilToInt(fp f) { throw new NotImplementedException(); }

        public static int FloorToInt(fp f) { throw new NotImplementedException(); }

        public static int RoundToInt(fp f) { throw new NotImplementedException(); }

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
    }
}
