using System;
using System.Collections.Generic;
using MathBase;

namespace Kindruk.lab1
{
    public static class InverseMatrixFinder
    {
        public const double Epsilon = 1e-6;

        public static DoubleMatrix Find(DoubleMatrix source)
        {
            var n = source.RowCount;
            var preResult = DoubleMatrix.One(n);
            var modifiedSource = DoubleMatrix.One(n);
            var resultReorder = new int[n];
            var nonBasis = new SortedSet<int>();
            var basis = new SortedSet<int>();
            for (var i = 0; i < n; i++)
                nonBasis.Add(i);
            for (var i = 0; i < n; i++)
            {
                var newVal = -1;
                foreach (var val in nonBasis)
                {
                    var tmp = DoubleVector.One(n, i)*preResult*source.GetVerticalVector(val);
                    if (Math.Abs(tmp) > Epsilon)
                    {
                        newVal = val;
                        break;
                    }
                }
                if (newVal == -1)
                {
                    return new DoubleMatrix(0, 0);
                }
                nonBasis.Remove(newVal);
                basis.Add(newVal);
                resultReorder[newVal] = i;
                modifiedSource.SetVerticalVector(i, source.GetVerticalVector(newVal));
                var z = preResult*modifiedSource.GetVerticalVector(i);
                var tmpVal = z[i];
                z[i] = -1;
                var d = (-1.0/tmpVal)*z;
                var D = DoubleMatrix.One(n);
                D.SetVerticalVector(i, d);
                preResult = D*preResult;
            }
            var result = new DoubleMatrix(n,n);
            for (var i = 0; i < n; i++)
            {
                result.SetHorizontalVector(i, preResult.GetHorizontalVector(resultReorder[i]));
            }
            return result;
        }
    }
}
