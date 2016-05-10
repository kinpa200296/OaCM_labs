using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MathBase;
using MathBase.Utility;

namespace Kindruk.lab6
{
    public class ConvexTaskSolver
    {
        public const double Epsilon = 1e-4;

        public static bool Equals(DoubleVector v1, DoubleVector v2)
        {
            return v1.Length == v2.Length && (v1 - v2).All(val => Math.Abs(val) < Math.Sqrt(Epsilon));
        }

        public static bool Equals(DoubleMatrix m1, DoubleMatrix m2)
        {
            return m1.RowCount == m2.RowCount && m1.ColumnCount == m2.ColumnCount &&
                   (m1 - m2).Select(v => v.All(val => Math.Abs(val) < Epsilon)).All(val => val);
        }

        public static DoubleMatrix GetPositiveMatrix(DoubleMatrix valuesMatrix)
        {
            var tmp = new DoubleMatrix(valuesMatrix.ColumnCount, valuesMatrix.RowCount);
            for (var i = 0; i < valuesMatrix.ColumnCount; i++)
            {
                tmp.SetHorizontalVector(i, valuesMatrix[i]);
            }
            return tmp*valuesMatrix;
        }

        public static double CalculateTargetFunction(DoubleVector valuesVector, DoubleMatrix valuesMatrix, DoubleVector plan)
        {
            return CalculateSquareFunction(valuesVector, valuesMatrix, plan, 0.0);
        }

        public static double CalculateSquareFunction(DoubleVector vector, DoubleMatrix matrix, DoubleVector plan, double value)
        {
            if (matrix.ColumnCount != matrix.RowCount)
                matrix = GetPositiveMatrix(matrix);
            return 1.0/2.0*plan*matrix*plan + vector*plan + value;
        }

        public static DoubleVector CalculateDerivative(DoubleVector vector, DoubleMatrix matrix, DoubleVector plan)
        {
            if (matrix.ColumnCount != matrix.RowCount)
                matrix = GetPositiveMatrix(matrix);
            return matrix * plan + vector;
        }

        public DoubleMatrix[] RestrictionsMatrices { get; private set; }
        public DoubleVector[] RestrictionsVectors { get; private set; }
        public DoubleVector RestrictionsValues { get; private set; }
        public DoubleVector ValuesVector { get; private set; }
        public DoubleMatrix ValuesMatrix { get; private set; }
        public DoubleVector StartingResult { get; private set; }
        public DoubleVector StrictResult { get; private set; }
        public DoubleVector UpperBound { get; private set; }
        public DoubleVector LowerBound { get; private set; }
        public DoubleVector Result { get; private set; }

        public void ReadData(StreamReader reader)
        {
            ValuesMatrix = MatrixIoManager.LoadMatrix(reader);
            ValuesVector = VectorIoManager.LoadVector(reader, ValuesMatrix.ColumnCount);
            RestrictionsValues = VectorIoManager.LoadVector(reader);
            RestrictionsMatrices = new DoubleMatrix[RestrictionsValues.Length];
            RestrictionsVectors = new DoubleVector[RestrictionsValues.Length];
            for (var i = 0; i < RestrictionsValues.Length; i++)
            {
                RestrictionsMatrices[i] = MatrixIoManager.LoadMatrix(reader);
            }
            for (var i = 0; i < RestrictionsValues.Length; i++)
            {
                RestrictionsVectors[i] = VectorIoManager.LoadVector(reader, ValuesVector.Length);
            }
            StartingResult = VectorIoManager.LoadVector(reader, ValuesVector.Length);
            StrictResult = VectorIoManager.LoadVector(reader, ValuesVector.Length);
            LowerBound = VectorIoManager.LoadVector(reader, ValuesVector.Length);
            UpperBound = VectorIoManager.LoadVector(reader, ValuesVector.Length);

            ValidateData();
        }

        public void WriteResult(StreamWriter writer)
        {
            if (Result.Length == 0)
            {
                writer.WriteLine("Plan is already optimal");
                writer.WriteLine("{0:F6}", CalculateTargetFunction(ValuesVector, ValuesMatrix, StartingResult));
            }
            else
            {
                VectorIoManager.SaveVectorStandalone(writer, Result);
                writer.WriteLine("{0:F6}", CalculateTargetFunction(ValuesVector, ValuesMatrix, Result));
            }
        }

