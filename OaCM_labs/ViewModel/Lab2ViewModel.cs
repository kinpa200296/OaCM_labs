using System.IO;
using Kindruk.lab2;
using MathBase;
using MathBase.Utility;

namespace OaCM_labs.ViewModel
{
    public class Lab2ViewModel : LabViewModelBase
    {
        #region properties

        #endregion

        #region constructors

        public Lab2ViewModel()
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
            DoubleVector values;
            DoubleVector startingResult;
            using (var file = File.OpenRead(InputFile))
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
                    }
                }
            }
        }

        #endregion
    }
}
