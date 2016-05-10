using System.IO;
using Kindruk.lab6;

namespace OaCM_labs.ViewModel
{
    public class Lab6ViewModel : LabViewModelBase
    {
        #region properties

        #endregion

        #region constructors

        public Lab6ViewModel()
        {
        }

        #endregion

        #region commands

        #endregion

        #region methods

        protected override void DoActionBodyExecute()
        {
            var solver = new ConvexTaskSolver();
            using (var file = File.OpenRead(InputFile))
            {
                using (var reader = new StreamReader(file))
                {
                    solver.ReadData(reader);
                }
            }
            solver.Solve();
            using (var file = File.OpenWrite(OutputFile))
            {
                using (var writer = new StreamWriter(file))
                {
                    solver.WriteResult(writer);
                }
            }
        }

        #endregion
    }
}
