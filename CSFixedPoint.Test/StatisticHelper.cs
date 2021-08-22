using System;
using System.Collections.Generic;
using System.Linq;

namespace AillieoUtils.CSFixedPoint.Test
{
    public static class StatisticHelper
    {
        public struct StatisticInfo
        {
            public readonly int count;
            public readonly fp max;
            public readonly fp min;
            public readonly double average;
            public readonly double variance;

            public StatisticInfo(int count, fp max, fp min, double a, double v)
            {
                this.count = count;
                this.max = max;
                this.min = min;
                this.average = a;
                this.variance = v;
            }

            public override string ToString()
            {
                return $"count={count}\nmax={max}\nmin={min}\navg={average}\nvar={variance}";
            }
        }

        public static StatisticInfo GetStatisticInfo(IEnumerable<fp> data)
        {
            fp max = fp.MinValue;
            fp min = fp.MaxValue;
            double sum = 0;
            int count = 0;
            foreach(var f in data)
            {
                max = Mathfp.Max(f, max);
                min = Mathfp.Min(f, min);
                double d = (double)f;
                sum += d;
                count++;
            }
            double avg = sum / count;

            double sqSum = 0;
            foreach (var f in data)
            {
                double dt = (double)f - avg;
                sqSum += (dt * dt);
            }
            double v = sqSum / (count - 1);
            return new StatisticInfo(count, max, min, avg, v);
        }

        public static Dictionary<int, int> GetCountInfo(IEnumerable<int> data)
        {
            Dictionary<int, int> counts = new Dictionary<int, int>();
            foreach (var v in data)
            {
                if (counts.TryGetValue(v, out int count))
                {
                    counts[v] = count + 1;
                }
                else
                {
                    counts.Add(v, 1);
                }
            }
            return counts;
        }

        public static string FormatCountInfo<T>(Dictionary<T, int> counts)
        {
            return string.Join("\n", counts.OrderBy(pair => pair.Value).Select(pair => $"{pair.Key}: {pair.Value}"));
        }
    }
}
