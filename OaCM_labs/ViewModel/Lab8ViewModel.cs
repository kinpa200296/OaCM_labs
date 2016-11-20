using System.IO;
using Kindruk.lab8;
using MathBase.Utility;

namespace OaCM_labs.ViewModel
{
    public class Lab8ViewModel : LabViewModelBase
    {
        #region properties

        #endregion

        #region constructors

        public Lab8ViewModel()
        {

        }

        #endregion

        #region commands

        #endregion

        #region methods

        protected override void DoActionBodyExecute()
        {
            var t = new LinearTask();
            using (var file = File.OpenRead(InputFile))
            {
                using (var reader = new StreamReader(file))
                {
                    t.Restrictions = MatrixIoManager.LoadMatrix(reader);
                    t.RestrictionsValue = VectorIoManager.LoadVector(reader, t.Restrictions.RowCount);
                    t.Values = VectorIoManager.LoadVector(reader, t.Restrictions.ColumnCount);
                    t.StartingResult = VectorIoManager.LoadVector(reader, t.Restrictions.ColumnCount);
                    t.LowerLimit = VectorIoManager.LoadVector(reader, t.Restrictions.ColumnCount);
                    t.UpperLimit = VectorIoManager.LoadVector(reader, t.Restrictions.ColumnCount);
                }
            }
            var result = ClippingMethodSolver.Solve(t);
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
                        writer.WriteLine(result*t.Values);
                        VectorIoManager.SaveVector(writer, result);
                    }
                }
            }
        }

        #endregion
    }
}