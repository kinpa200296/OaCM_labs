using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace MathBase.Utility
{
    public static class VectorIoManager
    {
        public static DoubleVector LoadVector(StreamReader reader, int n)
        {
            var vector = new DoubleVector(n);
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
    }
}
