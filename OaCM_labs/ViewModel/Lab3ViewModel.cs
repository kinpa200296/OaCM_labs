using System.IO;
using Kindruk.lab3;
using MathBase;
using MathBase.Utility;

namespace OaCM_labs.ViewModel
{
    public class Lab3ViewModel : LabViewModelBase
    {
        #region properties

        #endregion

        #region constructors

        public Lab3ViewModel()
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
            DoubleVector dualResult;
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
            var result = DualSimplexMethodSolver.Solve(restrictions, restrictionsValue, values, startingResult,
                out dualResult);
            using (var file = File.OpenWrite(OutputFile))
            {
                using (var writer = new StreamWriter(file))
                {
                    if (result.Length == 0)
                    {
                        writer.WriteLine("No solution. Set of permissible plans is empty.");
                    }
                    else
                    {
                        VectorIoManager.SaveVectorStandalone(writer, result);
                        //VectorIoManager.SaveVector(writer, dualResult);
                    }
                }
            }
        }

        #endregion
    }
}
