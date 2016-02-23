using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MathBase
{
    public class Polynom
    {
        private int _size;
        private double[] _data;

        public const double Zero = 1e-20;

        #region properties

        public int Size
        {
            get { return _size; }
            private set { _size = value; }
        }

        public double this[int index]
        {
            get
            {
                if (index < 0 || index >= Size)
                {
                    throw new IndexOutOfRangeException("Vector index is out of range");
                }
                return _data[index];
            }
            set
            {
                if (index < 0 || index >= Size)
                {
                    throw new IndexOutOfRangeException("Vector index is out of range");
                }
                _data[index] = value;
            }
        }
        #endregion

        #region constructors

        public Polynom(int size)
        {
            Size = size;
            _data = new double[Size];
        }

        public Polynom(double[] data)
        {
            Size = data.Length;
            _data = new double[Size];
            data.CopyTo(_data, 0);
        }

        #endregion

        public double Calculate(double x)
        {
            var temp = 1.0; 
            var result = 0.0;
            for (var i = 0; i < Size; i++)
            {
                result += temp*_data[i];
                temp *= x;
            }
            return result;
        }

        public double CalculateDerivative(double x, int order)
        {
            var temp = 1.0;
            var result = 0.0;
            for (var i = 0; i < Size; i++)
            {
                var m = 1;
                for (var j = i - order + 1; j < i + 1; j++)
                    m *= j;
                result += temp*_data[i]*m;
                temp *= m == 0 ? 1 : x;
            }
            return result;
        }

        public Polynom Derivative(int order)
        {
            var res = new Polynom(Size - order);
            for (var i = 0; i < Size; i++)
            {
                var m = 1;
                for (var j = i - order + 1; j < i + 1; j++)
                    m *= j;
                if (m != 0)
                {
                    res[i - order] = m*_data[i];
                }                
            }
            return res;
        }

        public void RemoveZeroes()
        {
            while (Math.Abs(_data[Size - 1]) < Zero && Size > 1)
            {
                Size--;
            }
        }

        public bool IsZero()
        {
            return (Size == 1) && (Math.Abs(_data[0]) < Zero);
        }

        public static Polynom operator +(Polynom p1, Polynom p2)
        {
            var res = new Polynom(Math.Max(p1.Size, p2.Size));
            for (var i = 0; i < p1.Size; i++)
                res[i] += p1[i];
            for (var i = 0; i < p2.Size; i++)
                res[i] += p2[i];
            return res;
        }

        public static Polynom operator -(Polynom p1, Polynom p2)
        {
            var res = new Polynom(Math.Max(p1.Size, p2.Size));
            for (var i = 0; i < p1.Size; i++)
                res[i] += p1[i];
            for (var i = 0; i < p2.Size; i++)
                res[i] -= p2[i];
            return res;
        }

        public static Polynom operator -(Polynom p)
        {
            var res = new Polynom(p.Size);
            for (var i = 0; i < res.Size; i++)
                res[i] = -p[i];
            return res;
        }

        public static Polynom operator *(Polynom p1, Polynom p2)
        {
            var res = new Polynom(p1.Size + p2.Size);
            for (var i = 0; i < p1.Size; i++)
                for (var j = 0; j < p2.Size; j++)
                    res[i + j] += p1[i]*p2[j];
            res.RemoveZeroes();
            return res;
        }

        public static Polynom operator *(Polynom p, double val)
        {
            var res = new Polynom(p.Size);
            for (var i = 0; i < res.Size; i++)
            {
                res[i] = p[i]*val;
            }
            return res;
        }

        public static Polynom operator *(double val, Polynom p)
        {
            return p*val;
        }

        public static Polynom operator /(Polynom p1, Polynom p2)
        {
            if (p2.IsZero())
            {
                throw new DivideByZeroException("Division by zero Polynom is prohibited!");
            }
            if (p1.Size < p2.Size)
            {
                return new Polynom(new[]{0.0});
            }
            var res = new Polynom(p1.Size - p2.Size + 1);
            for (var i = 0; i < res.Size; i++)
            {
                res[res.Size - i - 1] = p1[p1.Size - i - 1]/p2[p2.Size - 1];
                var temp = new Polynom(res.Size - i);
                temp[temp.Size - 1] = 1;
                p1 -= p2*res[res.Size - i - 1]*temp;
            }
            return res;
        }

        public static Polynom operator %(Polynom p1, Polynom p2)
        {
            var res = p1 - (p1/p2)*p2;
            res.RemoveZeroes();
            return res;
        }
    }
}
