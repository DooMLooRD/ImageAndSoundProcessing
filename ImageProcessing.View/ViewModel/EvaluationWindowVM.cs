using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Input;
using ImageProcessing.Core.Helpers;
using ImageProcessing.View.ViewModel.Base;
using Microsoft.Win32;

namespace ImageProcessing.View.ViewModel
{
    public class EvaluationWindowVM : BaseViewModel
    {
        public Bitmap OriginalBitmap { get; set; }
        public string OriginalPath { get; set; }
        public Bitmap ResultBitmap { get; set; }
        public string ResultPath { get; set; }
        public string Results { get; set; }

        public ICommand OpenOriginal { get; set; }
        public ICommand OpenResult { get; set; }
        public ICommand Calculate { get; set; }

        public EvaluationWindowVM()
        {
            OpenOriginal = new RelayCommand(() => LoadImage(value => OriginalBitmap = value, str => OriginalPath = str));
            OpenResult = new RelayCommand(() => LoadImage(value => ResultBitmap = value, str => ResultPath = str));
            Calculate = new RelayCommand(OnCalculate);
        }

        private void OnCalculate()
        {
            if (OriginalBitmap == null || ResultBitmap == null)
            {
                return;
            }

            Results = ImageProcessor.FiltersEvaluation(OriginalBitmap, ResultBitmap).ToString();

        }

        public void LoadImage(Action<Bitmap> setBitmap, Action<string> setPath)
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "Image files (*.jpg; *.bmp; *.png)|*.jpg; *.bmp; *.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var name = openFileDialog.FileName;
                setBitmap(new Bitmap(name));
                setPath(name);
            }
        }
    }


}