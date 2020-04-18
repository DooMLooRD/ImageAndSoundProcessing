using ImageProcessing.View.ViewModel.Base;
using ImageProcessing.View.Windows;
using Microsoft.Win32;
using System.Drawing;
using System.Windows.Input;

namespace ImageProcessing.View.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        public ICommand Open { get; set; }

        public ResultsViewModel ResultsViewModel { get; set; }
        public BasicViewModel BasicViewModel { get; set; }
        public ComplexViewModel ComplexViewModel { get; set; }
        public RegionGrowingViewModel RegionGrowingViewModel { get; set; }


        public ICommand OpenEvalWindow { get; set; }

        public bool IsComplexView { get; set; }
        public bool IsRegionGrowingView { get; set; }

        public MainWindowViewModel()
        {
            ResultsViewModel = new ResultsViewModel();
            BasicViewModel = new BasicViewModel(ResultsViewModel);
            ComplexViewModel = new ComplexViewModel(ResultsViewModel);
            RegionGrowingViewModel = new RegionGrowingViewModel();

            Open = new RelayCommand(Load);
            OpenEvalWindow = new RelayCommand(OnEvalWindowOpen);
        }

        private void Load()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.bmp; *.png, *.tif)|*.bmp; *.png; *.tif",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var bitmap = new Bitmap(openFileDialog.FileName);

                RegionGrowingViewModel.LoadOriginalImage(bitmap);
                ResultsViewModel.LoadImage(bitmap);
            }
        }

        private void OnEvalWindowOpen()
        {
            EvaluationWindowVM vm = new EvaluationWindowVM();
            EvaluationWindow window = new EvaluationWindow
            {
                DataContext = vm
            };
            window.Show();
        }
    }
}
