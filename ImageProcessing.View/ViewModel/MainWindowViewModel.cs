using ImageProcessing.View.ViewModel.Base;
using ImageProcessing.View.Windows;
using System.Windows.Input;

namespace ImageProcessing.View.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        public ICommand Open { get; set; }

        public ResultsViewModel ResultsViewModel { get; set; }
        public BasicViewModel BasicViewModel { get; set; }

        public ICommand OpenEvalWindow { get; set; }
        public ICommand Save { get; set; }

        public bool IsComplexView { get; set; }

        public MainWindowViewModel()
        {
            ResultsViewModel = new ResultsViewModel();
            BasicViewModel= new BasicViewModel(ResultsViewModel);

            Open = new RelayCommand(ResultsViewModel.LoadImage);
            OpenEvalWindow = new RelayCommand(OnEvalWindowOpen);
            Save = new RelayCommand(ResultsViewModel.SaveResult);
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
