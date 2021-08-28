using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AillieoUtils.CSFixedPoint.Test
{
    public class FixedPointTest
    {
        public FixedPointTest(ITestOutputHelper outputHelper)
        {
            testOutputHelper = outputHelper;
        }

        private readonly ITestOutputHelper testOutputHelper;
        private static readonly Random rand = new Random(453500);
        private static readonly fp[] testSet = new fp[]
        {
            fp.MinValue, fp.MaxValue,
            fp.One, fp.MinusOne, fp.Zero,

            fp.Epsilon,
            - fp.Epsilon,
            fp.One + fp.Epsilon,
            fp.One - fp.Epsilon,

            fp.EpsilonSqrt,

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

            fp.Nearest(rand.NextDouble()),
            fp.Nearest(rand.NextDouble()),
            fp.Nearest(rand.NextDouble()),
            fp.Nearest(rand.NextDouble()),
            fp.Nearest(rand.NextDouble()),

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

        private static void AssertApproximatelyEqual(double f0, double f1, double error)
        {
            Assert.True(Math.Abs(f0 - f1) <= Math.Abs(error), $"{f0}, {f1}");
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
                    if ((long)f1 + (long)f2 > (long)int.MaxValue) { continue; }
                    if ((long)f1 + (long)f2 < (long)int.MinValue) { continue; }

                    AssertApproximatelyEqual((double)(f1 + f2), (double)(f1) + (double)(f2));
                }   
            }
        }
        
        [Fact]
        public void FixedPointTest06()
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
                    if ((long)f1 - (long)f2 > (long)int.MaxValue) { continue; }
                    if ((long)f1 - (long)f2 < (long)int.MinValue) { continue; }

                    //testOutputHelper.WriteLine($"{f1},{f2},{f1 - f2}");
                    AssertApproximatelyEqual((double)(f1 - f2), (double)(f1) - (double)(f2));
                }   
            }
        }
        
        [Fact]
        public void FixedPointTest07()
        {
            AssertApproximatelyEqual(fp.Epsilon, fp.EpsilonSqrt * fp.EpsilonSqrt);

            foreach (var f1 in testSet)
            {
                foreach (var f2 in testSet)
                {
                    if (Mathfp.Abs(f1) >= int.MaxValue && Mathfp.Abs(f2) > fp.One) { continue; }
                    if (Mathfp.Abs(f1) > fp.One && Mathfp.Abs(f2) >= int.MaxValue) { continue; }
                    if (f1 <= int.MinValue || f2 <= int.MinValue) { continue; }
                    if (f1 == fp.MaxValue || f2 == fp.MaxValue) { continue; }
                    if (f1 == fp.MinValue || f2 == fp.MinValue) { continue; }
                    if ((long)f1 * (long)f2 > (long)(int.MaxValue)) { continue; }
                    if ((long)f1 * (long)f2 < (long)(int.MinValue)) { continue; }
                    double dm = (double)f1 * (double)f2;
                    double de = (double)fp.Epsilon;
                    if (dm < de && dm > -de) { continue; }

                    AssertApproximatelyEqual((double)(f1 * f2), (double)(f1) * (double)(f2), (double)fp.Epsilon * 10);
                }
            }
        }
        
        [Fact]
        public void FixedPointTest08()
        {
            //testOutputHelper.WriteLine($"{(fp)2},{(fp)3},{(fp)2 / (fp)3}");
            //testOutputHelper.WriteLine($"{(fp)3},{(fp)2},{(fp)3 / (fp)2}");

            foreach (var f1 in testSet)
            {
                foreach (var f2 in testSet)
                {
                    if (f2 == fp.Zero) { continue; }
                    if (Mathfp.Abs(f1) >= int.MaxValue && Mathfp.Abs(f2) < fp.One) { continue; }
                    if (f1 <= int.MinValue || f2 <= int.MinValue) { continue; }
                    if (f1 == fp.MaxValue || f2 == fp.MaxValue) { continue; }
                    if (f1 == fp.MinValue || f2 == fp.MinValue) { continue; }
                    if ((double)f1 / (double)f2 > (double)(int.MaxValue)) { continue; }
                    if ((double)f1 / (double)f2 < (double)(int.MinValue)) { continue; }
                    double dm = (double)f1 * (double)f2;
                    double de = (double)fp.Epsilon;
                    if (dm < de && dm > -de) { continue; }

                    //testOutputHelper.WriteLine($"{f1},{f2},{f1 / f2}");
                    //testOutputHelper.WriteLine($"[{count++}]  {(double)(f1 / f2) - (double)(f1) / (double)(f2)}");
                    AssertApproximatelyEqual((double)(f1 / f2), (double)(f1) / (double)(f2), (double)fp.Epsilon * 10);
                }
            }
        }
        
        [Fact]
        public void FixedPointTest09()
        {
            testOutputHelper.WriteLine($"{Math.Sqrt((double)4)}, {(double)Mathfp.Sqrt((fp)4)}");
            testOutputHelper.WriteLine($"{Math.Sqrt((double)0.04)}, {(double)Mathfp.Sqrt(fp.Nearest(0.04))}");
            testOutputHelper.WriteLine($"{Math.Sqrt((double)3600)}, {(double)Mathfp.Sqrt((fp)3600)}");

            foreach (var f1 in testSet)
            {
                if (f1 < fp.Zero)
                {
                    continue;
                }

                testOutputHelper.WriteLine($"{f1}, err={Math.Sqrt((double)f1) -(double)Mathfp.Sqrt(f1)}");
                AssertApproximatelyEqual(Math.Sqrt((double)f1), (double)Mathfp.Sqrt(f1), 1e-4);
            }
        }

        [Fact]
        public void FixedPointTest10()
        {
            foreach (var f in testSet)
            {
                if (f >= int.MaxValue) { continue; }

                testOutputHelper.WriteLine($"{f}: {Mathfp.Floor(f)},{Mathfp.FloorToInt(f)},{Mathfp.Ceil(f)},{Mathfp.CeilToInt(f)}");

                AssertApproximatelyEqual(Math.Ceiling((double)f), (double)Mathfp.Ceil(f), (double)fp.Epsilon * 10);
                AssertApproximatelyEqual(Math.Floor((double)f), (double)Mathfp.Floor(f), (double)fp.Epsilon * 10);
                AssertApproximatelyEqual(Math.Ceiling((double)f), (double)Mathfp.CeilToInt(f), (double)fp.Epsilon * 10);
                AssertApproximatelyEqual(Math.Floor((double)f), (double)Mathfp.FloorToInt(f), (double)fp.Epsilon * 10);
            }
        }

        [Fact]
        public void FixedPointTest11()
        {
            for (double d = - Math.PI * 2; d < Math.PI * 2; d += 0.001)
            {
                fp f = fp.Nearest(d);
                testOutputHelper.WriteLine($"{d / (Math.PI / 2)}: {f}, err={Math.Sin((double)f) - (double)Mathfp.Sin(f)}");
                AssertApproximatelyEqual(Math.Sin((double)f), (double)Mathfp.Sin(f), 1e-4);
                AssertApproximatelyEqual(Math.Cos((double)f), (double)Mathfp.Cos(f), 1e-4);
            }
        }

        [Fact]
        public void FixedPointTest12()
        {
            for (double d = -Math.PI * 2; d < Math.PI * 2; d += 0.001)
            {
                fp f = fp.Nearest(d);
                testOutputHelper.WriteLine($"{d / (Math.PI / 2)}: {f}, err={Math.Tan((double)f) - (double)Mathfp.Tan(f)}");
                AssertApproximatelyEqual(Math.Tan((double)f), (double)Mathfp.Tan(f), Math.Max(1e-4, Math.Abs(Math.Tan((double)f)) * 1e-1));
            }
        }

        [Fact]
        public void FixedPointTest13()
        {
            //for (int i = 1; i < 9; ++i)
            //{
            //    long p = (long)Math.Pow(2, i);
            //    testOutputHelper.WriteLine($"{i},{p}, {p - Math.Pow(2, (double)Mathfp.Log2(p))}");
            //}

            foreach (var f in testSet)
            {
                if (f <= fp.Zero) { continue; }

                testOutputHelper.WriteLine($"{f}, err={Math.Log((double)f, 2) - (double)Mathfp.Log2(f)}");
                AssertApproximatelyEqual(Math.Log((double)f, 2), (double)Mathfp.Log2(f), (double)fp.Epsilon * 100);

                testOutputHelper.WriteLine($"{f}, err={Math.Log((double)f) - (double)Mathfp.Log(f)}");
                AssertApproximatelyEqual(Math.Log((double)f), (double)Mathfp.Log(f), (double)fp.Epsilon * 100);

                testOutputHelper.WriteLine($"{f}, err={Math.Log10((double)f) - (double)Mathfp.Log10(f)}");
                AssertApproximatelyEqual(Math.Log10((double)f), (double)Mathfp.Log10(f), (double)fp.Epsilon * 100);

                foreach (var fbase in new fp[] { (fp)3, (fp)5, (fp)100, fp.Nearest(0.1), fp.Nearest(2.5) })
                {
                    testOutputHelper.WriteLine($"{f}, err={Math.Log((double)f, (double)fbase) - (double)Mathfp.Log(f, fbase)}");
                    AssertApproximatelyEqual(Math.Log((double)f, (double)fbase), (double)Mathfp.Log(f, fbase), (double)fp.Epsilon * 1000);
                }
            }
        }

        [Fact]
        public void FixedPointTest14()
        {
            Randomfp rand = new Randomfp((int)DateTime.Now.Ticks);
            //Random r = new Random();

            for (int t = 0; t < 5; ++t)
            {
                IEnumerable<int> list1 = Enumerable.Range(0, 100000).Select(i => rand.NextInt(-100, 100));
                //IEnumerable<int> list2 = Enumerable.Range(0, 100000).Select(i => r.Next(-100, 100));

                var info1 = StatisticHelper.GetStatisticInfo(list1.Select(i => (fp)i));
                //var info2 = StatisticHelper.GetStatisticInfo(list2.Select(i => (fp)i));
                testOutputHelper.WriteLine(info1.ToString());
                //testOutputHelper.WriteLine(info2.ToString());

                AssertApproximatelyEqual(info1.average, 0, 1);

                //var counts = StatisticHelper.GetCountInfo(list1);
                //testOutputHelper.WriteLine(StatisticHelper.FormatCountInfo(counts));

                testOutputHelper.WriteLine("------------------------------------------------------------------");
            }

            for (int t = 0; t < 5; ++t)
            {
                IEnumerable<int> list1 = Enumerable.Range(0, 100000).Select(i => rand.NextInt(0, 100));
                //IEnumerable<int> list2 = Enumerable.Range(0, 100000).Select(i => r.Next(0, 100));

                var info1 = StatisticHelper.GetStatisticInfo(list1.Select(i => (fp)i));
                //var info2 = StatisticHelper.GetStatisticInfo(list2.Select(i => (fp)i));
                testOutputHelper.WriteLine(info1.ToString());
                //testOutputHelper.WriteLine(info2.ToString());

                AssertApproximatelyEqual(info1.average, 50, 1);

                //var counts = StatisticHelper.GetCountInfo(list1);
                //testOutputHelper.WriteLine(StatisticHelper.FormatCountInfo(counts));

                testOutputHelper.WriteLine("------------------------------------------------------------------");
            }

            for (int t = 0; t < 5; ++t)
            {
                IEnumerable<fp> list2 = Enumerable.Range(0, 100000).Select(i => rand.Nextfp(fp.MinusOne * 100, fp.One * 100));

                var info = StatisticHelper.GetStatisticInfo(list2);
                testOutputHelper.WriteLine(info.ToString());

                AssertApproximatelyEqual(info.average, 0, 1);

                testOutputHelper.WriteLine("------------------------------------------------------------------");
            }

            for (int t = 0; t < 5; ++t)
            {
                IEnumerable<fp> list2 = Enumerable.Range(0, 100000).Select(i => rand.Nextfp(fp.MinValue, fp.MaxValue));

                var info = StatisticHelper.GetStatisticInfo(list2);
                testOutputHelper.WriteLine(info.ToString());

                // AssertApproximatelyEqual(info.average, 0, 1);

                testOutputHelper.WriteLine("------------------------------------------------------------------");
            }

            //Assert.True(false);
        }
    }
}
