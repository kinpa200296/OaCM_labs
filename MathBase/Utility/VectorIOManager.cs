using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace MathBase.Utility
{
    public static class VectorIoManager
    {
        public static DoubleVector LoadVector(StreamReader reader)
        {
            var s = reader.ReadLine();
            if (s == null)
            {
                throw new FormatException("File format doesn't match");
            }
            int n;
            if (!int.TryParse(s, out n))
            {
                throw new FormatException("File format doesn't match");
            }
            return LoadVector(reader, n);
        }

        public static DoubleVector LoadVector(StreamReader reader, int n)
        {
            var vector = new DoubleVector(n);
            if (n == 0)
            {
                return vector;
            }
            var s = reader.ReadLine();
            if (s == null)
            {
                throw new FormatException("File format doesn't match");
            }
            var c1 = s.Split(',').Where(str => !string.IsNullOrWhiteSpace(str)).ToArray();
            if (c1.Length != n)
            {
                throw new FormatException("File format doesn't match");
            }
            for (var j = 0; j < n; j++)
            {
                double temp;
                if (!double.TryParse(c1[j], NumberStyles.Float, CultureInfo.InvariantCulture, out temp))
                {
                    throw new FormatException("File format doesn't match");
                }
                vector[j] = temp;
            }
            return vector;
        }

        public static void SaveVector(StreamWriter writer, DoubleVector vector)
        {
            foreach (var val in vector)
            {
                writer.Write("{0},", val.ToString("F6", CultureInfo.InvariantCulture));
            }
            writer.WriteLine();
        }

        public static void SaveVectorStandalone(StreamWriter writer, DoubleVector vector)
        {
            writer.WriteLine("{0}", vector.Length);
            foreach (var val in vector)
            {
                writer.Write("{0},", val.ToString("F6", CultureInfo.InvariantCulture));
            }
            writer.WriteLine();
        }
    }
}
