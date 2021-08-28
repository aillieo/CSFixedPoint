﻿using System;

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

        public override bool Equals(object other)
        {
            if (other is Quaternion q)
            {
                return Equals(q);
            }

            return false;
        }

        public bool Equals(Quaternion other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w);
        }

        public static fp Dot(Quaternion a, Quaternion b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        public static Quaternion operator -(Quaternion a) { return new Quaternion(-a.x, -a.y, -a.z, -a.w); }
    }
}
