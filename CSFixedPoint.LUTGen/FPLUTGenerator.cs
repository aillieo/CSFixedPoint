using System;
using System.IO;
using System.Linq;

namespace AillieoUtils.CSFixedPoint.LUTGen
{
    public static class FPLUTGenerator
    {
        private static readonly int defaultLen = 65536;
        private static readonly string defaultPath = ".";
        private static readonly string SinLut = "FPSinLut.cs";
        private static readonly string TanLut = "FPTanLut.cs";

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

            GenSinLutText(Path.Join(outPath, SinLut), length);
            GenTanLutText(Path.Join(outPath, TanLut), length);

            //Console.ReadKey();
        }

        private static void GenSinLutText(string filepath, int length)
        {
            StreamWriter writer = File.CreateText(filepath);

            writer.WriteLine("namespace AillieoUtils.CSFixedPoint {");
            writer.WriteLine("public static class FPSinLut {");
            writer.WriteLine("public static fp[] table = new fp[] {");
            
            foreach (var i in Enumerable.Range(0, length + 1))
            {
                double p = (double)i / length;
                double v = p * Math.PI / 2;
                double sin = Math.Sin(v);
                writer.WriteLine($"fp.CreateWithRaw({fp.Nearest(sin).raw}), // {sin}");
            }

            writer.WriteLine("};");
            writer.WriteLine("}");
            writer.WriteLine("}");

            writer.Flush();
        }

        private static void GenTanLutText(string filepath, int length)
        {
            StreamWriter writer = File.CreateText(filepath);

            writer.WriteLine("namespace AillieoUtils.CSFixedPoint {");
            writer.WriteLine("public static class FPTanLut {");
            writer.WriteLine("public static fp[] table = new fp[] {");

            foreach (var i in Enumerable.Range(0, length + 1))
            {
                double p = (double)i / length;
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
                writer.WriteLine($"fp.CreateWithRaw({n.raw}), // {tan}");
            }

            writer.WriteLine("};");
            writer.WriteLine("}");
            writer.WriteLine("}");

            writer.Flush();
        }
    }
}
