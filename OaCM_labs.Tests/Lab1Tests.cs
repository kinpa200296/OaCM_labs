using System;
using Kindruk.lab1;
using MathBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OaCM_labs.Tests
{
    [TestClass]
    public class Lab1Tests
    {
        private bool CheckAlmostEqual(DoubleMatrix expected, DoubleMatrix received)
        {
            var difference = expected - received;
            var result = true;
            foreach (var vector in difference)
            {
                foreach (var val in vector)
                {
                    result = result && Math.Abs(val) < InverseMatrixFinder.Epsilon;
                }
            }
            return result;
        }

        [TestMethod]
        public void Test1()
        {
            var matrix = new DoubleMatrix(new double[,] {{0, 2, 1}, {0, 1, 1}, {1, 1, 1}});
            var answer = new DoubleMatrix(new double[,] {{0, -1, 1}, {1, -1, 0}, {-1, 2, 0}});
            var result = InverseMatrixFinder.Find(matrix);
            Assert.IsTrue(CheckAlmostEqual(result, answer));
        }

        [TestMethod]
        public void Test2()
        {
            var matrix = new DoubleMatrix(new double[,] {{2}});
            var answer = new DoubleMatrix(new[,] {{0.5}});
            var result = InverseMatrixFinder.Find(matrix);
            Assert.IsTrue(CheckAlmostEqual(result, answer));
        }

        [TestMethod]
        public void Test3()
        {
            var matrix = new DoubleMatrix(new double[,] {{-3, 0, 2}, {3, -1, 0}, {-2, 0, 1}});
            var answer = new DoubleMatrix(new double[,] {{1, 0, -2}, {3, -1, -6}, {2, 0, -3}});
            var result = InverseMatrixFinder.Find(matrix);
            Assert.IsTrue(CheckAlmostEqual(result, answer));
        }

        [TestMethod]
        public void Test4()
        {
            var matrix = new DoubleMatrix(new double[,] {{-1, 2, 1}, {1, 1, 1}, {3, -1, 1}});
            var answer = new DoubleMatrix(new[,] {{-1, 1.5, -0.5}, {-1, 2, -1}, {2, -2.5, 1.5}});
            var result = InverseMatrixFinder.Find(matrix);
            Assert.IsTrue(CheckAlmostEqual(result, answer));
        }

        [TestMethod]
        public void Test5()
        {
            var matrix = new DoubleMatrix(new double[,] {{-3, -2, 1}, {1, 1, 0}, {0, -1, 0}});
            var answer = new DoubleMatrix(new double[,] {{0, 1, 1}, {0, 0, -1}, {1, 3, 1}});
            var result = InverseMatrixFinder.Find(matrix);
            Assert.IsTrue(CheckAlmostEqual(result, answer));
        }

        [TestMethod]
        public void TestNoSolution1()
        {
            var matrix = new DoubleMatrix(new double[,] {{0, 0, 0}, {2, 2, 8}, {1, 1, 1}});
            var result = InverseMatrixFinder.Find(matrix);
            Assert.IsTrue(result.RowCount == 0 && result.ColumnCount == 0);
        }

        [TestMethod]
        public void TestNoSolution2()
        {
            var matrix = new DoubleMatrix(new[,] {{2, 4, 1}, {0.5, 1, 0.25}, {11, 3, 24}});
            var result = InverseMatrixFinder.Find(matrix);
            Assert.IsTrue(result.RowCount == 0 && result.ColumnCount == 0);
        }
    }
}