        public void ValidateData()
        {
            if (ValuesMatrix.ColumnCount != ValuesMatrix.RowCount)
                ValuesMatrix = GetPositiveMatrix(ValuesMatrix);

            if (ValuesMatrix.ColumnCount != ValuesVector.Length)
                throw new ArgumentException("Bad target function vector");

            if (ValuesVector.Length != StartingResult.Length)
                throw new ArgumentException("Bad starting result vector");

            if (StartingResult.Any(val => val < -Epsilon))
                throw new ArgumentException("Bad plan! Starting result has negative values");

            if (ValuesVector.Length != StrictResult.Length)
                throw new ArgumentException("Bad strict result vector");

            if (StartingResult.Any(val => val < -Epsilon))
                throw new ArgumentException("Bad strict result! Strict result has negative values");

            //if ((UpperBound - LowerBound).Any(val => val < -Epsilon))
            //    throw new ArgumentException("Bad bounds");

            for (var i = 0; i < RestrictionsMatrices.Length; i++)
            {
                if (RestrictionsMatrices[i].RowCount != RestrictionsMatrices[i].ColumnCount)
                    RestrictionsMatrices[i] = GetPositiveMatrix(RestrictionsMatrices[i]);

                if (RestrictionsMatrices[i].RowCount != ValuesVector.Length ||
                    RestrictionsMatrices[i].ColumnCount != ValuesVector.Length)
                    throw new ArgumentException($"Bad restriction matrix {i + 1}");

                if (RestrictionsVectors[i].Length != ValuesVector.Length)
                    throw new ArgumentException($"Bad restriction vector {i + 1}");

                var g = CalculateSquareFunction(RestrictionsVectors[i], RestrictionsMatrices[i], StartingResult,
                    RestrictionsValues[i]);

                if (g > Epsilon)
                    throw new ArgumentException($"Bad plan! Restriction {i + 1} is not matched");

                g = CalculateSquareFunction(RestrictionsVectors[i], RestrictionsMatrices[i], StrictResult,
                    RestrictionsValues[i]);

                if (g > -Epsilon)
                    throw new ArgumentException($"Bad strict result! Restriction {i + 1} is not matched");

                Result = new DoubleVector(StartingResult);
            }
        }

        public void Solve()
        {
            ValidateData();
            var solver = new Solver(this);
            solver.InvokeSolve();
        }

        private class Solver
        {
            private ConvexTaskSolver _instance;
            private DoubleVector _direction;
            private HashSet<int> _activeRestrictions, _nonBasis; 

            public Solver(ConvexTaskSolver instance)
            {
                _instance = instance;

                _activeRestrictions = new HashSet<int>();
                _nonBasis = new HashSet<int>();

                _direction = new DoubleVector();
            }

            public void InvokeSolve()
            {
                for (var i = 0; i < _instance.RestrictionsValues.Length; i++)
                {
                    if (_calculateSquareFunction(i, _instance.Result) > -Epsilon)
                    {
                        _activeRestrictions.Add(i);
                    }
                }
                for (var i = 0; i < _instance.Result.Length; i++)
                {
                    if (_instance.Result[i] < Epsilon)
                    {
                        _nonBasis.Add(i);
                    }
                }

                _findDirection();

                _improveResult();
            }

            private double _calculateSquareFunction(int index, DoubleVector plan)
            {
                return CalculateSquareFunction(_instance.RestrictionsVectors[index],
                    _instance.RestrictionsMatrices[index], plan, _instance.RestrictionsValues[index]);
            }

