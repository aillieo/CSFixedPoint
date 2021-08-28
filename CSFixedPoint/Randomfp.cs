using System;

namespace AillieoUtils.CSFixedPoint
{
    public class Randomfp
    {
        private static readonly int Lo31Mask = 0x7FFFFFFF;  // 0b0111 1111 ...
        private static readonly long Hi1Mask = 1l << 31;
        private static readonly long Hi1Mask64 = 1l << 63;
        private static readonly int RandMax = 32767;
        private static readonly int longValueBits = 62;

        public Randomfp() : this(0){}

        public Randomfp(int seed)
        {
            this.seed = seed;
            this.state = seed;
            this.buffer = 0;

            for (int i = 0; i < 5; ++ i)
            {
                Next();
            }
        }

        public readonly int seed;
        private int state;
        private long buffer;

        // https://pubs.opengroup.org/onlinepubs/9699919799/
        public int Next() 
        {
            // max:32767 16bit
            state = state * 1103515245 + 12345;
            // 为兼容Java 这里绕开uint 使用long存储
            long uintState = state >= 0 ? state : (long)((state & Lo31Mask) + Hi1Mask);
            int rand = (int)((uintState / 65536) % 32768);
            buffer = (buffer << 15) + rand;
            return rand;
        }

        public int NextInt()
        {
            Next();
            return (int)(buffer & fp.FracMask);
        }

        // 包含min 不包含max
        public int NextInt(int minValue, int maxValue) 
        {
            if (minValue > maxValue)
            {
                int t = minValue;
                minValue = maxValue;
                maxValue = t;
            }
            else if (minValue == maxValue)
            {
                return minValue;
            }

            long diff = maxValue - minValue;
            long diff0 = diff;
            int diffBits = 0;
            while (diff0 != 0)
            {
                diff0 >>= 1;
                diffBits++;
            }
            // 用long是因为可能超出int范围（小于uint范围）
            long rand;
            do
            {
                Next();
                rand = buffer & ((1 << (diffBits + 1)) - 1);
            }
            while (rand >= diff);

            return minValue + (int)rand;
        }

        // 包含0 不包含1
        public fp Nextfp() 
        {
            // 随机的32位填给raw的Frac部分
            Next();
            return new fp { raw = buffer & fp.FracMask };
        }

        // 包含min 包含max 和UnityEngine.Random保持一致吧
        public fp Nextfp(fp minValue, fp maxValue)
        {
            long lmin = minValue.raw;
            long lmax = maxValue.raw;

            if (lmin > lmax)
            {
                long t = lmin;
                lmin = lmax;
                lmax = t;
            }
            else if (lmin == lmax)
            {
                return minValue;
            }

            // 理论上diff是会超过long的 需要做一些拆分
            long diff = lmax - lmin;
            bool overflow = ((lmax & Hi1Mask64) != (lmin & Hi1Mask64)) && ((lmax & Hi1Mask64) != (diff & Hi1Mask64));
            long raw;
            if (overflow)
            {
                // diff的范围已超出long.MaxValue
                // 需要处理成两部分
                diff = (lmax >> 1) - (lmin >> 1);
                // 将整个diff范围平均分成两部分
                Next();
                // 如果Next()得到奇数 在低half中随机 否则在高half中随机
                bool low = (buffer & 1) == 1;
                if (low)
                {
                    lmax -= diff;
                    lmax -= 1;
                }
                else
                {
                    lmin += diff;
                    lmin += 1;
                }
                diff = lmax - lmin;
            }

            long rand;
            long diff0 = diff;
            int diffBits = 0;
            while (diff0 != 0 && diffBits < longValueBits)
            {
                diff0 >>= 1;
                diffBits++;
            }
            do
            {
                Next();
                rand = buffer & ((1l << (diffBits + 1)) - 1);
            }
            while (rand > diff);
            raw = lmin + rand;

            return new fp() { raw = raw };
        }

        public Vector2 OnUnitCircle()
        {
            fp a = Nextfp(fp.Zero, Mathfp.PI * 2);
            return new Vector2(Mathfp.Cos(a), Mathfp.Sin(a));
        }

        public Vector2 InsideUnitCircle()
        {
            Vector2 v = OnUnitCircle();
            return v *= Mathfp.Pow(Nextfp(), fp.One / 2);
        }

        public Vector3 OnUnitSphere()
        {
            fp z = Nextfp(fp.MinusOne, fp.One);
            fp a = Nextfp(fp.Zero, Mathfp.PI * 2);
            fp r = Mathfp.Sqrt(fp.One - z * z);
            return new Vector3(r * Mathfp.Cos(a), r * Mathfp.Sin(a), z);
        }

        public Vector3 InsideUnitSphere() 
        {
            Vector3 v = OnUnitSphere();
            return v *= Mathfp.Pow(Nextfp(), fp.One / 3);
        }

        public Quaternion RotationUniform() { throw new NotImplementedException(); }

        public Quaternion Rotation() { throw new NotImplementedException(); }
    }
}
