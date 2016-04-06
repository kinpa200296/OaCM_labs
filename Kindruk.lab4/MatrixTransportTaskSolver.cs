using System;
using System.Collections.Generic;
using System.Linq;
using MathBase;

namespace Kindruk.lab4
{
    public static class MatrixTransportTaskSolver
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

        public static double CalculateTargetFunction(DoubleMatrix cost, DoubleMatrix volume)
        {
            var res = 0.0;
            if (cost.RowCount != volume.RowCount || cost.ColumnCount != volume.ColumnCount)
                throw new ArgumentException("Cost and volume matrix dimensions doesn't match");
            for (var i = 0 ; i < cost.RowCount; i++)
                for (var j = 0; j < cost.ColumnCount; j++)
                {
                    res += cost[i, j]*volume[i, j];
                }
            return res;
        }

        public static DoubleMatrix Solve(DoubleMatrix cost, DoubleVector production, DoubleVector consumption)
        {
            if (production.Count(v => v > -Epsilon) != cost.RowCount)
                throw new ArgumentException("Production of any source can't be negative");
            
            if (consumption.Count(v => v > -Epsilon) != cost.ColumnCount)
                throw new ArgumentException("Consumption of any point can't be negative");

            if (Math.Abs(production.Sum() - consumption.Sum()) > Epsilon)
                throw new ArgumentException(
                    $"Total consumption({consumption.Sum()}) should be equal to total production({production.Sum()})");

            return _solve(cost, production, consumption);
        }

        private static DoubleMatrix _solve(DoubleMatrix cost, DoubleVector production, DoubleVector consumption)
        {
            List<Tuple<int, int>> basis;
            var res = _findStartingPlan(production, consumption, out basis);

            var done = false;

            while (!done)
            {
                var u = new DoubleVector(production.Length);
                var v = new DoubleVector(consumption.Length);
                _solveSystem(cost, basis, u, v);

                var delta = _countDelta(cost, u, v);

                int i0;
                int j0;
                _checkPlan(delta, out i0, out j0);
                if (i0 == -1 && j0 == -1)
                {
                    done = true;
                    continue;
                }

                var positiveCycle = new List<Tuple<int, int>>();
                var negativeCycle = new List<Tuple<int, int>>();
                _findCycle(basis, i0, j0, positiveCycle, negativeCycle);

                var i = -1;
                var j = -1;
                var theta = double.PositiveInfinity;
                foreach (var tuple in negativeCycle)
                {
                    if (theta - res[tuple.Item1, tuple.Item2] > Epsilon)
                    {
                        theta = res[tuple.Item1, tuple.Item2];
                        i = tuple.Item1;
                        j = tuple.Item2;
                    }
                    else if (Math.Abs(theta - res[tuple.Item1, tuple.Item2]) < Epsilon &&
                             i*res.ColumnCount + j > tuple.Item1*res.ColumnCount + tuple.Item2)
                    {
                        i = tuple.Item1;
                        j = tuple.Item2;
                    }
                }

                foreach (var tuple in positiveCycle)
                {
                    res[tuple.Item1, tuple.Item2] += theta;
                }
                foreach (var tuple in negativeCycle)
                {
                    res[tuple.Item1, tuple.Item2] -= theta;
                }

                for (var k = 0; k < basis.Count; k++)
                {
                    if (basis[k].Item1 == i && basis[k].Item2 == j)
                    {
                        basis.RemoveAt(k);
                        break;
                    }
                }
                basis.Add(new Tuple<int, int>(i0, j0));
            }

            return res;
        }

        private static DoubleMatrix _findStartingPlan(DoubleVector production, DoubleVector consumption,
            out List<Tuple<int, int>> basis)
        {
            var res = new DoubleMatrix(production.Length, consumption.Length);
            basis = new List<Tuple<int, int>>();
            var i = 0;
            var j = 0;

            while (i < production.Length && j < consumption.Length)
            {
                var tmp = Math.Min(production[i], consumption[j]);
                res[i,j] = tmp;
                basis.Add(new Tuple<int, int>(i, j));
                production[i] -= tmp;
                consumption[j] -= tmp;
                if (Math.Abs(production[i]) < Epsilon)
                {
                    i++;
                }
                else if (Math.Abs(consumption[j]) < Epsilon)
                {
                    j++;
                }
            }

            return res;
        }

