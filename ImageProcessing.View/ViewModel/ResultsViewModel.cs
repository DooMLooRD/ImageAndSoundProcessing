using ImageProcessing.Core.Helpers;
using ImageProcessing.View.ViewModel.Base;
using System.Drawing;

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

        public void LoadImage(Bitmap bitmap)
        {
            HistogramSeparateFactorsVisible.Visible = !ImageHelper.IsGreyscale(bitmap.PixelFormat);
            UseSeparateCanals = ImageHelper.IsGreyscale(bitmap.PixelFormat) ? false : UseSeparateCanals;
            OriginalResult.LoadData(bitmap);
        }
    }
}
