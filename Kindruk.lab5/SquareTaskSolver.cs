using System;
using System.Collections.Generic;
using System.Linq;
using Kindruk.lab1;
using MathBase;

namespace Kindruk.lab5
{
    public static class SquareTaskSolver
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

        public static DoubleMatrix GetValuesMatrix(DoubleMatrix valuesMatrix)
        {
            var tmp = new DoubleMatrix(valuesMatrix.ColumnCount, valuesMatrix.RowCount);
            for (var i = 0; i < valuesMatrix.ColumnCount; i++)
            {
                tmp.SetHorizontalVector(i, valuesMatrix[i]);
            }
            return tmp * valuesMatrix;
        }

        public static DoubleVector GetValuesVector(DoubleVector valuesVector, DoubleMatrix valuesMatrix)
        {
            return -1*valuesVector*valuesMatrix;
        }

        public static double CalculateTargetFunction(DoubleVector valuesVector, DoubleMatrix valuesMatrix, DoubleVector plan)
        {
            if (valuesVector.Length != valuesMatrix.ColumnCount)
                valuesVector = GetValuesVector(valuesVector, valuesMatrix);
            if (valuesMatrix.ColumnCount != valuesMatrix.RowCount)
                valuesMatrix = GetValuesMatrix(valuesMatrix);
            return 1.0/2.0*plan*valuesMatrix*plan + valuesVector*plan;
        }

        public static DoubleVector Solve(DoubleMatrix restrictions, DoubleVector restrictionsValue,
            DoubleVector valuesVector, DoubleMatrix valuesMatrix, DoubleVector startingResult)
        {
            if (restrictions.RowCount != restrictionsValue.Length)
                throw new ArgumentException("Restrictions dimensions mismatch");

            if (restrictions.ColumnCount != startingResult.Length)
                throw new ArgumentException("Variables count mismatch");

            if (valuesVector.Length != restrictions.ColumnCount)
                valuesVector = GetValuesVector(valuesVector, valuesMatrix);

            if (restrictions.ColumnCount != valuesMatrix.RowCount)
                valuesMatrix = GetValuesMatrix(valuesMatrix);

            if (restrictions.ColumnCount != valuesVector.Length)
                throw new ArgumentException("Bad target function vector");

            if (restrictions.ColumnCount != valuesMatrix.RowCount ||
                restrictions.ColumnCount != valuesMatrix.ColumnCount)
                throw new ArgumentException("Bad target function matrix");

            if (startingResult.Count(val => val > Epsilon) != restrictions.RowCount)
                throw new ArgumentException("Bad basis");

            if (startingResult.Any(val => val < -Epsilon))
                throw new ArgumentException("Bad plan! Starting result has negative values");

            if (!Equals(restrictions*startingResult, restrictionsValue))
                throw new ArgumentException("Bad plan! Restrictions are not matched");

            return _solve(restrictions, restrictionsValue, valuesVector, valuesMatrix, startingResult);
        }

        private static DoubleVector _solve(DoubleMatrix restrictions, DoubleVector restrictionsValue,
            DoubleVector valuesVector, DoubleMatrix valuesMatrix, DoubleVector startingResult)
        {
            var solver = new Solver(restrictions, restrictionsValue, valuesVector, valuesMatrix, startingResult);
            solver.InvokeSolve();
            return solver.Result;
        }

        private class Solver
        {
            public DoubleMatrix Restrictions, ValuesMatrix, BearingRestrictions, BearingReversedRestrictions;
            public DoubleVector RestrictionsValue, ValuesVector, StartingResult, Result, Delta, Direction;
            public HashSet<int> NonBasis, BearingBasis, ImprovedBasis;

            private bool _done;
            private int _j0, _js;
            private double _theta0;

            public Solver(DoubleMatrix restrictions, DoubleVector restrictionsValue, DoubleVector valuesVector,
                DoubleMatrix valuesMatrix, DoubleVector startingResult)
            {
                Restrictions = restrictions;
                RestrictionsValue = restrictionsValue;
                ValuesVector = valuesVector;
                ValuesMatrix = valuesMatrix;
                StartingResult = startingResult;

                Result = new DoubleVector(StartingResult);

                NonBasis = new HashSet<int>();
                BearingBasis = new HashSet<int>();
                ImprovedBasis = new HashSet<int>();
            }

            public void InvokeSolve()
            {
                _buildBasis();
                _done = false;
                while (!_done)
                {
                    _calcDelta();
                }
            }

            private void _buildBasis()
            {
                for (var i = 0; i < Result.Length; i++)
                {
                    if (Math.Abs(Result[i]) < Epsilon)
                    {
                        NonBasis.Add(i);
                    }
                    else
                    {
                        BearingBasis.Add(i);
                        ImprovedBasis.Add(i);
                    }
                }
                _makeBearingBasis();
            }

            private void _makeBearingBasis()
            {
                BearingRestrictions = new DoubleMatrix(BearingBasis.Count, BearingBasis.Count);
                var index = 0;
                foreach (var val in BearingBasis)
                {
                    BearingRestrictions[index++] = Restrictions[val];
                }
                BearingReversedRestrictions = InverseMatrixFinder.Find(BearingRestrictions);
                if (BearingReversedRestrictions.RowCount != BearingRestrictions.RowCount)
                    throw new ArgumentException("Possible linear dependency! Check restrictions");
            }

