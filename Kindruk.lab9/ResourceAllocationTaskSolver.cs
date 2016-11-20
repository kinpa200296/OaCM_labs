using System.IO;
using MathBase;

namespace Kindruk.lab9
{
    public class AllocationTask
    {
        public int[,] Gain { get; set; }

        public int[,] Optimal { get; set; }

        public int[,] Allocated { get; set; }


        public AllocationTask(DoubleMatrix gain)
        {
            var n = gain.RowCount;
            var c = gain.ColumnCount;

            Gain = new int[n, c];
            Optimal = new int[n, c];
            Allocated = new int[n, c];

            for (var i = 0; i < n; i++)
                for (var j = 0; j < c; j++)
                {
                    Gain[i, j] = (int) gain[j][i];
                }
        }
    }


    public static class ResourceAllocationTaskSolver
    {
        public static int[] Solve(AllocationTask task)
        {
            var writer = new StreamWriter(File.OpenWrite("verbose4.log"));

            var n = task.Gain.GetLength(0);
            var c = task.Gain.GetLength(1);

            for (var i = 0; i < c; i++)
            {
                task.Optimal[0, i] = task.Gain[0, i];
            }

            for (var k = 1; k < n; k++)
                for (var j = 0; j < c; j++)
                    for (var i = 0; i <= j; i++)
                    {
                        if (task.Optimal[k, j] < task.Optimal[k - 1, j - i] + task.Gain[k, i])
                        {
                            task.Optimal[k, j] = task.Optimal[k - 1, j - i] + task.Gain[k, i];
                            task.Allocated[k, j] = i;
                        }
                    }

            var res = new int[n];

            var left = c - 1;
            for (var k = n - 1; k > 0; k--)
            {
                res[k] = task.Allocated[k, left];
                left -= res[k];
            }
            res[0] = left;

            writer.WriteLine("Optimal:");
            var tmp = new IntMatrix(task.Optimal);
            var tmpArray = new string[n];
            for (var k = 0; k < n; k++)
            {
                tmpArray[k] = $"({string.Join(",", tmp.GetHorizontalVector(k))})";
            }
            writer.WriteLine($"({string.Join(",\n", tmpArray)})");
            writer.WriteLine("Allocated:");
            tmp = new IntMatrix(task.Allocated);
            tmpArray = new string[n];
            for (var k = 0; k < n; k++)
            {
                tmpArray[k] = $"({string.Join(",", tmp.GetHorizontalVector(k))})";
            }
            writer.WriteLine($"({string.Join(",\n", tmpArray)})");

            writer.Flush();
            writer.Close();

            return res;
        }
    }
}