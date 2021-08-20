using System;

namespace AillieoUtils.CSFixedPoint
{
    public struct Quaternion : IEquatable<Quaternion>
    {
        public fp x;
        public fp y;
        public fp z;
        public fp w;

        public Quaternion(fp x, fp y, fp z, fp w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public bool Equals(Quaternion other)
        {
            throw new NotImplementedException();
        }
    }
}
