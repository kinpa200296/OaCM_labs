using System;
using System.IO;
using Kindruk.lab6;
using MathBase;
using MathBase.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OaCM_labs.Tests
{
    [TestClass]
    public class Lab6Tests
    {
        private bool CheckAlmostEqual(DoubleVector expected, DoubleVector received)
        {
            var difference = expected - received;
            var result = true;
            foreach (var val in difference)
            {
                result = result && Math.Abs(val) < Math.Sqrt(ConvexTaskSolver.Epsilon);
            }
            return result;
        }

        private void Check(string inputFile, string answerFile)
        {
            var solver = new ConvexTaskSolver();
            using (var file = File.OpenRead(inputFile))
            {
                using (var reader = new StreamReader(file))
                {
                    solver.ReadData(reader);
                }
            }
            solver.Solve();
            DoubleVector answer;
            using (var file = File.OpenRead(answerFile))
            {
                using (var reader = new StreamReader(file))
                {
                    answer = VectorIoManager.LoadVector(reader);
                }
            }
            Assert.IsTrue(CheckAlmostEqual(answer, solver.Result) ||
                          ConvexTaskSolver.CalculateTargetFunction(solver.ValuesVector, solver.ValuesMatrix,
                              solver.Result) <
                          ConvexTaskSolver.CalculateTargetFunction(solver.ValuesVector, solver.ValuesMatrix, answer) +
                          ConvexTaskSolver.Epsilon);
        }

        [TestMethod]
        public void Test1()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab6\\test1.csv", dir + "\\tests\\lab6\\ans1.csv");
        }

        [TestMethod]
        public void Test2()
        {
            var dir = Directory.GetCurrentDirectory();
            Check(dir + "\\tests\\lab6\\test2.csv", dir + "\\tests\\lab6\\ans2.csv");
        }
    }
}
