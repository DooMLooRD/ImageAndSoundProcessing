using ImageProcessing.Core.Helpers;
using ImageProcessing.View.ViewModel.Base;
using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace ImageProcessing.View.ViewModel
{
    public class ResultsViewModel : BaseViewModel
    {
        public bool UseSeparateCanals { get; set; }

        public SingleResultViewModel OriginalResult { get; set; }
        public SingleResultViewModel ProcessedResult { get; set; }

        public Visibility HistogramSeparateFactorsVisible { get; set; } = new Visibility();

        public ResultsViewModel()
        {
            OriginalResult = new SingleResultViewModel();
            ProcessedResult = new SingleResultViewModel();
        }

        public void LoadImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog.Filter = "Image files (*.jpg; *.bmp; *.png)|*.jpg; *.bmp; *.png";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                var bitmap = new System.Drawing.Bitmap(openFileDialog.FileName);
                HistogramSeparateFactorsVisible.Visible = !ImageHelper.IsGreyscale(bitmap.PixelFormat);
                UseSeparateCanals = ImageHelper.IsGreyscale(bitmap.PixelFormat) ? false : UseSeparateCanals;
                OriginalResult.LoadData(bitmap);
            }
        }

        public void SaveResult()
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Image file ( *.png)| *.png",
                AddExtension = true,
                OverwritePrompt = true,
                RestoreDirectory = true
            };

            if (sfd.ShowDialog() == true)
            {
                ProcessedResult.ImageComponents.Image.Save(sfd.FileName);
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo($@"{sfd.FileName}")
                    {
                        UseShellExecute = true
                    }
                };
                p.Start();
            }
        }


    }
}
