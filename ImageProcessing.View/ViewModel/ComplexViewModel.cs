using ImageProcessing.Core.Constants;
using ImageProcessing.Core.FourierOperations;
using ImageProcessing.Core.Interfaces;
using ImageProcessing.Core.Model;
using ImageProcessing.View.ViewModel.Base;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ImageProcessing.View.ViewModel
{
    public class ComplexViewModel : BaseViewModel
    {
        public IEnumerable<string> FourierOperations { get; set; }

        private string _selectedOperation;

        public string SelectedOperation
        {
            get => _selectedOperation;
            set
            {
                _selectedOperation = value;
                SetVisibility(value);
            }
        }

        public ICommand ApplyOperationCommand { get; set; }

        public int FilterRadius { get; set; } = 50;
        public int FilterSize { get; set; } = 50;
        public int K { get; set; } = 10;
        public int L { get; set; } = 10;
        public int FilterAngle { get; set; } = 10;
        public int FilterAngleOffset { get; set; } = 10;

        public Visibility FilterRadiusVisible { get; set; } = new Visibility();
        public Visibility FilterSizeVisible { get; set; } = new Visibility();
        public Visibility KVisible { get; set; } = new Visibility();
        public Visibility LVisible { get; set; } = new Visibility();
        public Visibility FilterAngleVisible { get; set; } = new Visibility();
        public Visibility FilterAngleOffsetVisible { get; set; } = new Visibility();

        public Visibility[] VisibilityProps;

        public ResultsViewModel ResultsViewModel { get; set; }

        public ComplexViewModel(ResultsViewModel resultsViewModel)
        {
            ResultsViewModel = resultsViewModel;

            ApplyOperationCommand = new RelayCommand(ApplyOperation);
            FourierOperations = new[] {
                FourierAlgorithms.LowPassFilter,
                FourierAlgorithms.HighPassFilter,
                FourierAlgorithms.BandPassFilter,
                FourierAlgorithms.BandStopFilter,
                FourierAlgorithms.SpectrumFilter,
                FourierAlgorithms.EdgeDetectionFilter,
            };

            VisibilityProps = new[] {
                FilterRadiusVisible,
                FilterSizeVisible,
                KVisible,
                LVisible,
                FilterAngleVisible,
                FilterAngleOffsetVisible,
            };

            SelectedOperation = FourierAlgorithms.LowPassFilter;
        }

        public void SetVisibility(string operation)
        {
            if (VisibilityProps == null)
            {
                return;
            }
            foreach (var visibilityProp in VisibilityProps)
            {
                visibilityProp.Visible = false;
            }

            switch (operation)
            {
                case FourierAlgorithms.LowPassFilter:
                case FourierAlgorithms.HighPassFilter:
                    FilterRadiusVisible.Visible = true;
                    break;
                case FourierAlgorithms.BandPassFilter:
                case FourierAlgorithms.BandStopFilter:
                    FilterRadiusVisible.Visible = true;
                    FilterSizeVisible.Visible = true;
                    break;
                case FourierAlgorithms.SpectrumFilter:
                    LVisible.Visible = true;
                    KVisible.Visible = true;
                    break;
                case FourierAlgorithms.EdgeDetectionFilter:
                    FilterRadiusVisible.Visible = true;
                    FilterAngleVisible.Visible = true;
                    FilterAngleOffsetVisible.Visible = true;
                    break;
            }
        }

        public async void ApplyOperation()
        {
            IProcessingFourierOperation operation;
            switch (SelectedOperation)
            {
                case FourierAlgorithms.LowPassFilter:
                    operation = new LowPassFilter(FilterRadius);
                    break;
                case FourierAlgorithms.HighPassFilter:
                    operation = new HighPassFilter(FilterRadius);
                    break;
                case FourierAlgorithms.BandPassFilter:
                    operation = new BandPassFilter(FilterRadius, FilterSize);
                    break;
                case FourierAlgorithms.BandStopFilter:
                    operation = new BandStopFilter(FilterRadius, FilterSize);
                    break;
                case FourierAlgorithms.SpectrumFilter:
                    operation = new SpectrumFilter(L, K);
                    break;
                case FourierAlgorithms.EdgeDetectionFilter:
                    operation = new EdgeDetectionFilter(FilterAngle, FilterAngleOffset, FilterRadius);
                    break;
                default:
                    operation = new LowPassFilter(FilterRadius);
                    break;
            }

            ImageComponents imageComponents = null;
            await Task.Run(() =>
            {
                imageComponents = ImageProcessor.ProcessFourierImage(ResultsViewModel.OriginalResult.ImageComponents, operation);
            });

            ResultsViewModel.ProcessedResult.LoadData(imageComponents);
        }
    }
}
