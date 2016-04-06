using System;
using System.IO;
using Kindruk.lab4;
using MathBase;
using MathBase.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OaCM_labs.Tests
{
    [TestClass]
    public class Lab4Tests
    {
        private bool CheckAlmostEqual(DoubleMatrix expected, DoubleMatrix received)
        {
            var difference = expected - received;
            var result = true;
            foreach (var vector in difference)
                foreach (var val in vector)
                {
                    result = result && Math.Abs(val) < Math.Sqrt(MatrixTransportTaskSolver.Epsilon);
                }
            return result;
        }

        private void Check(string inputFile, string answerFile)
        {
            DoubleMatrix cost;
            DoubleVector production;
            DoubleVector consumption;
            using (var file = File.OpenRead(inputFile))
            {
                using (var reader = new StreamReader(file))
                {
                    cost = MatrixIoManager.LoadMatrix(reader);
                    production = VectorIoManager.LoadVector(reader, cost.RowCount);
                    consumption = VectorIoManager.LoadVector(reader, cost.ColumnCount);
                }
            }
            var result = MatrixTransportTaskSolver.Solve(cost, production, consumption);

            DoubleMatrix answer;
            using (var file = File.OpenRead(answerFile))
            {
                using (var reader = new StreamReader(file))
                {
                    answer = MatrixIoManager.LoadMatrix(reader);
                }
            }

            Assert.IsTrue(CheckAlmostEqual(answer, result) ||
                          MatrixTransportTaskSolver.CalculateTargetFunction(cost, result) <
                          MatrixTransportTaskSolver.CalculateTargetFunction(cost, answer) +
                          MatrixTransportTaskSolver.Epsilon);
        }

        [TestMethod]
        public void Test1()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab4\\test1.csv", dir + "\\tests\\lab4\\ans1.csv");
        }

        [TestMethod]
        public void Test2()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab4\\test2.csv", dir + "\\tests\\lab4\\ans2.csv");
        }

        [TestMethod]
        public void Test3()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab4\\test3.csv", dir + "\\tests\\lab4\\ans3.csv");
        }

        [TestMethod]
        public void Test4()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab4\\test4.csv", dir + "\\tests\\lab4\\ans4.csv");
        }

        [TestMethod]
        public void Test5()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab4\\test5.csv", dir + "\\tests\\lab4\\ans5.csv");
        }

        [TestMethod]
        public void Test6()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab4\\test6.csv", dir + "\\tests\\lab4\\ans6.csv");
        }

        [TestMethod]
        public void Test7()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab4\\test7.csv", dir + "\\tests\\lab4\\ans7.csv");
        }

        [TestMethod]
        public void Test8()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab4\\test8.csv", dir + "\\tests\\lab4\\ans8.csv");
        }
    }
}
