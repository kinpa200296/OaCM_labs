using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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

        public ICommand DoAction
        {
            get { return new RelayCommand(DoActionExecute);}
        }

        #endregion

        #region methods

        private async void DoActionExecute()
        {
            ButtonVisibility = Visibility.Collapsed;
            ProgressBarVisibility = Visibility.Visible;
            try
            {
                DoubleMatrix matrix;
                using (var file = File.OpenRead(InputFile))
                {
                    using (var reader = new StreamReader(file))
                    {
                        matrix = MatrixIoManager.LoadSquareMatrix(reader);
                    }
                }
                await Task.Delay(500);
                var inverseMatrix =
                    await Task<DoubleMatrix>.Factory.StartNew(() => { return InverseMatrixFinder.Find(matrix); });
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
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                ButtonVisibility = Visibility.Visible;
                ProgressBarVisibility = Visibility.Collapsed;
            }
        }

        #endregion
    }
}