            private void _findDirection()
            {
                var n = _instance.Result.Length;
                var m = _activeRestrictions.Count;
                var restrictions = new DoubleMatrix(m + 2*n, m + 3*n);
                var restrictionsValue = new DoubleVector(m + 2*n);
                var values = new DoubleVector(m + 3*n);
                var startingResult = new DoubleVector(values.Length);
                var i = 0;
                foreach (var val in _activeRestrictions)
                {
                    restrictionsValue[i] = 0;
                    var deriv = _calculateDerivative(val);
                    for (var j = 0; j < n; j++)
                    {
                        restrictions[i, j] = deriv[j];
                    }
                    restrictions[i, n + i] = 1;
                    values[n + i] = -1000;
                    startingResult[n + i] = 1;
                    i++;
                }
                var der = CalculateDerivative(_instance.ValuesVector, _instance.ValuesMatrix, _instance.Result);
                for (var j = 0; j < n; j++)
                {
                    values[j] = -der[j];
                }
                for (var j = n + m; j < 2*n + m; j++, i++)
                {
                    values[j] = -1000;
                    startingResult[j] = 1;
                    restrictionsValue[i] = _instance.UpperBound[j - n - m];
                    restrictions[i, j] = 1;
                    restrictions[i, j - n - m] = 1;
                }
                for (var j = 2*n + m; j < 3*n + m; j++, i++)
                {
                    values[j] = -1000;
                    startingResult[j] = 1;
                    restrictionsValue[i] = -_instance.LowerBound[j - 2*n - m];
                    restrictions[i, j] = 1;
                    restrictions[i, j - 2*n - m] = -1;
                }
                using (var tmpFile = File.OpenWrite("dump.csv"))
                {
                    using (var writer = new StreamWriter(tmpFile))
                    {
                        MatrixIoManager.SaveMatrixStandalone(writer, restrictions);
                        VectorIoManager.SaveVectorStandalone(writer, restrictionsValue);
                        VectorIoManager.SaveVectorStandalone(writer, values);
                        VectorIoManager.SaveVectorStandalone(writer, startingResult);
                    }
                }
                DoubleVector dualTmp;
                var tmp = DualSimplexMethodSolver.Solve(restrictions, restrictionsValue, values, startingResult,
                    out dualTmp);
                _direction = new DoubleVector(n);
                for (var j = 0; j < n; j++)
                {
                    _direction[j] = tmp[j];
                }
            }

            private DoubleVector _calculateDerivative(int index)
            {
                return CalculateDerivative(_instance.RestrictionsVectors[index], _instance.RestrictionsMatrices[index],
                    _instance.Result);
            }

            private void _improveResult()
            {
                var df = CalculateDerivative(_instance.ValuesVector, _instance.ValuesMatrix, _instance.Result);
                var a = df*_direction;
                if (Math.Abs(a) < Epsilon)
                {
                    _instance.Result = new DoubleVector(0);
                    return;
                }
                if (a > Epsilon)
                {
                    throw new Exception("Something went wrong...");
                }
                var b = df*(_instance.StrictResult - _instance.Result);
                var alpha = 1.0;
                if (b > Epsilon)
                {
                    alpha = -a/2.0/b;
                }

                var t = _findT(alpha);

                _instance.Result = _calculateNewResult(alpha, t);
            }

            private DoubleVector _calculateNewResult(double alpha, double t)
            {
                return _instance.Result + t*_direction + alpha*t*(_instance.StrictResult - _instance.Result);
            }

            private double _findT(double alpha)
            {
                var res = 0.0;
                var best = _calculateTargetFunction(_calculateNewResult(alpha, res));
                for (var t = 0.0; t < 1.0; t += 1e-4)
                {
                    var tmpRes = _calculateNewResult(alpha, t);
                    if (!_checkRestrictions(tmpRes))
                    {
                        continue;
                    }
                    var tmp = _calculateTargetFunction(tmpRes);
                    if (best > tmp)
                    {
                        best = tmp;
                        res = t;
                    }
                }

                return res;
            }

            private double _calculateTargetFunction(DoubleVector plan)
            {
                return CalculateTargetFunction(_instance.ValuesVector, _instance.ValuesMatrix, plan);
            }

            private bool _checkRestrictions(DoubleVector plan)
            {
                for (var i = 0; i < _instance.RestrictionsValues.Length; i++)
                {
                    if (_calculateSquareFunction(i, plan) > Epsilon)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
