using System;
using System.Collections.Generic;
using System.Linq;
using Kindruk.lab1;
using MathBase;

namespace Kindruk.lab2
{
    public static class SimplexMethodSolver
    {
        public const double Epsilon = 1e-6;

        public static bool Equals(DoubleVector v1, DoubleVector v2)
        {
            return v1.Length == v2.Length && (v1 - v2).All(val => Math.Abs(val) < Math.Sqrt(Epsilon));
        }

        public static bool Equals(DoubleMatrix m1, DoubleMatrix m2)
        {
            return m1.RowCount == m2.RowCount && m1.ColumnCount == m2.ColumnCount &&
                   (m1 - m2).Select(v => v.All(val => Math.Abs(val) < Epsilon)).All(val => val);
        }

        public static DoubleVector Solve(DoubleMatrix restrictions, DoubleVector restrictionsValue, DoubleVector values,
            DoubleVector startingResult)
        {
            if (restrictions.RowCount != restrictionsValue.Length)
                throw new ArgumentException("Restrictions dimensions mismatch");

            if (restrictions.ColumnCount != startingResult.Length)
                throw new ArgumentException("Variables count mismatch");

            if (startingResult.Length != values.Length)
                throw new ArgumentException("Bad target function");

            if (startingResult.Count(val => val > Epsilon) != restrictions.RowCount)
                throw new ArgumentException("Bad basis");

            if (startingResult.Any(val => val < -Epsilon))
                throw new ArgumentException("Bad plan! Starting result has negative values");

            if (!Equals(restrictions*startingResult, restrictionsValue))
                throw new ArgumentException("Bad plan! Restrictions are not matched");

            return _solve(restrictions, restrictionsValue, values, startingResult);
        }

        private static DoubleVector _solve(DoubleMatrix restrictions, DoubleVector restrictionsValue, DoubleVector values,
            DoubleVector startingResult)
        {
            var result = new DoubleVector(startingResult);

            var nonBasis = new HashSet<int>();
            var basis = new HashSet<int>();
            for (int i = 0; i < result.Length; i++)
            {
                if (Math.Abs(result[i]) < Epsilon)
                {
                    nonBasis.Add(i);
                }
                else
                {
                    basis.Add(i);
                }
            }

            var basisMatrix = new DoubleMatrix(basis.Count, basis.Count);
            var basisValues = new DoubleVector(basis.Count);
            var index = 0;
            foreach (var val in basis)
            {
                basisMatrix[index] = restrictions[val];
                basisValues[index] = values[val];
                index++;
            }
            var reversedBasisMatrix = InverseMatrixFinder.Find(basisMatrix);
            if (reversedBasisMatrix.RowCount != basisMatrix.RowCount)
                throw new ArgumentException("Possible linear dependency! Check restrictions");

            var done = false;

            while (!done)
            {
                var potential = basisValues*reversedBasisMatrix;
                var newVal = -1;
                foreach (var val in nonBasis)
                {
                    if (potential*restrictions[val] - values[val] < -Epsilon)
                    {
                        if ((newVal == -1) || (newVal != -1 && newVal > val))
                        {
                            newVal = val;
                        }
                    }
                }
                if (newVal == -1)
                {
                    done = true;
                    continue;
                }
                var z = reversedBasisMatrix*restrictions[newVal];
                var theta = Double.PositiveInfinity;
                index = 0;
                var oldVal = -1;
                foreach (var val in basis)
                {
                    if (z[index] > Epsilon)
                    {
                        if (theta > result[val]/z[index])
                        {
                            theta = result[val]/z[index];
                            oldVal = val;
                        }
                    }
                    index++;
                }
                if (oldVal == -1)
                {
                    result = new DoubleVector(0);
                    done = true;
                    continue;
                }
                index = 0;
                foreach (var val in basis)
                {
                    result[val] -= theta*z[index];
                    if (val == oldVal)
                    {
                        basisMatrix[index] = restrictions[newVal];
                        basisValues[index] = values[newVal];
                        var d = new DoubleVector(z);
                        var tmpVal = d[index];
                        d[index] = -1;
                        d = -1.0/tmpVal*d;
                        var D = DoubleMatrix.One(basisMatrix.RowCount);
                        D[index] = d;
                        reversedBasisMatrix = D*reversedBasisMatrix;
                    }
                    index++;
                }
                result[newVal] = theta;
                basis.Remove(oldVal);
                basis.Add(newVal);
                nonBasis.Remove(newVal);
                nonBasis.Add(oldVal);
            }

            return result;
        }
    }
}
