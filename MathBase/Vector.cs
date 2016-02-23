using System;
using System.Collections;
using System.Collections.Generic;

namespace MathBase
{
    public abstract class Vector<T> : IEnumerable<T>
    {
        protected T[] Data = new T[10];

        public int Length
        {
            get { return Data.Length; }
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Length)
                {
                    throw new IndexOutOfRangeException("Vector index is out of range");
                }
                return Data[index];
            }
            set
            {
                if (index < 0 || index >= Length)
                {
                    throw new IndexOutOfRangeException("Vector index is out of range");
                }
                Data[index] = value;
            }
        }

        private class VectorEnumerator : IEnumerator<T>
        {
            private T[] _data;
            private int _index = -1;

            public VectorEnumerator(T[] data)
            {
                _data = new T[data.Length];
                data.CopyTo(_data, 0);
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                _index++;
                return _index < _data.Length;
            }

            public void Reset()
            {
                _index = -1;
            }

            public T Current { get { return _data[_index]; } }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new VectorEnumerator(Data);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
