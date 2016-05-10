using System;
using System.IO;
using Kindruk.lab3;
using MathBase;
using MathBase.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OaCM_labs.Tests
{
    [TestClass]
    public class Lab3Tests
    {
        private bool CheckAlmostEqual(DoubleVector expected, DoubleVector received)
        {
            var difference = expected - received;
            var result = true;
            foreach (var val in difference)
            {
                result = result && Math.Abs(val) < Math.Sqrt(DualSimplexMethodSolver.Epsilon);
            }
            return result;
        }

        private void Check(string inputFile, string answerFile)
        {
            DoubleMatrix restrictions;
            DoubleVector restrictionsValue;
            DoubleVector values;
            DoubleVector startingResult;
            DoubleVector dualResult;
            using (var file = File.OpenRead(inputFile))
            {
                using (var reader = new StreamReader(file))
                {
                    restrictions = MatrixIoManager.LoadMatrix(reader);
                    restrictionsValue = VectorIoManager.LoadVector(reader, restrictions.RowCount);
                    values = VectorIoManager.LoadVector(reader, restrictions.ColumnCount);
                    startingResult = VectorIoManager.LoadVector(reader, restrictions.ColumnCount);
                }
            }

            var result = DualSimplexMethodSolver.Solve(restrictions, restrictionsValue, values, startingResult,
                out dualResult);

            DoubleVector answer;
            using (var file = File.OpenRead(answerFile))
            {
                using (var reader = new StreamReader(file))
                {
                    answer = VectorIoManager.LoadVector(reader);
                }
            }

            Assert.IsTrue(CheckAlmostEqual(answer, result) ||
                          result * values > answer * values - DualSimplexMethodSolver.Epsilon);
        }

        [TestMethod]
        public void Test1()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab3\\test1.csv", dir + "\\tests\\lab3\\ans1.csv");
        }

        [TestMethod]
        public void Test2()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab3\\test2.csv", dir + "\\tests\\lab3\\ans2.csv");
        }

        [TestMethod]
        public void Test3()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab3\\test3.csv", dir + "\\tests\\lab3\\ans3.csv");
        }

        [TestMethod]
        public void Test4()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab3\\test4.csv", dir + "\\tests\\lab3\\ans4.csv");
        }

        [TestMethod]
        public void Test5()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab3\\test5.csv", dir + "\\tests\\lab3\\ans5.csv");
        }

        [TestMethod]
        public void Test6()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab3\\test6.csv", dir + "\\tests\\lab3\\ans6.csv");
        }

        [TestMethod]
        public void Test7()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab3\\test7.csv", dir + "\\tests\\lab3\\ans7.csv");
        }

        [TestMethod]
        public void Test8()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab3\\test8.csv", dir + "\\tests\\lab3\\ans8.csv");
        }

        [TestMethod]
        public void Test9()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab3\\test9.csv", dir + "\\tests\\lab3\\ans9.csv");
        }

        [TestMethod]
        public void Test10()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab3\\test10.csv", dir + "\\tests\\lab3\\ans10.csv");
        }

        [TestMethod]
        public void Test11()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab3\\test11.csv", dir + "\\tests\\lab3\\ans11.csv");
        }
    }
}
