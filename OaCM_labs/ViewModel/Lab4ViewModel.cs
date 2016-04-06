using System;
using System.IO;
using Kindruk.lab4;
using MathBase;
using MathBase.Utility;

namespace OaCM_labs.ViewModel
{
    public class Lab4ViewModel: LabViewModelBase
    {
        #region properties

        #endregion

        #region constructors

        public Lab4ViewModel()
        {

        }

        #endregion

        #region commands

        #endregion

        #region methods

        protected override void DoActionBodyExecute()
        {
            DoubleMatrix cost;
            DoubleVector production;
            DoubleVector consumption;
            using (var file = File.OpenRead(InputFile))
            {
                using (var reader = new StreamReader(file))
                {
                    cost = MatrixIoManager.LoadMatrix(reader);
                    production = VectorIoManager.LoadVector(reader, cost.RowCount);
                    consumption = VectorIoManager.LoadVector(reader, cost.ColumnCount);
                }
            }
            var result = MatrixTransportTaskSolver.Solve(cost, production, consumption);
            using (var file = File.OpenWrite(OutputFile))
            {
                using (var writer = new StreamWriter(file))
                {
                    if (result.RowCount != cost.RowCount || result.ColumnCount != cost.ColumnCount)
                    {
                        writer.WriteLine("No solution.");
                    }
                    else
                    {
                        MatrixIoManager.SaveMatrixStandalone(writer, result);
                        writer.WriteLine("{0:F6}", MatrixTransportTaskSolver.CalculateTargetFunction(cost, result));
                    }
                }
            }
        }

        #endregion
    }
}
