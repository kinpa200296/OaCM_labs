using System.IO;
using Kindruk.lab1;
using MathBase;
using MathBase.Utility;

namespace OaCM_labs.ViewModel
{
    public class Lab1ViewModel : LabViewModelBase
    {
        #region properties

        #endregion

        #region constructors

        public Lab1ViewModel()
        {

        }

        #endregion

        #region commands

        #endregion

        #region methods

        protected override void DoActionBodyExecute()
        {
            DoubleMatrix matrix;
            using (var file = File.OpenRead(InputFile))
            {
                using (var reader = new StreamReader(file))
                {
                    matrix = MatrixIoManager.LoadSquareMatrix(reader);
                }
            }
            var inverseMatrix = InverseMatrixFinder.Find(matrix);
            using (var file = File.OpenWrite(OutputFile))
            {
                using (var writer = new StreamWriter(file))
                {
                    if (inverseMatrix.RowCount == 0)
                    {
                        writer.WriteLine("Impossible to find inverse matrix.");
                    }
                    else
                    {
                        MatrixIoManager.SaveSquareMatrixStandalone(writer, inverseMatrix);
                    }
                }
            }
        }

        #endregion
    }
}
