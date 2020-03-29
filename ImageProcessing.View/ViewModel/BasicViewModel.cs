using ImageProcessing.Core.BasicFilters;
using ImageProcessing.Core.BasicImageOperations;
using ImageProcessing.Core.Constants;
using ImageProcessing.Core.HistogramOperations;
using ImageProcessing.Core.Interfaces;
using ImageProcessing.Core.LinearFilters;
using ImageProcessing.Core.NonLinearFilters;
using ImageProcessing.View.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ImageProcessing.View.ViewModel
{
    public class BasicViewModel : BaseViewModel
    {
        public IEnumerable<string> Operations { get; set; }

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

        public int WindowSize { get; set; } = 3;
        public double ContrastFactor { get; set; } = 10;
        public int BrightnessFactor { get; set; } = 100;
        public int UolisFactor { get; set; } = 1000;

        public int A_Min { get; set; }
        public int A_Max { get; set; }
        public int R_Min { get; set; }
        public int R_Max { get; set; }
        public int G_Min { get; set; }
        public int G_Max { get; set; }
        public int B_Min { get; set; }
        public int B_Max { get; set; }

        public bool ConvertResultToGreyscale { get; set; } = false;

        public int MaskSize { get; set; }
        public ICommand SetMaskTabCommand { get; set; }
        public DataView Mask { get; set; }

        public Visibility WindowSizeVisible { get; set; } = new Visibility();
        public Visibility BrightnessFactorVisible { get; set; } = new Visibility();
        public Visibility ContrastFactorVisible { get; set; } = new Visibility();
        public Visibility MaskVisible { get; set; } = new Visibility();
        public Visibility UolisNormalizationVisible { get; set; } = new Visibility();
        public Visibility HistogramFactorsVisible { get; set; } = new Visibility();
        public Visibility HistogramSeparateFactorsVisible { get; set; } = new Visibility();

        public Visibility[] VisibilityProps;

        public ResultsViewModel ResultsViewModel { get; set; }

        public BasicViewModel(ResultsViewModel resultsViewModel)
        {
            ResultsViewModel = resultsViewModel;

            MaskSize = 3;
            A_Min = R_Min = G_Min = B_Min = 0;
            A_Max = R_Max = G_Max = B_Max = 255;

            SetMaskTab();
            ApplyOperationCommand = new RelayCommand(ApplyOperation);
            SetMaskTabCommand = new RelayCommand(SetMaskTab);
            Operations = new[] {
                Algorithms.Greyscale,
                Algorithms.Negative,
                Algorithms.Brightness,
                Algorithms.Contrast,
                Algorithms.AverageFilter,
                Algorithms.LinearFilter,
                Algorithms.MedianFilter,
                Algorithms.SobelOperator,
                Algorithms.UolisOperator,
                Algorithms.UniformProbabilityDensity
            };

            SelectedOperation = Algorithms.Greyscale;

            VisibilityProps = new[] {
                WindowSizeVisible,
                BrightnessFactorVisible,
                ContrastFactorVisible,
                MaskVisible,
                UolisNormalizationVisible,
                HistogramFactorsVisible
            };
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
                case Algorithms.MedianFilter:
                case Algorithms.AverageFilter:
                    WindowSizeVisible.Visible = true;
                    break;
                case Algorithms.Brightness:
                    BrightnessFactorVisible.Visible = true;
                    break;
                case Algorithms.Contrast:
                    ContrastFactorVisible.Visible = true;
                    break;
                case Algorithms.LinearFilter:
                    MaskVisible.Visible = true;
                    break;
                case Algorithms.UolisOperator:
                    UolisNormalizationVisible.Visible = true;
                    break;
                case Algorithms.UniformProbabilityDensity:
                    HistogramFactorsVisible.Visible = true;
                    break;
            }
        }

        public void SetMaskTab()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("#", Type.GetType("System.Int32"));

            for (int i = 0; i < MaskSize; i++)
            {
                dataTable.Columns.Add($"{i + 1}", Type.GetType("System.Int32"));
            }

            for (int i = 0; i < MaskSize; i++)
            {
                var row = dataTable.NewRow();
                row["#"] = i + 1;
                for (int j = 0; j < MaskSize; j++)
                {
                    row[$"{j + 1}"] = 0;
                }
                dataTable.Rows.Add(row);
            }
            Mask = dataTable.AsDataView();
        }

        public int[] ReadMask()
        {
            var tab = new int[MaskSize * MaskSize];
            var rows = Mask.Table.Rows;
            for (int i = 0; i < MaskSize; i++)
            {
                for (int j = 0; j < MaskSize; j++)
                {
                    tab[i * MaskSize + j] = (int)rows[i][j + 1];
                }
            }
            return tab;
        }

        public async void ApplyOperation()
        {

            IProcessingOperation operation;
            var offset = 0;
            switch (SelectedOperation)
            {
                case Algorithms.Greyscale:
                    operation = new GreyscaleOperation();
                    break;
                case Algorithms.Negative:
                    operation = new NegativeOperation();
                    break;
                case Algorithms.Brightness:
                    operation = new BrightnessOperation(BrightnessFactor);
                    break;
                case Algorithms.Contrast:
                    operation = new ContrastOperation(ContrastFactor);
                    break;
                case Algorithms.AverageFilter:
                    operation = new AverageFilter(WindowSize);
                    offset = WindowSize / 2;
                    break;
                case Algorithms.LinearFilter:
                    operation = new LinearFilter(ReadMask(), MaskSize);
                    offset = MaskSize / 2;
                    break;
                case Algorithms.MedianFilter:
                    operation = new MedianFilter(WindowSize);
                    offset = WindowSize / 2;
                    break;
                case Algorithms.SobelOperator:
                    operation = new SobelOperator();
                    offset = 1;
                    break;
                case Algorithms.UolisOperator:
                    operation = new UolisOperator(UolisFactor);
                    offset = 1;
                    break;
                case Algorithms.UniformProbabilityDensity:
                    if (ResultsViewModel.UseSeparateCanals)
                    {
                        operation = new UniformProbabilityDensity(ResultsViewModel.OriginalResult.Histogram.Values, R_Min, R_Max, G_Min, G_Max, B_Min, B_Max);
                    }
                    else
                    {
                        operation = new UniformProbabilityDensity(ResultsViewModel.OriginalResult.Histogram.Values, A_Min, A_Max);
                    }
                    break;
                default:
                    operation = new NegativeOperation();
                    break;
            }

            System.Drawing.Bitmap bitmap = null;
            await Task.Run(() =>
            {
                bitmap = ImageProcessor.ProcessImage(ResultsViewModel.OriginalResult.ImageComponents.Image, operation, offset);

                if (ConvertResultToGreyscale)
                {
                    bitmap = ImageProcessor.ProcessImage(bitmap, new GreyscaleOperation());
                }
            });

            ResultsViewModel.ProcessedResult.LoadData(bitmap);
        }
    }
}
