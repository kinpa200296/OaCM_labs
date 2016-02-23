using System;
using System.Collections.Generic;
using System.Linq;

namespace MathBase
{
    public class IntVector : Vector<int>
    {
        public IntVector()
        {
            
        }

        public IntVector(int length)
        {
            Data = new int[length];
        }

        public IntVector(IEnumerable<int> data)
        {
            Data = data.ToArray();
        }

        public IntVector(int[] data)
        {
            Data = new int[data.Length];
            data.CopyTo(Data, 0);
        }

        public static IntVector One(int size, int index)
        {
            var result = new IntVector(size);
            result[index] = 1;
            return result;
        }

        public static IntVector operator +(IntVector vector1, IntVector vector2)
        {
            if (vector1.Length != vector2.Length)
            {
                throw new ArgumentException("Vector sizes do not match!!!");
            }
            var vector = new IntVector(vector1.Length);
            for (var i = 0; i < vector.Length; i++)
            {
                vector[i] = vector1[i] + vector2[i];
            }
            return vector;
        }

        public static IntVector operator -(IntVector vector1, IntVector vector2)
        {
            if (vector1.Length != vector2.Length)
            {
                throw new ArgumentException("Vector sizes do not match!!!");
            }
            var vector = new IntVector(vector1.Length);
            for (var i = 0; i < vector.Length; i++)
            {
                vector[i] = vector1[i] - vector2[i];
            }
            return vector;
        }

        public static int operator *(IntVector vector1, IntVector vector2)
        {
            if (vector1.Length != vector2.Length)
            {
                throw new ArgumentException("Vector sizes do not match!!!");
            }
            return vector1.Select((t, i) => t * vector2[i]).Sum();
        }

        public static IntVector operator +(IntVector vector, int val)
        {
            return new IntVector(vector.Select(t => t + val));
        }

        public static IntVector operator -(IntVector vector, int val)
        {
            return new IntVector(vector.Select(t => t - val));
        }

        public static IntVector operator *(IntVector vector, int val)
        {
            return new IntVector(vector.Select(t => t * val));
        }

        public static IntVector operator +(int val, IntVector vector)
        {
            return new IntVector(vector.Select(t => val + t));
        }

        public static IntVector operator -(int val, IntVector vector)
        {
            return new IntVector(vector.Select(t => val - t));
        }

        public static IntVector operator *(int val, IntVector vector)
        {
            return new IntVector(vector.Select(t => val * t));
        }

        public static IntVector operator /(IntVector vector, int val)
        {
            return new IntVector(vector.Select(t => t / val));
        } 
    }
}