        private static void _solveSystem(DoubleMatrix cost, List<Tuple<int, int>> basis, DoubleVector u, DoubleVector v)
        {
            for (var i = 0; i < u.Length; i++)
            {
                u[i] = double.NaN;
            }
            for (var j = 0; j < v.Length; j++)
            {
                v[j] = double.NaN;
            }
            u[0] = 0;
            var queue = new Queue<Tuple<int, int>>();
            foreach (var tuple in basis)
            {
                if (tuple.Item1 == 0)
                {
                    queue.Enqueue(tuple);
                }
            }
            while (queue.Count > 0)
            {
                var tuple = queue.Dequeue();
                if (double.IsNaN(u[tuple.Item1]))
                {
                    u[tuple.Item1] = cost[tuple.Item1, tuple.Item2] - v[tuple.Item2];
                    foreach (var t in basis)
                    {
                        if (t.Item1 == tuple.Item1 && t.Item2 != tuple.Item2)
                        {
                            queue.Enqueue(t);
                        }
                    }
                }
                else if (double.IsNaN(v[tuple.Item2]))
                {
                    v[tuple.Item2] = cost[tuple.Item1, tuple.Item2] - u[tuple.Item1];
                    foreach (var t in basis)
                    {
                        if (t.Item2 == tuple.Item2 && t.Item1 != tuple.Item1)
                        {
                            queue.Enqueue(t);
                        }
                    }
                }
            }
        }

        private static DoubleMatrix _countDelta(DoubleMatrix cost, DoubleVector u, DoubleVector v)
        {
            var res = new DoubleMatrix(cost.RowCount, cost.ColumnCount);

            for (var i = 0; i < res.RowCount; i++)
                for (int j = 0; j < res.ColumnCount; j++)
                {
                    res[i, j] = cost[i, j] - u[i] - v[j];
                }

            return res;
        }

        private static void _checkPlan(DoubleMatrix delta, out int i0, out int j0)
        {
            i0 = -1;
            j0 = -1;
            for (var i = 0; i < delta.RowCount; i++)
                for (var j = 0; j < delta.ColumnCount; j++)
                {
                    if (i0 == -1 && j0 == -1 && delta[i, j] < -Epsilon)
                    {
                        i0 = i;
                        j0 = j;
                        return;
                    }
                }
        }

        private static void _findCycle(List<Tuple<int, int>> basis, int i0, int j0,
            List<Tuple<int, int>> positiveCycle, List<Tuple<int, int>> negativeCycle)
        {
            var nodes = new List<Tuple<int, int>>();
            var length = _dfs(nodes, 1, new Tuple<int, int>(i0, j0), 0, i0, j0, basis);
            if (length <= 1)
            {
                throw new ArgumentException("Cannot find cycle");
            }
            for (var i = 0; i < length; i++)
            {
                if (i%2 == 0)
                {
                    positiveCycle.Add(nodes[i]);
                }
                else
                {
                    negativeCycle.Add(nodes[i]);
                }
            }
        }

        private static int _dfs(List<Tuple<int, int>> nodes, int length, Tuple<int, int> node, int direction, int i0,
            int j0, List<Tuple<int, int>> basis)
        {
            if (nodes.Count < length)
            {
                nodes.Add(node);
            }
            else
            {
                nodes[length - 1] = node;
            }
            if (length > 1 &&
                ((nodes[length - 1].Item1 == i0 && direction == 2) || (nodes[length - 1].Item2 == j0 && direction == 1)))
            {
                return length;
            }
            foreach (var v in basis)
            {
                if (v.Item1 == nodes[length - 1].Item1 && v.Item2 == nodes[length - 1].Item2)
                {
                    continue;
                }
                if (direction == 1 || direction == 0)
                {
                    if (v.Item2 == nodes[length - 1].Item2)
                    {
                        var tmp = _dfs(nodes, length + 1, v, 2, i0, j0, basis);
                        if (tmp > 1)
                        {
                            return tmp;
                        }
                    }
                }
                if (direction == 2 || direction == 0)
                {
                    if (v.Item1 == nodes[length - 1].Item1)
                    {
                        var tmp = _dfs(nodes, length + 1, v, 1, i0, j0, basis);
                        if (tmp > 1)
                        {
                            return tmp;
                        }
                    }
                }
            }
            return 0;
        }
    }
}
