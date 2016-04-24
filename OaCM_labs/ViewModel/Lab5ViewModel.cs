using System.IO;
using Kindruk.lab5;
using MathBase;
using MathBase.Utility;

namespace OaCM_labs.ViewModel
{
    public class Lab5ViewModel : LabViewModelBase
    {
        #region properties

        #endregion

        #region constructors

        public Lab5ViewModel()
        {
        }

        #endregion

        #region commands

        #endregion

        #region methods

        protected override void DoActionBodyExecute()
        {
            DoubleMatrix restrictions;
            DoubleVector restrictionsValue;
            DoubleVector valuesVector;
            DoubleMatrix valuesMatrix;
            DoubleVector startingResult;
            DoubleVector bearingIndexes;
            DoubleVector improvedIndexes;
            using (var file = File.OpenRead(InputFile))
            {
                using (var reader = new StreamReader(file))
                {
                    restrictions = MatrixIoManager.LoadMatrix(reader);
                    restrictionsValue = VectorIoManager.LoadVector(reader, restrictions.RowCount);
                    valuesVector = VectorIoManager.LoadVector(reader);
                    valuesMatrix = MatrixIoManager.LoadMatrix(reader);
                    startingResult = VectorIoManager.LoadVector(reader, restrictions.ColumnCount);
                    bearingIndexes = VectorIoManager.LoadVector(reader, restrictions.ColumnCount);
                    improvedIndexes = VectorIoManager.LoadVector(reader, restrictions.ColumnCount);
                }
            }
            var result = SquareTaskSolver.Solve(restrictions, restrictionsValue, valuesVector, valuesMatrix,
                startingResult, bearingIndexes, improvedIndexes);
            using (var file = File.OpenWrite(OutputFile))
            {
                using (var writer = new StreamWriter(file))
                {
                    if (result.Length == 0)
                    {
                        writer.WriteLine("No solution. Target function is not limited from above.");
                    }
                    else
                    {
                        VectorIoManager.SaveVectorStandalone(writer, result);
                        writer.WriteLine("{0:F6}", SquareTaskSolver.CalculateTargetFunction(valuesVector, valuesMatrix, result));
                    }
                }
            }
        }

        #endregion
    }
}
