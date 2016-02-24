using System;
using System.IO;
using Kindruk.lab1;
using MathBase;
using MathBase.Utility;
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

        private void Check(string inputFile, string answerFile)
        {
            DoubleMatrix matrix;
            using (var file = File.OpenRead(inputFile))
            {
                using (var reader = new StreamReader(file))
                {
                    matrix = MatrixIoManager.LoadSquareMatrix(reader);
                }
            }

            var result = InverseMatrixFinder.Find(matrix);

            DoubleMatrix answer;
            using (var file = File.OpenRead(answerFile))
            {
                using (var reader = new StreamReader(file))
                {
                    answer = MatrixIoManager.LoadSquareMatrix(reader);
                }
            }

            Assert.IsTrue(CheckAlmostEqual(answer, result));
        }

        [TestMethod]
        public void Test1()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab1\\test1.csv", dir + "\\tests\\lab1\\ans1.csv");
        }

        [TestMethod]
        public void Test2()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab1\\test2.csv", dir + "\\tests\\lab1\\ans2.csv");
        }

        [TestMethod]
        public void Test3()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab1\\test3.csv", dir + "\\tests\\lab1\\ans3.csv");
        }

        [TestMethod]
        public void Test4()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab1\\test4.csv", dir + "\\tests\\lab1\\ans4.csv");
        }

        [TestMethod]
        public void Test5()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab1\\test5.csv", dir + "\\tests\\lab1\\ans5.csv");
        }

        [TestMethod]
        public void Test6()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab1\\test6.csv", dir + "\\tests\\lab1\\ans6.csv");
        }

        [TestMethod]
        public void Test7()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab1\\test7.csv", dir + "\\tests\\lab1\\ans7.csv");
        }
    }
}
