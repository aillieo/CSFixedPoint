using System;
using Xunit;
namespace AillieoUtils.CSFixedPoint.Test
{
    public class FixedPointTest
    {
        [Fact]
        public static void FixedPointTest1()
        {
            Assert.Equal((fp)1, fp.One);
            Assert.Equal((fp)0, fp.Zero);
            Assert.Equal((fp)(-1), fp.MinusOne);

            Assert.Equal(1, (int)fp.One);
            Assert.Equal(0, (int)fp.Zero);
            Assert.Equal(-1, (int)fp.MinusOne);
        }

        [Fact]
        public static void FixedPointTest2()
        {
            Assert.Equal(fp.Epsilon * 2, fp.Epsilon + fp.Epsilon);
            Assert.Equal(0, fp.Epsilon - fp.Epsilon);
            Assert.Equal(fp.MinValue + fp.Epsilon + fp.Epsilon, fp.MinValue + 2 * fp.Epsilon);
            Assert.Equal(fp.MaxValue - fp.Epsilon - fp.Epsilon, fp.MaxValue - 2 * fp.Epsilon);
        }

        [Fact]
        public static void FixedPointTest3()
        {
            Assert.True(Math.Abs((double)Mathfp.PI - Math.PI) <= (double)fp.Epsilon);
            Assert.True(Math.Abs((double)Mathfp.E - Math.E) <= (double)fp.Epsilon);
        }
    }
}
