using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Kindruk.lab1;
using MathBase;

namespace Kindruk.lab8
{
    public class LinearTask
    {
        public DoubleMatrix Restrictions { get; set; }

        public DoubleVector RestrictionsValue { get; set; }

        public DoubleVector Values { get; set; }

        public DoubleVector StartingResult { get; set; }

        public DoubleVector LowerLimit { get; set; }

        public DoubleVector UpperLimit { get; set; }


        public LinearTask Clone()
        {
            return new LinearTask
            {
                Restrictions = Restrictions,
                RestrictionsValue = RestrictionsValue,
                Values = Values,
                StartingResult = StartingResult,
                LowerLimit = LowerLimit,
                UpperLimit = UpperLimit
            };
        }

        public LinearTask Expand()
        {
            var restrictions = new DoubleMatrix(Restrictions.RowCount + 1, Restrictions.ColumnCount + 1);
            var restrictionsValue = new DoubleVector(RestrictionsValue.Length + 1);
            var values = new DoubleVector(Values.Length + 1);
            var startingResult = new DoubleVector(StartingResult.Length + 1);
            var lowerLimit = new DoubleVector(LowerLimit.Length + 1);
            var upperLimit = new DoubleVector(UpperLimit.Length + 1);
            for (var i = 0; i < Restrictions.RowCount; i++)
            {
                for (var j = 0; j < Restrictions.ColumnCount; j++)
                {
                    restrictions[i, j] = Restrictions[i, j];
                }
                restrictionsValue[i] = RestrictionsValue[i];
            }
            for (var i = 0; i < Restrictions.ColumnCount; i++)
            {
                values[i] = Values[i];
                startingResult[i] = StartingResult[i];
                lowerLimit[i] = LowerLimit[i];
                upperLimit[i] = UpperLimit[i];
            }
            return new LinearTask
            {
                Restrictions = restrictions,
                RestrictionsValue = restrictionsValue,
                Values = values,
                StartingResult = startingResult,
                LowerLimit = lowerLimit,
                UpperLimit = upperLimit
            };
        }
    }


    public static class ClippingMethodSolver
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

        public static bool IsIntegerPlan(DoubleVector v)
        {
            return v.All(it => Math.Abs(Math.Round(it) - it) < Epsilon);
        }

        public static DoubleVector Solve(LinearTask task)
        {
            if (task.Restrictions.RowCount != task.RestrictionsValue.Length)
                throw new ArgumentException("Restrictions dimensions mismatch");

            if (task.Restrictions.ColumnCount != task.StartingResult.Length)
                throw new ArgumentException("Variables count mismatch");

            if (task.StartingResult.Length != task.Values.Length)
                throw new ArgumentException("Bad target function");

            if (task.StartingResult.Count(val => val > Epsilon) != task.Restrictions.RowCount)
                throw new ArgumentException("Bad basis");

            return _solve(task);
        }

        private static string VectorToString(DoubleVector vector)
        {
            return $"({string.Join(",", vector.Select(val => val.ToString("####0.######", CultureInfo.InvariantCulture)))})";
        }

        private static string MatrixToString(DoubleMatrix matrix)
        {
            var vectors = new List<string>();
            for (var i = 0; i < matrix.RowCount; i++)
            {
                vectors.Add(VectorToString(matrix.GetHorizontalVector(i)));
            }
            return $"({string.Join("\n", vectors)})";
        }

