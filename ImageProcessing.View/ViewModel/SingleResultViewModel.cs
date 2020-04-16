using ImageProcessing.Core.FourierTransform;
using ImageProcessing.Core.Helpers;
using ImageProcessing.Core.Model;
using ImageProcessing.View.Helpers;
using ImageProcessing.View.ViewModel.Base;
using ImageProcessing.View.Windows;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace ImageProcessing.View.ViewModel
{
    public class SingleResultViewModel : BaseViewModel
    {
        public ICommand EnlargeResultImage { get; set; }
        public ICommand EnlargePhaseImage { get; set; }
        public ICommand EnlargeMagnitudeImage { get; set; }

        public ImageSource ResultImage { get; set; }
        public ImageSource PhaseImage { get; set; }
        public ImageSource MagnitudeImage { get; set; }

        public ImageComponents ImageComponents { get; set; }

        public HistogramVM HistogramVm { get; set; }
        public Histogram Histogram { get; set; }
        public ResultType SelectedResultType { get; set; } = ResultType.Image;

        public SingleResultViewModel()
        {
            HistogramVm = new HistogramVM(Histogram);
            EnlargeResultImage = new RelayCommand(() => ShowImageInFullWindow(ResultImage));
            EnlargePhaseImage = new RelayCommand(() => ShowImageInFullWindow(PhaseImage));
            EnlargeMagnitudeImage = new RelayCommand(() => ShowImageInFullWindow(MagnitudeImage));
        }

        public void ShowImageInFullWindow(ImageSource imageSource)
        {
            EnlargedImageWindowVM vm = new EnlargedImageWindowVM(imageSource);
            EnlargedImageWindow window = new EnlargedImageWindow()
            {
                DataContext = vm
            };
            window.Show();
        }

        public async void LoadData(System.Drawing.Bitmap bitmap)
        {
            await Task.Run(() =>
            {
                Histogram = ImageProcessor.CreateHistogram(bitmap);
                ImageComponents = FastFourierTransform.FFT2D(bitmap);
            });

            HistogramVm = new HistogramVM(Histogram);
            ResultImage = ImageGdiHelper.LoadBitmap(ImageComponents.Image);
            PhaseImage = ImageGdiHelper.LoadBitmap(ImageComponents.PhaseImage);
            MagnitudeImage = ImageGdiHelper.LoadBitmap(ImageComponents.MagnitudeImage);
        }

        public void LoadResultData(System.Drawing.Bitmap bitmap)
        {
            ResultImage = ImageGdiHelper.LoadBitmap(bitmap);
        }

        public async void LoadData(ImageComponents imageComponents)
        {
            await Task.Run(() =>
            {
                Histogram = ImageProcessor.CreateHistogram(imageComponents.Image);
            });

            ImageComponents = imageComponents;
            HistogramVm = new HistogramVM(Histogram);
            ResultImage = ImageGdiHelper.LoadBitmap(ImageComponents.Image);
            PhaseImage = ImageGdiHelper.LoadBitmap(ImageComponents.PhaseImage);
            MagnitudeImage = ImageGdiHelper.LoadBitmap(ImageComponents.MagnitudeImage);
        }
    }
}
