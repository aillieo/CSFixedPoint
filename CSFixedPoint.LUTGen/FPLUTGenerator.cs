using System;
using System.IO;
using System.Linq;

namespace AillieoUtils.CSFixedPoint.LUTGen
{
    public static class FPLUTGenerator
    {
        private static readonly int defaultLen = 65536;
        private static readonly string defaultPath = ".";
        private static readonly int lenPerFile = 1024;

        private static readonly string FPSinLut = "FPSinLut";
        private static readonly string FPTanLut = "FPTanLut";

        public static void Main(string[] args)
        {
            string outPath = defaultPath;
            int length = defaultLen;
            if (args != null && args.Length > 0)
            {
                outPath = args[0];
            }

            if (args != null && args.Length > 1)
            {
                if (int.TryParse(args[1], out int arg1))
                {
                    length = arg1;
                }
            }

            GenSinLutText(outPath, length);
            GenTanLutText(outPath, length);

            //Console.ReadKey();
        }

        private static void GenSinLutText(string path, int length)
        {
            StreamWriter writer = File.CreateText($"{Path.Join(path, FPSinLut)}.cs");
            writer.WriteLine("namespace AillieoUtils.CSFixedPoint {");
            writer.WriteLine($"    internal static class {FPSinLut} {{");
            writer.WriteLine($"        private static readonly fp[][] tables = new fp[][] {{");

            int start = 0;
            double step = 1d / length;
            while (true)
            {
                int count = Math.Min(lenPerFile, length + 1 - start);
                if (count <= 0)
                {
                    break;
                }
                writer.WriteLine($"            {FPSinLut}_{start}.table,");
                GenSinLutTextOneFile(path, start, count, step);
                start += count;
            }

            writer.WriteLine($"        }};");
            writer.WriteLine($"        internal static readonly int length = {length};");
            writer.WriteLine($"        internal static readonly fp oneOverStep = fp.CreateWithRaw({fp.Nearest(length / (Math.PI / 2)).raw}); // (length/(PI/2))");
            writer.WriteLine($"        internal static readonly int lengthPerFile = {lenPerFile};");
            writer.WriteLine($"        internal static fp Get(int index) {{ return tables[index / lengthPerFile][index % lengthPerFile]; }}");
            writer.WriteLine($"    }}");
            writer.WriteLine("}");

            writer.Flush();
        }

        private static void GenSinLutTextOneFile(string path, int start, int count, double step)
        {
            string dir = Path.Join(path, FPSinLut);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            StreamWriter writer = File.CreateText($"{Path.Join(dir, $"{FPSinLut}_{start}")}.cs");

            writer.WriteLine($"namespace AillieoUtils.CSFixedPoint {{");
            writer.WriteLine($"    internal static class {FPSinLut}_{start} {{");
            writer.WriteLine($"        internal static fp[] table = new fp[] {{");

            foreach (var i in Enumerable.Range(start, count))
            {
                double p = i * step;
                double v = p * Math.PI / 2;
                double sin = Math.Sin(v);
                writer.WriteLine($"            fp.CreateWithRaw({fp.Nearest(sin).raw}), // {sin}");
            }

            writer.WriteLine($"        }};");
            writer.WriteLine($"    }}");
            writer.WriteLine("}");

            writer.Flush();
        }

        private static void GenTanLutText(string path, int length)
        {
            StreamWriter writer = File.CreateText($"{Path.Join(path, FPTanLut)}.cs");
            writer.WriteLine("namespace AillieoUtils.CSFixedPoint {");
            writer.WriteLine($"    internal static class {FPTanLut} {{");
            writer.WriteLine($"        private static readonly fp[][] tables = new fp[][] {{");

            int start = 0;
            double step = 1d / length;
            while (true)
            {
                int count = Math.Min(lenPerFile, length + 1 - start);
                if (count <= 0)
                {
                    break;
                }
                writer.WriteLine($"            {FPTanLut}_{start}.table,");
                GenTanLutTextOneFile(path, start, count, step);
                start += count;
            }

            writer.WriteLine($"        }};");
            writer.WriteLine($"        internal static readonly int length = {length};");
            writer.WriteLine($"        internal static readonly fp oneOverStep = fp.CreateWithRaw({fp.Nearest(length / (Math.PI / 2)).raw}); // (length/(PI/2))");
            writer.WriteLine($"        internal static readonly int lengthPerFile = {lenPerFile};");
            writer.WriteLine($"        internal static fp Get(int index) {{ return tables[index / lengthPerFile][index % lengthPerFile]; }}");
            writer.WriteLine($"    }}");
            writer.WriteLine("}");

            writer.Flush();
        }

        private static void GenTanLutTextOneFile(string path, int start, int count, double step)
        {
            string dir = Path.Join(path, FPTanLut);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            StreamWriter writer = File.CreateText($"{Path.Join(dir, $"{FPTanLut}_{start}")}.cs");

            writer.WriteLine($"namespace AillieoUtils.CSFixedPoint {{");
            writer.WriteLine($"    internal static class FPTanLut_{start} {{");
            writer.WriteLine($"        internal static fp[] table = new fp[] {{");

            foreach (var i in Enumerable.Range(start, count))
            {
                double p = i * step;
                double v = p * Math.PI / 2;
                double tan = Math.Tan(v);
                fp n;
                if (tan >= (double)fp.MaxValue || tan < 0)
                {
                    tan = (double)fp.MaxValue;
                    n = fp.MaxValue;
                }
                else
                {
                    n = fp.Nearest(tan);
                }
                writer.WriteLine($"            fp.CreateWithRaw({n.raw}), // {tan}");
            }

            writer.WriteLine($"        }};");
            writer.WriteLine($"    }}");
            writer.WriteLine("}");

            writer.Flush();
        }
    }
}
