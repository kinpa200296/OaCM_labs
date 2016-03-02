using System;
using System.IO;
using Kindruk.lab2;
using MathBase;
using MathBase.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OaCM_labs.Tests
{
    [TestClass]
    public class Lab2Tests
    {
        private bool CheckAlmostEqual(DoubleVector expected, DoubleVector received)
        {
            var difference = expected - received;
            var result = true;
            foreach (var val in difference)
            {
                result = result && Math.Abs(val) < Math.Sqrt(SimplexMethodSolver.Epsilon);
            }
            return result;
        }

        private void Check(string inputFile, string answerFile)
        {
            DoubleMatrix restrictions;
            DoubleVector restrictionsValue;
            DoubleVector values;
            DoubleVector startingResult;
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

            var result = SimplexMethodSolver.Solve(restrictions, restrictionsValue, values, startingResult);

            DoubleVector answer;
            using (var file = File.OpenRead(answerFile))
            {
                using (var reader = new StreamReader(file))
                {
                    answer = VectorIoManager.LoadVector(reader);
                }
            }

            Assert.IsTrue(CheckAlmostEqual(answer, result) ||
                          result*values > answer*values - SimplexMethodSolver.Epsilon);
        }

        [TestMethod]
        public void Test0()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab2\\sample_in.csv", dir + "\\tests\\lab2\\sample_out.csv");
        }

        [TestMethod]
        public void Test1()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab2\\test1.csv", dir + "\\tests\\lab2\\ans1.csv");
        }

        [TestMethod]
        public void Test2()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab2\\test2.csv", dir + "\\tests\\lab2\\ans2.csv");
        }

        [TestMethod]
        public void Test3()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab2\\test3.csv", dir + "\\tests\\lab2\\ans3.csv");
        }

        [TestMethod]
        public void Test4()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab2\\test4.csv", dir + "\\tests\\lab2\\ans4.csv");
        }

        [TestMethod]
        public void Test5()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab2\\test5.csv", dir + "\\tests\\lab2\\ans5.csv");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Bad plan! Restrictions are not matched")]
        public void Test6()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab2\\test6.csv", dir + "\\tests\\lab2\\ans6.csv");
        }

        [TestMethod]
        public void Test7()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab2\\test7.csv", dir + "\\tests\\lab2\\ans7.csv");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Bad plan! Restrictions are not matched")]
        public void Test8()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab2\\test8.csv", dir + "\\tests\\lab2\\ans8.csv");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Possible linear dependency! Check restrictions")]
        public void Test9()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab2\\test9.csv", dir + "\\tests\\lab2\\ans9.csv");
        }

        [TestMethod]
        public void Test10()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab2\\test10.csv", dir + "\\tests\\lab2\\ans10.csv");
        }

        [TestMethod]
        public void Test11()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab2\\test11.csv", dir + "\\tests\\lab2\\ans11.csv");
        }

        [TestMethod]
        public void Test12()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab2\\test12.csv", dir + "\\tests\\lab2\\ans12.csv");
        }

        [TestMethod]
        public void Test13()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab2\\test13.csv", dir + "\\tests\\lab2\\ans13.csv");
        }

        [TestMethod]
        public void Test14()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab2\\test14.csv", dir + "\\tests\\lab2\\ans14.csv");
        }

        [TestMethod]
        public void Test15()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab2\\test15.csv", dir + "\\tests\\lab2\\ans15.csv");
        }

        [TestMethod]
        public void Test16()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab2\\test16.csv", dir + "\\tests\\lab2\\ans16.csv");
        }
    }
}
