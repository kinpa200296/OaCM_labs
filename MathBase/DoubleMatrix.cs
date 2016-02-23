using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MathBase
{
    public class DoubleMatrix : Matrix<double>, IEnumerable<DoubleVector>
    {
        public DoubleMatrix()
        {
        }

        public DoubleMatrix(int rowCount, int columnCount)
        {
            _data = new double[rowCount, columnCount];
        }

        public DoubleMatrix(double[,] data)
        {
            _data = new double[data.GetLength(0), data.GetLength(1)];
            for (var i = 0; i < data.GetLength(0); i++)
                for (var j = 0; j < data.GetLength(1); j++)
                {
                    _data[i, j] = data[i, j];
                }
        }

        public DoubleMatrix(IEnumerable<DoubleVector> data)
        {
            var vectors = data.ToArray();
            _data = new double[vectors.Length, vectors[0].Length];
            for (var i = 0; i < vectors.Length; i++)
            {
                this[i] = vectors[i];
            }
        }

        public static DoubleMatrix One(int size)
        {
            var res = new DoubleMatrix(size, size);
            for (var i = 0; i < size; i++)
            {
                res[i, i] = 1;
            }
            return res;
        }

        public DoubleVector this[int rowNumber]
        {
            get { return GetVerticalVector(rowNumber); }
            set { SetVerticalVector(rowNumber, value); }
        }

        public void SetHorizontalVector(int rowNumber, DoubleVector vector)
        {
            if (rowNumber < 0 || rowNumber >= RowCount)
            {
                throw new IndexOutOfRangeException("Matrix row number out of range");
            }
            if (vector.Length != ColumnCount)
            {
                throw new ArgumentException("Matrix and vector sizes do not match");
            }
            for (var i = 0; i < ColumnCount; i++)
            {
                _data[rowNumber, i] = vector[i];
            }
        }

        public void SetVerticalVector(int columnNumber, DoubleVector vector)
        {
            if (columnNumber < 0 || columnNumber >= ColumnCount)
            {
                throw new IndexOutOfRangeException("Matrix column number out of range");
            }
            if (vector.Length != RowCount)
            {
                throw new ArgumentException("Matrix and vector sizes do not match");
            }
            for (var i = 0; i < ColumnCount; i++)
            {
                _data[i, columnNumber] = vector[i];
            }
        }

        public DoubleVector GetHorizontalVector(int rowNumder)
        {
            if (rowNumder < 0 || rowNumder >= RowCount)
            {
                throw new IndexOutOfRangeException("Matrix row number out of range");
            }
            var vector = new DoubleVector(ColumnCount);
            for (var i = 0; i < ColumnCount; i++)
            {
                vector[i] = _data[rowNumder, i];
            }
            return vector;
        }

        public DoubleVector GetVerticalVector(int columnNumder)
        {
            if (columnNumder < 0 || columnNumder >= ColumnCount)
            {
                throw new IndexOutOfRangeException("Matrix column number out of range");
            }
            var vector = new DoubleVector(RowCount);
            for (var i = 0; i < RowCount; i++)
            {
                vector[i] = _data[i, columnNumder];
            }
            return vector;
        }

        public static DoubleMatrix operator +(DoubleMatrix matrix1, DoubleMatrix matrix2)
        {
            if (matrix1.RowCount != matrix2.RowCount || matrix1.ColumnCount != matrix2.ColumnCount)
            {
                throw new ArgumentException("Matrix size doesn't match. Cannot sum.");
            }
            var matrix = new DoubleMatrix(matrix1.RowCount, matrix1.ColumnCount);
            for (var i = 0; i < matrix.RowCount; i++)
                for (var j = 0; j < matrix.ColumnCount; j++)
                {
                    matrix[i, j] = matrix1[i, j] + matrix2[i, j];
                }
            return matrix;
        }

        public static DoubleMatrix operator -(DoubleMatrix matrix1, DoubleMatrix matrix2)
        {
            if (matrix1.RowCount != matrix2.RowCount || matrix1.ColumnCount != matrix2.ColumnCount)
            {
                throw new ArgumentException("Matrix size doesn't match. Cannot sum.");
            }
            var matrix = new DoubleMatrix(matrix1.RowCount, matrix1.ColumnCount);
            for (var i = 0; i < matrix.RowCount; i++)
                for (var j = 0; j < matrix.ColumnCount; j++)
                {
                    matrix[i, j] = matrix1[i, j] - matrix2[i, j];
                }
            return matrix;
        }

        public static DoubleMatrix operator *(DoubleMatrix matrix1, DoubleMatrix matrix2)
        {
            if (matrix1.ColumnCount != matrix2.RowCount)
            {
                throw new ArgumentException("Matrices are inconsistent. Cannot multiply.");
            }
            var matrix = new DoubleMatrix(matrix1.RowCount, matrix2.ColumnCount);
            for (var i = 0; i < matrix.RowCount; i++)
                for (var j = 0; j < matrix.ColumnCount; j++)
                {
                    matrix[i, j] = matrix1.GetHorizontalVector(i) * matrix2.GetVerticalVector(j);
                }
            return matrix;
        }

        public static DoubleVector operator *(DoubleMatrix matrix, DoubleVector vector)
        {
            if (matrix.ColumnCount != vector.Length)
            {
                throw new ArgumentException("Matrix and vector are inconsistent. Cannot multiply.");
            }
            var res = new DoubleVector(vector.Length);
            for (int i = 0; i < vector.Length; i++)
            {
                res[i] = matrix.GetHorizontalVector(i) * vector;
            }
            return res;
        }

        public static DoubleVector operator *(DoubleVector vector, DoubleMatrix matrix)
        {
            if (matrix.RowCount != vector.Length)
            {
                throw new ArgumentException("Matrix and vector are inconsistent. Cannot multiply.");
            }
            var res = new DoubleVector(vector.Length);
            for (int i = 0; i < vector.Length; i++)
            {
                res[i] = vector * matrix.GetVerticalVector(i);
            }
            return res;
        }

        public static DoubleMatrix operator +(DoubleMatrix matrix, double val)
        {
            var result = new DoubleMatrix(matrix.RowCount, matrix.ColumnCount);
            for (var i = 0; i < result.RowCount; i++)
                for (var j = 0; j < result.ColumnCount; j++)
                {
                    result[i, j] = matrix[i, j] + val;
                }
            return result;
        }

        public static DoubleMatrix operator +(double val, DoubleMatrix matrix)
        {
            return matrix + val;
        }

        public static DoubleMatrix operator -(DoubleMatrix matrix, double val)
        {
            var result = new DoubleMatrix(matrix.RowCount, matrix.ColumnCount);
            for (var i = 0; i < result.RowCount; i++)
                for (var j = 0; j < result.ColumnCount; j++)
                {
                    result[i, j] = matrix[i, j] - val;
                }
            return result;
        }

        public static DoubleMatrix operator -(double val, DoubleMatrix matrix)
        {
            var result = new DoubleMatrix(matrix.RowCount, matrix.ColumnCount);
            for (var i = 0; i < result.RowCount; i++)
                for (var j = 0; j < result.ColumnCount; j++)
                {
                    result[i, j] = val - matrix[i, j];
                }
            return result;
        }

        public static DoubleMatrix operator *(DoubleMatrix matrix, double val)
        {
            var result = new DoubleMatrix(matrix.RowCount, matrix.ColumnCount);
            for (var i = 0; i < result.RowCount; i++)
                for (var j = 0; j < result.ColumnCount; j++)
                {
                    result[i, j] = matrix[i, j] * val;
                }
            return result;
        }

        public static DoubleMatrix operator *(double val, DoubleMatrix matrix)
        {
            return matrix * val;
        }

        public static DoubleMatrix operator /(DoubleMatrix matrix, double val)
        {
            var result = new DoubleMatrix(matrix.RowCount, matrix.ColumnCount);
            for (var i = 0; i < result.RowCount; i++)
                for (var j = 0; j < result.ColumnCount; j++)
                {
                    result[i, j] = matrix[i, j] / val;
                }
            return result;
        }

        public IEnumerator<DoubleVector> GetEnumerator()
        {
            return new VerticalVectorEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class VerticalVectorEnumerator : IEnumerator<DoubleVector>
        {
            private int _index = -1;
            private DoubleMatrix _matrix;

            public VerticalVectorEnumerator(DoubleMatrix matrix)
            {
                _matrix = matrix;
            }

            public void Dispose() { }

            public bool MoveNext()
            {
                _index++;
                return _index < _matrix.RowCount;
            }

            public void Reset()
            {
                _index = -1;
            }

            public DoubleVector Current { get { return _matrix.GetVerticalVector(_index); } }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}
