﻿using System;
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
                result = result && Math.Abs(val) < SimplexMethodSolver.Epsilon;
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
            Check(dir + "\\tests\\sample_in.csv", dir + "\\tests\\sample_out.csv");
        }

        [TestMethod]
        public void Test1()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\test1.csv", dir + "\\tests\\ans1.csv");
        }

        [TestMethod]
        public void Test2()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\test2.csv", dir + "\\tests\\ans2.csv");
        }

        [TestMethod]
        public void Test3()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\test3.csv", dir + "\\tests\\ans3.csv");
        }

        [TestMethod]
        public void Test4()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\test4.csv", dir + "\\tests\\ans4.csv");
        }

        [TestMethod]
        public void Test5()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\test5.csv", dir + "\\tests\\ans5.csv");
        }

        [TestMethod]
        public void Test6()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\test6.csv", dir + "\\tests\\ans6.csv");
        }

        [TestMethod]
        public void Test7()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\test7.csv", dir + "\\tests\\ans7.csv");
        }

        [TestMethod]
        public void Test8()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\test8.csv", dir + "\\tests\\ans8.csv");
        }
    }
}