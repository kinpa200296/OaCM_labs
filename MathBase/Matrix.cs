using System;

namespace MathBase
{
    public abstract class Matrix<T>
    {
        protected T[,] _data = new T[10, 10];

        public T this[int index1, int index2]
        {
            get
            {
                if (index1 < 0 || index1 >= RowCount || index2 < 0 || index2 >= ColumnCount)
                {
                    throw new IndexOutOfRangeException("Matrix index out of range");
                }
                return _data[index1, index2];
            }
            set
            {
                if (index1 < 0 || index1 >= RowCount || index2 < 0 || index2 >= ColumnCount)
                {
                    throw new IndexOutOfRangeException("Matrix index out of range");
                }
                _data[index1, index2] = value;
            }
        }

        public int RowCount
        {
            get { return _data.GetLength(0); }
        }

        public int ColumnCount
        {
            get { return _data.GetLength(1); }
        }
    }
}
