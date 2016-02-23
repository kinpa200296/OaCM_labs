using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace MathBase.Utility
{
    public static class MatrixIoManager
    {
        public static DoubleMatrix LoadSquareMatrix(StreamReader reader)
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
            return LoadSquareMatrix(reader, n);
        }

        public static DoubleMatrix LoadMatrix(StreamReader reader)
        {
            var s = reader.ReadLine();
            if (s == null)
            {
                throw new FormatException("File format doesn't match");
            }
            int m;
            if (!int.TryParse(s, out m))
            {
                throw new FormatException("File format doesn't match");
            }
            s = reader.ReadLine();
            if (s == null)
            {
                throw new FormatException("File format doesn't match");
            }
            int n;
            if (!int.TryParse(s, out n))
            {
                throw new FormatException("File format doesn't match");
            }
            return LoadMatrix(reader, m, n);
        }

        public static DoubleMatrix LoadSquareMatrix(StreamReader reader, int n)
        {
            return LoadMatrix(reader, n, n);
        }

        public static DoubleMatrix LoadMatrix(StreamReader reader, int m, int n)
        {
            var matrix = new DoubleMatrix(m, n);
            for (var i = 0; i < m; i++)
            {
                var s = reader.ReadLine();
                if (s == null)
                {
                    throw new FormatException("File format doesn't match");
                }
                var c = s.Split(',').Where(str => !string.IsNullOrWhiteSpace(str)).ToArray();
                if (c.Length != n)
                {
                    throw new FormatException("File format doesn't match");
                }
                for (var j = 0; j < n; j++)
                {
                    double temp;
                    if (!double.TryParse(c[j], NumberStyles.Float, CultureInfo.InvariantCulture, out temp))
                    {
                        throw new FormatException("File format doesn't match");
                    }
                    matrix[i, j] = temp;
                }
            }
            return matrix;
        }

        public static void SaveMatrix(StreamWriter writer, DoubleMatrix matrix)
        {
            for (int i = 0; i < matrix.RowCount; i++)
            {
                foreach (var val in matrix.GetHorizontalVector(i))
                {
                    writer.Write("{0},", val.ToString("F6", CultureInfo.InvariantCulture));
                }
                writer.WriteLine();
            }
        }

        public static void SaveMatrixStandalone(StreamWriter writer, DoubleMatrix matrix)
        {
            writer.WriteLine("{0}", matrix.RowCount);
            writer.WriteLine("{0}", matrix.ColumnCount);
            for (int i = 0; i < matrix.RowCount; i++)
            {
                foreach (var val in matrix.GetHorizontalVector(i))
                {
                    writer.Write("{0},", val.ToString("F6", CultureInfo.InvariantCulture));
                }
                writer.WriteLine();
            }
        }

        public static void SaveSquareMatrix(StreamWriter writer, DoubleMatrix matrix)
        {
            for (int i = 0; i < matrix.RowCount; i++)
            {
                foreach (var val in matrix.GetHorizontalVector(i))
                {
                    writer.Write("{0},", val.ToString("F6", CultureInfo.InvariantCulture));
                }
                writer.WriteLine();
            }
        }

        public static void SaveSquareMatrixStandalone(StreamWriter writer, DoubleMatrix matrix)
        {
            writer.WriteLine("{0}", matrix.RowCount);
            for (int i = 0; i < matrix.RowCount; i++)
            {
                foreach (var val in matrix.GetHorizontalVector(i))
                {
                    writer.Write("{0},", val.ToString("F6", CultureInfo.InvariantCulture));
                }
                writer.WriteLine();
            }
        }
    }
}
