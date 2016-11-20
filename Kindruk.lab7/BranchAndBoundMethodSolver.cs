using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Kindruk.lab3x;
using MathBase;

namespace Kindruk.lab7
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
                LowerLimit = new DoubleVector(LowerLimit),
                UpperLimit = new DoubleVector(UpperLimit)
            };
        }
    }

    public static class BranchAndBoundMethodSolver
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

            if (task.Values.Length != task.LowerLimit.Length)
                throw new ArgumentException("Lower limit count mismatch");

            if (task.Values.Length != task.UpperLimit.Length)
                throw new ArgumentException("Upper count mismatch");

            return _solve(task);
        }

        private static string VectorToString(DoubleVector vector)
        {
            return $"({string.Join(",", vector.Select(val => val.ToString("####0.######", CultureInfo.InvariantCulture)))})";
        }

        private static DoubleVector _solve(LinearTask task)
        {
            var writer = new StreamWriter(File.OpenWrite("verbose2.log"));
            var taskList = new List<LinearTask> {task};
            var bestRes = double.NegativeInfinity;
            var bestPlan = new DoubleVector(task.StartingResult);
            for (var i = 0; i < taskList.Count; i++)
            {
                writer.WriteLine($"Iteration {i + 1}");
                var t = taskList[i];
                writer.WriteLine($"Task {i + 1}:");
                writer.WriteLine($"lower = {VectorToString(t.LowerLimit)}");
                writer.WriteLine($"upper = {VectorToString(t.UpperLimit)}");
                DoubleVector dualRes;
                var plan = DualSimplexMethodSolver.Solve(t.Restrictions, t.RestrictionsValue, t.Values, t.StartingResult,
                    t.LowerLimit, t.UpperLimit, out dualRes);
                if (plan.Length == 0)
                {
                    writer.WriteLine("Inconsistent restrictions.");
                    continue;
                }
                var res = plan*t.Values;
                writer.WriteLine($"plan = {VectorToString(plan)}");
                writer.WriteLine($"res = {res.ToString(CultureInfo.InvariantCulture)}");
                if (res > bestRes)
                {
                    writer.WriteLine($"{res} > {bestRes}");
                    if (IsIntegerPlan(plan))
                    {
                        writer.WriteLine("Integer plan. Updating best.");
                        bestRes = res;
                        bestPlan = new DoubleVector(plan);
                    }
                    else
                    {
                        var j0 = 0;
                        for (; j0 < plan.Length; j0++)
                        {
                            if (Math.Abs(Math.Round(plan[j0]) - plan[j0]) > Epsilon)
                            {
                                break;
                            }
                        }
                        writer.WriteLine($"j0 = {j0 + 1}");
                        var t1 = t.Clone();
                        t1.UpperLimit[j0] = Math.Floor(plan[j0]);
                        var t2 = t.Clone();
                        t2.LowerLimit[j0] = Math.Ceiling(plan[j0]);
                        taskList.Add(t1);
                        writer.WriteLine($"Task {taskList.Count}:");
                        writer.WriteLine($"lower = {VectorToString(t1.LowerLimit)}");
                        writer.WriteLine($"upper = {VectorToString(t1.UpperLimit)}");
                        taskList.Add(t2);
                        writer.WriteLine($"Task {taskList.Count}:");
                        writer.WriteLine($"lower = {VectorToString(t2.LowerLimit)}");
                        writer.WriteLine($"upper = {VectorToString(t2.UpperLimit)}");
                    }
                }
                else
                {
                    writer.WriteLine($"{res} < {bestRes}");
                    writer.WriteLine("Ignoring branch.");
                }
            }
            writer.Flush();
            writer.Close();
            return bestPlan;
        }
    }
}