using System;
using System.IO;
using Kindruk.lab3;
using Kindruk.lab5;
using MathBase;
using MathBase.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OaCM_labs.Tests
{
    [TestClass]
    public class Lab5Tests
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
            DoubleVector valuesVector;
            DoubleMatrix valuesMatrix;
            DoubleVector startingResult;
            using (var file = File.OpenRead(inputFile))
            {
                using (var reader = new StreamReader(file))
                {
                    restrictions = MatrixIoManager.LoadMatrix(reader);
                    restrictionsValue = VectorIoManager.LoadVector(reader, restrictions.RowCount);
                    valuesVector = VectorIoManager.LoadVector(reader);
                    valuesMatrix = MatrixIoManager.LoadMatrix(reader);
                    startingResult = VectorIoManager.LoadVector(reader, restrictions.ColumnCount);
                }
            }

            var result = SquareTaskSolver.Solve(restrictions, restrictionsValue, valuesVector, valuesMatrix,
                startingResult);

            DoubleVector answer;
            using (var file = File.OpenRead(answerFile))
            {
                using (var reader = new StreamReader(file))
                {
                    answer = VectorIoManager.LoadVector(reader);
                }
            }

            Assert.IsTrue(CheckAlmostEqual(answer, result) ||
                          SquareTaskSolver.CalculateTargetFunction(valuesVector, valuesMatrix, result) <
                          SquareTaskSolver.CalculateTargetFunction(valuesVector, valuesMatrix, answer) +
                          SquareTaskSolver.Epsilon);
        }

        [TestMethod]
        public void Test1()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab5\\test1.csv", dir + "\\tests\\lab5\\ans1.csv");
        }

        [TestMethod]
        public void Test2()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab5\\test2.csv", dir + "\\tests\\lab5\\ans2.csv");
        }

        [TestMethod]
        public void Test3()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab5\\test3.csv", dir + "\\tests\\lab5\\ans3.csv");
        }

        [TestMethod]
        public void Test4()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab5\\test4.csv", dir + "\\tests\\lab5\\ans4.csv");
        }

        [TestMethod]
        public void Test5()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab5\\test5.csv", dir + "\\tests\\lab5\\ans5.csv");
        }

        [TestMethod]
        public void Test6()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab5\\test6.csv", dir + "\\tests\\lab5\\ans6.csv");
        }
    }
}
