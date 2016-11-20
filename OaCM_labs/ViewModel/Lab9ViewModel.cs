using System.IO;
using Kindruk.lab9;
using MathBase;
using MathBase.Utility;

namespace OaCM_labs.ViewModel
{
    public class Lab9ViewModel : LabViewModelBase
    {
        #region properties

        #endregion

        #region constructors

        public Lab9ViewModel()
        {

        }

        #endregion

        #region commands

        #endregion

        #region methods

        protected override void DoActionBodyExecute()
        {
            DoubleMatrix gain;
            using (var file = File.OpenRead(InputFile))
            {
                using (var reader = new StreamReader(file))
                {
                    gain = MatrixIoManager.LoadMatrix(reader);
                }
            }
            var t = new AllocationTask(gain);
            var result = ResourceAllocationTaskSolver.Solve(t); 
            using (var file = File.OpenWrite(OutputFile))
            {
                using (var writer = new StreamWriter(file))
                {
                    writer.WriteLine(t.Optimal[gain.RowCount - 1, gain.ColumnCount - 1]);
                    writer.WriteLine(string.Join(",", result));
                }
            }
        }

        #endregion
    }
}