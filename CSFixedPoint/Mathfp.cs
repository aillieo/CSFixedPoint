using System;

namespace AillieoUtils.CSFixedPoint
{
    public static class Mathfp
    {
        public static readonly fp PI = new fp(3, (uint)((Math.PI - 3) * uint.MaxValue));

        public static readonly fp Deg2Rad = PI / (fp)180;

        public static readonly fp Rad2Deg = (fp)180 / PI;

        public static readonly fp E = new fp(2, (uint)((Math.E - 2) * uint.MaxValue));

        public static fp Abs(fp f) { return f > fp.Zero ? f : -f; }

        public static fp Sign(fp f) { return f > fp.Zero ? fp.One : fp.MinusOne; }

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

        public static fp Sqrt(fp f) { throw new NotImplementedException(); }

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
    }
}
