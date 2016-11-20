using System;
using System.Collections.Generic;
using System.Linq;
using Kindruk.lab1;
using MathBase;

namespace Kindruk.lab3x
{
    public class DualSimplexMethodSolver
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
            DoubleVector startingResult, DoubleVector lowerLimit, DoubleVector upperLimit, out DoubleVector dualResult)
        {
            if (restrictions.RowCount != restrictionsValue.Length)
                throw new ArgumentException("Restrictions dimensions mismatch");

            if (restrictions.ColumnCount != startingResult.Length)
                throw new ArgumentException("Variables count mismatch");

            if (startingResult.Length != values.Length)
                throw new ArgumentException("Bad target function");

            if (startingResult.Count(val => val > Epsilon) != restrictions.RowCount)
                throw new ArgumentException("Bad basis");

            //if (startingResult.Any(val => val < -Epsilon))
            //    throw new ArgumentException("Bad plan! Starting result has negative values");

            //if (!Equals(restrictions * startingResult, restrictionsValue))
            //    throw new ArgumentException("Bad plan! Restrictions are not matched");

            if (values.Length != lowerLimit.Length)
                throw new ArgumentException("Lower limit count mismatch");

            if (values.Length != upperLimit.Length)
                throw new ArgumentException("Upper count mismatch");

            return _solve(restrictions, restrictionsValue, values, startingResult, lowerLimit, upperLimit,
                out dualResult);
        }

        private static DoubleVector _solve(DoubleMatrix restrictions, DoubleVector restrictionsValue,
            DoubleVector values, DoubleVector startingResult, DoubleVector lowerLimit, DoubleVector upperLimit,
            out DoubleVector dualResult)
        {
            var result = new DoubleVector(startingResult);

            var nonBasis = new HashSet<int>();
            var nonBasisPositive = new HashSet<int>();
            var nonBasisNegative = new HashSet<int>();
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

            dualResult = basisValues*reversedBasisMatrix;
            var delta = dualResult*restrictions - values;

            var done = false;

            while (!done)
            {
                nonBasisPositive.Clear();
                nonBasisNegative.Clear();
                foreach (var val in nonBasis)
                {
                    if (delta[val] > Epsilon)
                    {
                        nonBasisPositive.Add(val);
                    }
                    if (delta[val] < -Epsilon)
                    {
                        nonBasisNegative.Add(val);
                    }
                }
                var kappa = new DoubleVector(values.Length);
                foreach (var val in nonBasisPositive)
                {
                    kappa[val] = lowerLimit[val];
                }
                foreach (var val in nonBasisNegative)
                {
                    kappa[val] = upperLimit[val];
                }
                var kappaSum = new DoubleVector(restrictionsValue);
                foreach (var val in nonBasis)
                {
                    kappaSum -= restrictions[val]*kappa[val];
                }
                kappaSum = reversedBasisMatrix*kappaSum;
                index = 0;
                foreach (var val in basis)
                {
                    kappa[val] = kappaSum[index];
                    index++;
                }
                index = 0;
                var oldVal = -1;
                var k = -1;
                foreach (var val in basis)
                {
                    if (kappa[val] < lowerLimit[val] - Epsilon || kappa[val] > upperLimit[val] + Epsilon)
                    {
                        if ((oldVal == -1) || (oldVal != -1 && oldVal > val))
                        {
                            oldVal = val;
                            k = index;
                        }
                    }
                    index++;
                }
                if (oldVal == -1)
                {
                    done = true;
                    result = new DoubleVector(kappa);
                    continue;
                }
                var mult = 1;
                if (kappa[oldVal] > upperLimit[oldVal] + Epsilon)
                {
                    mult = -1;
                }
                var deltaDualResult = mult*DoubleVector.One(basis.Count, k)*reversedBasisMatrix;
                var mu = deltaDualResult*restrictions;
                var sigma = double.PositiveInfinity;
                var newVal = -1;
                foreach (var val in nonBasis)
                {
                    if ((mu[val] < -Epsilon && nonBasisPositive.Contains(val)) ||
                        (mu[val] > Epsilon && nonBasisNegative.Contains(val)))
                    {
                        //var deltaVal = restrictions[val] * dualResult - values[val];
                        if (sigma > -(delta[val]/mu[val]))
                        {
                            sigma = -(delta[val]/mu[val]);
                            newVal = val;
                        }
                    }
                }
                if (newVal == -1)
                {
                    result = new DoubleVector(0);
                    done = true;
                    continue;
                }
                delta += sigma*mu;
                index = 0;
                dualResult += sigma*deltaDualResult;
                foreach (var val in basis)
                {
                    if (val == oldVal)
                    {
                        basisMatrix[index] = restrictions[newVal];
                        basisValues[index] = values[newVal];
                        var d = reversedBasisMatrix*restrictions[newVal];
                        var tmpVal = d[index];
                        d[index] = -1;
                        d = -1.0/tmpVal*d;
                        var D = DoubleMatrix.One(basisMatrix.RowCount);
                        D[index] = d;
                        reversedBasisMatrix = D*reversedBasisMatrix;
                    }
                    index++;
                }
                basis.Remove(oldVal);
                basis.Add(newVal);
                nonBasis.Remove(newVal);
                nonBasis.Add(oldVal);
            }

            return result;
        }
    }
}