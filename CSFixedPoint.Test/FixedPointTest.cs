using System;
using Xunit;
namespace AillieoUtils.CSFixedPoint.Test
{
    public class FixedPointTest
    {
        private static readonly Random rand = new Random();
        private static readonly fp[] testSet = new fp[]
        {
            fp.MinValue, fp.MaxValue,
            fp.One, fp.MinusOne, fp.Zero,
            fp.Epsilon,
            (fp)(int.MinValue), (fp)(int.MaxValue),
            (fp)(short.MinValue), (fp)(short.MaxValue),

            (fp)10, (fp)1000, (fp)100000, (fp)10000000, (fp)1000000000,
            (fp)(-10), (fp)(-1000), (fp)(-100000), (fp)(-10000000), (fp)(-1000000000),
            
            (fp)9, (fp)999, (fp)99999, (fp)9999999, (fp)999999999,
            (fp)(-9), (fp)(-999), (fp)(-99999), (fp)(-9999999), (fp)(-999999999),
            
            fp.Nearest(0.1), fp.Nearest(0.001), fp.Nearest(0.00001), fp.Nearest(0.0000001), fp.Nearest(0.000000001),
            fp.Nearest(-0.1), fp.Nearest(-0.001), fp.Nearest(-0.00001), fp.Nearest(-0.0000001), fp.Nearest(-0.000000001),
            
            fp.Nearest(0.9), fp.Nearest(0.999), fp.Nearest(0.99999), fp.Nearest(0.9999999), fp.Nearest(0.999999999),
            fp.Nearest(-0.9), fp.Nearest(-0.999), fp.Nearest(-0.99999), fp.Nearest(-0.9999999), fp.Nearest(-0.999999999),
            
            (fp)rand.NextDouble(), 
            (fp)rand.NextDouble(), 
            (fp)rand.NextDouble(), 
            (fp)rand.NextDouble(), 
            (fp)rand.NextDouble(),

            (fp)rand.Next(int.MinValue, int.MaxValue), 
            (fp)rand.Next(int.MinValue, int.MaxValue), 
            (fp)rand.Next(int.MinValue, int.MaxValue), 
            (fp)rand.Next(int.MinValue, int.MaxValue), 
            (fp)rand.Next(int.MinValue, int.MaxValue), 
        };

        private static void AssertApproximatelyEqual(fp f0, fp f1)
        {
            Assert.True(Mathfp.Abs(f0 - f1) <= fp.Epsilon);
        }
        
        private static void AssertApproximatelyEqual(double f0, double f1)
        {
            Assert.True(Math.Abs(f0 - f1) <= (double)fp.Epsilon, $"{f0}, {f1}"); 
        }

        [Fact]
        public static void FixedPointTest01()
        {
            Assert.Equal((fp)1, fp.One);
            Assert.Equal((fp)0, fp.Zero);
            Assert.Equal((fp)(-1), fp.MinusOne);

            Assert.Equal(1, (int)fp.One);
            Assert.Equal(0, (int)fp.Zero);
            Assert.Equal(-1, (int)fp.MinusOne);
            
            Assert.Equal(int.MaxValue, (int)(fp)int.MaxValue);
            Assert.Equal(int.MinValue, (int)(fp)int.MinValue);
        }

        [Fact]
        public static void FixedPointTest02()
        {
            Assert.Equal(fp.Epsilon * 2, fp.Epsilon + fp.Epsilon);
            Assert.Equal(0, fp.Epsilon - fp.Epsilon);
            Assert.Equal(fp.MinValue + fp.Epsilon + fp.Epsilon, fp.MinValue + 2 * fp.Epsilon);
            Assert.Equal(fp.MaxValue - fp.Epsilon - fp.Epsilon, fp.MaxValue - 2 * fp.Epsilon);
        }

        [Fact]
        public static void FixedPointTest03()
        {
            AssertApproximatelyEqual((double)Mathfp.PI, Math.PI);
            AssertApproximatelyEqual((double)Mathfp.E, Math.E);
        }
        
        [Fact]
        public static void FixedPointTest04()
        {
            foreach (var f1 in testSet)
            {
                foreach (var f2 in testSet)
                {
                    Assert.Equal(f1 > f2, (double)f1 > (double)f2);
                    Assert.Equal(f1 < f2, (double)f1 < (double)f2);
                    Assert.Equal(f1 >= f2, (double)f1 >= (double)f2);
                    Assert.Equal(f1 <= f2, (double)f1 <= (double)f2);
                }   
            }
        }
        