            private void _calcDelta()
            {
                var c = ValuesMatrix*Result + ValuesVector;
                var bearingC = new DoubleVector(BearingBasis.Count);
                var index = 0;
                foreach (var val in BearingBasis)
                {
                    bearingC[index++] = c[val];
                }
                var u = -1*bearingC*BearingReversedRestrictions;
                Delta = new DoubleVector(Restrictions.ColumnCount);
                for (var i = 0; i < Restrictions.ColumnCount; i++)
                {
                    Delta[i] = u*Restrictions[i] + c[i];
                }

                _checkDelta();
            }

            private void _checkDelta()
            {
                _j0 = -1;
                foreach (var val in NonBasis)
                {
                    if (Delta[val] < -Epsilon)
                    {
                        if (_j0 == -1 || (_j0 != -1 && _j0 > val))
                        {
                            _j0 = val;
                        }
                    }
                }
                if (_j0 == -1)
                {
                    _done = true;
                    return;
                }

                _findDirection();
            }

            private void _findDirection()
            {
                var k = ImprovedBasis.Count;
                var m = Restrictions.RowCount;
                var improvedRestrictions = new DoubleMatrix(m, k);
                var improvedValuesMatrix = new DoubleMatrix(k, k);
                var index = 0;
                foreach (var val in ImprovedBasis)
                {
                    var indexj = 0;
                    foreach (var valj in ImprovedBasis)
                    {
                        improvedValuesMatrix[index, indexj++] = ValuesMatrix[val, valj];
                    }
                    improvedRestrictions[index++] = Restrictions[val];
                }
                var h = new DoubleMatrix(k + m, k + m);
                var hVal = new DoubleVector(k + m);
                for (var i = 0; i < k; i++)
                {
                    for (var j = 0; j < k; j++)
                    {
                        h[i, j] = improvedValuesMatrix[i, j];
                    }
                    //hVal[i] = ValuesMatrix[i, _j0];
                }
                for (var i = 0; i < m; i++)
                {
                    for (var j = 0; j < k; j++)
                    {
                        h[i + k, j] = improvedRestrictions[i, j];
                        h[j, i + k] = improvedRestrictions[i, j];
                    }
                    hVal[i + k] = Restrictions[i, _j0];
                }
                index = 0;
                foreach (var val in ImprovedBasis)
                {
                    hVal[index++] = ValuesMatrix[val, _j0];
                }
                var inverseH = InverseMatrixFinder.Find(h);
                var z = -1*inverseH*hVal;
                Direction = new DoubleVector(StartingResult.Length);
                Direction[_j0] = 1;
                index = 0;
                foreach (var val in ImprovedBasis)
                {
                    Direction[val] = z[index++];
                }

                _findTheta();
            }

            private void _findTheta()
            {
                _theta0 = double.PositiveInfinity;
                _js = -1;
                var tmp = Direction*ValuesMatrix*Direction;
                if (tmp > Epsilon)
                {
                    _js = _j0;
                    _theta0 = Math.Abs(Delta[_j0])/tmp;
                }
                foreach (var val in ImprovedBasis)
                {
                    if (Direction[val] < -Epsilon)
                    {
                        var temp = -Result[val]/Direction[val];
                        if (_theta0 > temp || (Math.Abs(_theta0 - temp) < Epsilon && _js != -1 && _js > val))
                        {
                            _theta0 = temp;
                            _js = val;
                        }
                    }
                }
                if (_js == -1)
                {
                    Result = new DoubleVector(0);
                    _done = true;
                    return;
                }
                Result += _theta0*Direction;
                if (_js == _j0)
                {
                    NonBasis.Remove(_j0);
                    ImprovedBasis.Add(_j0);
                    _calcDelta();
                    return;
                }
                if (ImprovedBasis.Contains(_js) && !BearingBasis.Contains(_js))
                {
                    ImprovedBasis.Remove(_js);
                    NonBasis.Add(_js);
                    Delta[_j0] += _theta0*tmp;
                    _findDirection();
                    return;
                }
                var index = 0;
                var k = -1;
                foreach (var val in BearingBasis)
                {
                    if (_js == val)
                    {
                        k = index;
                    }
                    index++;
                }
                if (k == -1)
                    throw new IndexOutOfRangeException("Something not right...");
                var tempBasis = new HashSet<int>(ImprovedBasis);
                tempBasis.ExceptWith(BearingBasis);
                var newj = -1;
                foreach (var val in tempBasis)
                {
                    if (
                        Math.Abs(DoubleVector.One(Restrictions.RowCount, k)*BearingReversedRestrictions*
                                 Restrictions[val]) > Epsilon)
                    {
                        newj = val;
                    }
                }
                if (newj == -1)
                {
                    NonBasis.Add(_js);
                    NonBasis.Remove(_j0);
                    ImprovedBasis.Remove(_js);
                    BearingBasis.Remove(_js);
                    ImprovedBasis.Add(_j0);
                    BearingBasis.Add(_j0);
                    _makeBearingBasis();
                    _calcDelta();
                }
                else
                {
                    NonBasis.Add(_js);
                    ImprovedBasis.Remove(_js);
                    BearingBasis.Add(newj);
                    BearingBasis.Remove(_js);
                    Delta[_j0] += _theta0*tmp;
                    _makeBearingBasis();
                    _findDirection();
                }
            }
        }
    }
}
