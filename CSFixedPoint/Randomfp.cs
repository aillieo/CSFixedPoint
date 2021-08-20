using System;

namespace AillieoUtils.CSFixedPoint
{
    public class Randomfp
    {
        private static readonly int Lo31Mask = 0x7FFFFFFF;  // 0b0111 1111 ...
        private static readonly long Hi1Mask = 1 << 31;

        public Randomfp() : this(0){}

        public Randomfp(int seed)
        {
            this.seed = seed;
            this.state = seed;
        }

        public readonly int seed;
        private int state;

        // https://pubs.opengroup.org/onlinepubs/9699919799/
        public int Next() 
        {
            // max:32767 16bit
            state = state * 1103515245 + 12345;
            // 为兼容Java 这里绕开uint 使用long存储
            long uintState = state >= 0 ? state : (long)((state & Lo31Mask) + Hi1Mask);
            return (int)((uintState / 65536) % 32768);
        }

        public int Next(int minValue, int maxValue) { throw new NotImplementedException(); }

        public fp Nextfp() { throw new NotImplementedException(); }

        public int Nextfp(fp minValue, fp maxValue) { throw new NotImplementedException(); }


        public Quaternion Rotation() { throw new NotImplementedException(); }

        public Vector3 OnUnitSphere() { throw new NotImplementedException(); }

        public Vector2 InsideUnitCircle() { throw new NotImplementedException(); }

        public Vector3 InsideUnitSphere() { throw new NotImplementedException(); }

        public Quaternion RotationUniform() { throw new NotImplementedException(); }
    }
}