        [Fact]
        public static void FixedPointTest05()
        {
            foreach (var f1 in testSet)
            {
                foreach (var f2 in testSet)
                {
                    if (f1 == fp.MaxValue && f2 > fp.Zero) { continue; }
                    if (f1 > fp.Zero && f2 == fp.MaxValue) { continue; }
                    if (f1 == fp.MinValue && f2 < fp.Zero) { continue; }
                    if (f1 < fp.Zero && f2 == fp.MinValue) { continue; }
                    if (f1 == int.MaxValue && f2 >= fp.One) { continue; }
                    if (f1 >= fp.One && f2 == int.MaxValue) { continue; }
                    if (f1 == int.MinValue && f2 <= fp.MinusOne) { continue; }
                    if (f1 <= fp.MinusOne && f2 == int.MinValue) { continue; }
                    
                    AssertApproximatelyEqual((double)(f1 + f2), (double)(f1) + (double)(f2));
                }   
            }
        }
        
        [Fact]
        public static void FixedPointTest06()
        {
            foreach (var f1 in testSet)
            {
                foreach (var f2 in testSet)
                {
                    if (f1 == fp.MaxValue && f2 < fp.Zero) { continue; }
                    if (f1 > fp.MinusOne && f2 == fp.MinValue) { continue; }
                    if (f1 == fp.MinValue && f2 > fp.Zero) { continue; }
                    if (f1 < fp.Zero && f2 == fp.MaxValue) { continue; }
                    if (f1 == int.MaxValue && f2 <= fp.One) { continue; }
                    if (f1 >= fp.Zero && f2 == int.MinValue) { continue; }
                    if (f1 == int.MinValue && f2 >= fp.MinusOne) { continue; }
                    if (f1 <= fp.MinusOne && f2 == int.MaxValue) { continue; }
                    
                    // Console.WriteLine($"{f1} {f2}");
                    AssertApproximatelyEqual((double)(f1 - f2), (double)(f1) - (double)(f2));
                }   
            }
        }
        
        [Fact]
        public static void FixedPointTest07()
        {
            fp safeFrac = fp.Epsilon * 100000;
            foreach (var f1 in testSet)
            {
                foreach (var f2 in testSet)
                {
                    if (Mathfp.Abs(f1) >= int.MaxValue && Mathfp.Abs(f2) > fp.One) { continue; }
                    if (Mathfp.Abs(f1) > fp.One && Mathfp.Abs(f2) >= int.MaxValue) { continue; }
                    if (Mathfp.Abs(f1) <= safeFrac  && Mathfp.Abs(f2) <= safeFrac) { continue; }

                    // Console.WriteLine($"{f1} {f2}");
                    AssertApproximatelyEqual((double)(f1 * f2), (double)(f1) * (double)(f2));
                }   
            }
        }
        
        [Fact]
        public static void FixedPointTest08()
        {
            foreach (var f1 in testSet)
            {
                foreach (var f2 in testSet)
                {
                    // if (f1 == fp.MaxValue && f2 < fp.Zero) { continue; }
                    // if (f1 > fp.MinusOne && f2 == fp.MinValue) { continue; }
                    // if (f1 == fp.MinValue && f2 > fp.Zero) { continue; }
                    // if (f1 < fp.Zero && f2 == fp.MaxValue) { continue; }
                    // if (f1 == int.MaxValue && f2 <= fp.One) { continue; }
                    // if (f1 >= fp.Zero && f2 == int.MinValue) { continue; }
                    // if (f1 == int.MinValue && f2 >= fp.MinusOne) { continue; }
                    // if (f1 <= fp.MinusOne && f2 == int.MaxValue) { continue; }
                    
                    // AssertApproximatelyEqual((double)(f1 / f2), (double)(f1) / (double)(f2));
                }   
            }
        }
        
        [Fact]
        public static void FixedPointTest09()
        {
            foreach (var f1 in testSet)
            {
                if (f1 < fp.Zero)
                {
                    continue;
                }
                
                //AssertApproximatelyEqual(Math.Sqrt((double)f1), (double)Mathfp.Sqrt(f1));
            }
        }
    }
}