        private static DoubleVector _solve(LinearTask task)
        {
            var writer = new StreamWriter(File.OpenWrite("verbose3.log"));
            var result = new DoubleVector(task.StartingResult);
            var curTask = task.Clone();
            var n = task.Restrictions.ColumnCount;

            var done = false;

            while (!done)
            {
                DoubleVector dualRes;
                var solution = _solve(curTask.Restrictions, curTask.RestrictionsValue,
                    curTask.Values, curTask.StartingResult, curTask.LowerLimit, curTask.UpperLimit, out dualRes);
                writer.WriteLine($"x0: {VectorToString(solution)}");
                var realBasisVal = -1;
                var index = 0;
                foreach(var val in basis)
                {
                    if (Math.Abs(Math.Round(solution[val]) - solution[val]) > Epsilon)
                    {
                        realBasisVal = index;
                        break;
                    }
                    index++;
                }
                if (realBasisVal == -1)
                {
                    result = new DoubleVector(solution.Take(n));
                    writer.WriteLine($"Answer: {VectorToString(result)}");
                    done = true;
                    continue;
                }
                if (curTask.Restrictions.ColumnCount > 2 * n)
                {
                    curTask = task.Clone();
                }
                writer.WriteLine($"k: {index}");
                writer.WriteLine($"jk: {realBasisVal}");
                //writer.WriteLine($"A(-1): {MatrixToString(reversedBasisMatrix)}");
                var y = DoubleVector.One(reversedBasisMatrix.ColumnCount, index)*reversedBasisMatrix;
                var alpha = y*curTask.Restrictions;
                var betta = y*curTask.RestrictionsValue;
                writer.WriteLine($"alpha: {VectorToString(alpha)}");
                writer.WriteLine($"betta: {betta.ToString("####0.######", CultureInfo.InvariantCulture)}");
                var m = alpha.Length;
                curTask = curTask.Expand();
                curTask.Values[m] = 0;
                basis.Add(m);
                curTask.LowerLimit[m] = 0;
                curTask.UpperLimit[m] = 99;
                curTask.RestrictionsValue[curTask.Restrictions.RowCount - 1] = Math.Floor(betta) - betta;
                for (var i = 0; i < m; i++)
                {
                    if (Math.Abs(Math.Round(alpha[i]) - alpha[i]) < Epsilon)
                    {
                        alpha[i] = Math.Round(alpha[i]);
                    }
                    curTask.Restrictions[curTask.Restrictions.RowCount - 1, i] = Math.Floor(alpha[i]) - alpha[i];
                }
                curTask.Restrictions[curTask.Restrictions.RowCount - 1, m] = 1;
                curTask.StartingResult = new DoubleVector(m + 1);
                foreach (var val in basis)
                {
                    curTask.StartingResult[val] = 1;
                }

                writer.WriteLine($"A: {MatrixToString(curTask.Restrictions)}");
                writer.WriteLine($"b: {VectorToString(curTask.RestrictionsValue)}");
                writer.WriteLine($"c: {VectorToString(curTask.Values)}");
                writer.WriteLine($"x: {VectorToString(curTask.StartingResult)}");
                writer.WriteLine();
                writer.Flush();
            }

            writer.Close();
            return result;
        }

        private static HashSet<int> basis = new HashSet<int>();
        private static DoubleMatrix reversedBasisMatrix;

        private static DoubleVector _solve(DoubleMatrix restrictions, DoubleVector restrictionsValue,
            DoubleVector values, DoubleVector startingResult, DoubleVector lowerLimit, DoubleVector upperLimit,
            out DoubleVector dualResult)
        {
            var result = new DoubleVector(startingResult);

            var nonBasis = new HashSet<int>();
            var nonBasisPositive = new HashSet<int>();
            var nonBasisNegative = new HashSet<int>();
            basis.Clear();
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
            reversedBasisMatrix = InverseMatrixFinder.Find(basisMatrix);
            if (reversedBasisMatrix.RowCount != basisMatrix.RowCount)
                throw new ArgumentException("Possible linear dependency! Check restrictions");

            dualResult = basisValues * reversedBasisMatrix;
            var delta = dualResult * restrictions - values;

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
                    kappaSum -= restrictions[val] * kappa[val];
                }
                kappaSum = reversedBasisMatrix * kappaSum;
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
                var deltaDualResult = mult * DoubleVector.One(basis.Count, k) * reversedBasisMatrix;
                var mu = deltaDualResult * restrictions;
                var sigma = double.PositiveInfinity;
                var newVal = -1;
                foreach (var val in nonBasis)
                {
                    if ((mu[val] < -Epsilon && nonBasisPositive.Contains(val)) ||
                        (mu[val] > Epsilon && nonBasisNegative.Contains(val)))
                    {
                        //var deltaVal = restrictions[val] * dualResult - values[val];
                        if (sigma > -(delta[val] / mu[val]))
                        {
                            sigma = -(delta[val] / mu[val]);
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
                delta += sigma * mu;
                index = 0;
                dualResult += sigma * deltaDualResult;
                foreach (var val in basis)
                {
                    if (val == oldVal)
                    {
                        basisMatrix[index] = restrictions[newVal];
                        basisValues[index] = values[newVal];
                        var d = reversedBasisMatrix * restrictions[newVal];
                        var tmpVal = d[index];
                        d[index] = -1;
                        d = -1.0 / tmpVal * d;
                        var D = DoubleMatrix.One(basisMatrix.RowCount);
                        D[index] = d;
                        reversedBasisMatrix = D * reversedBasisMatrix;
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