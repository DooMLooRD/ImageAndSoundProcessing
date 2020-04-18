using ImageProcessing.Core.FourierTransform;
using ImageProcessing.Core.Helpers;
using ImageProcessing.Core.Model;
using ImageProcessing.View.Helpers;
using ImageProcessing.View.ViewModel.Base;
using ImageProcessing.View.Windows;
using System.Drawing;
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
        public Bitmap ResultBitmap { get; set; }

        public ImageSource PhaseImage { get; set; }
        public Bitmap PhaseBitmap { get; set; }

        public ImageSource MagnitudeImage { get; set; }
        public Bitmap MagnitudeBitmap { get; set; }

        public ImageComponents ImageComponents { get; set; }

        public HistogramVM HistogramVm { get; set; }
        public Histogram Histogram { get; set; }
        public ResultType SelectedResultType { get; set; } = ResultType.Image;

        public SingleResultViewModel()
        {
            HistogramVm = new HistogramVM(Histogram);
            EnlargeResultImage = new RelayCommand(() => ShowImageInFullWindow(ResultImage, ResultBitmap));
            EnlargePhaseImage = new RelayCommand(() => ShowImageInFullWindow(PhaseImage, PhaseBitmap));
            EnlargeMagnitudeImage = new RelayCommand(() => ShowImageInFullWindow(MagnitudeImage, MagnitudeBitmap));
        }

        public void ShowImageInFullWindow(ImageSource imageSource, Bitmap bitmap)
        {
            EnlargedImageWindowVM vm = new EnlargedImageWindowVM(imageSource, bitmap);
            EnlargedImageWindow window = new EnlargedImageWindow()
            {
                DataContext = vm
            };
            window.Show();
        }

        public async void LoadData(Bitmap bitmap)
        {
            await Task.Run(() =>
            {
                Histogram = ImageProcessor.CreateHistogram(bitmap);
                ImageComponents = FastFourierTransform.FFT2D(bitmap);
            });

            HistogramVm = new HistogramVM(Histogram);
            
            ResultBitmap = ImageComponents.Image;
            ResultImage = ImageGdiHelper.LoadBitmap(ImageComponents.Image);
            
            PhaseBitmap = ImageComponents.PhaseImage;
            PhaseImage = ImageGdiHelper.LoadBitmap(ImageComponents.PhaseImage);

            MagnitudeBitmap = ImageComponents.MagnitudeImage;
            MagnitudeImage = ImageGdiHelper.LoadBitmap(ImageComponents.MagnitudeImage);
        }

        public async void LoadData(ImageComponents imageComponents)
        {
            await Task.Run(() =>
            {
                Histogram = ImageProcessor.CreateHistogram(imageComponents.Image);
            });

            ImageComponents = imageComponents;
            HistogramVm = new HistogramVM(Histogram);

            ResultBitmap = ImageComponents.Image;
            ResultImage = ImageGdiHelper.LoadBitmap(ImageComponents.Image);

            PhaseBitmap = ImageComponents.PhaseImage;
            PhaseImage = ImageGdiHelper.LoadBitmap(ImageComponents.PhaseImage);

            MagnitudeBitmap = ImageComponents.MagnitudeImage;
            MagnitudeImage = ImageGdiHelper.LoadBitmap(ImageComponents.MagnitudeImage);
        }
    }
}
