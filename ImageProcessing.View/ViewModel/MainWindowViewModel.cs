using ImageProcessing.Core.BasicFilters;
using ImageProcessing.Core.BasicImageOperations;
using ImageProcessing.Core.Constants;
using ImageProcessing.Core.Helpers;
using ImageProcessing.Core.HistogramOperations;
using ImageProcessing.Core.Interfaces;
using ImageProcessing.Core.LinearFilters;
using ImageProcessing.Core.NonLinearFilters;
using ImageProcessing.View.ViewModel.Base;
using ImageProcessing.View.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageProcessing.View.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        public System.Drawing.Bitmap OriginalBitmap { get; set; }
        public System.Drawing.Bitmap ResultBitmap { get; set; }

        public ImageSource OriginalImage { get; set; }

        public ImageSource ResultImage { get; set; }

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

        public ICommand Open { get; set; }
        public ICommand ApplyOperationCommand { get; set; }
        public ICommand EnlargeOriginalImage { get; set; }
        public ICommand EnlargeResultImage { get; set; }

        public int WindowSize { get; set; } = 3;
        public double ContrastFactor { get; set; } = 10;
        public int BrightnessFactor { get; set; } = 100;
        public int UolisFactor { get; set; } = 1000;

        public bool UseSeparateCanals { get; set; }
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

        public Histogram OriginalHistogram { get; set; }

        public Histogram ResultHistogram { get; set; }

        public bool IsHistogramShown { get; set; }
        public ICommand ShowHistogram { get; set; }

        public HistogramVM OriginalHistogramVm { get; set; }
        public HistogramVM ResultHistogramVm { get; set; }

        public Visibility WindowSizeVisible { get; set; } = new Visibility();
        public Visibility BrightnessFactorVisible { get; set; } = new Visibility();
        public Visibility ContrastFactorVisible { get; set; } = new Visibility();
        public Visibility MaskVisible { get; set; } = new Visibility();
        public Visibility UolisNormalizationVisible { get; set; } = new Visibility();
        public Visibility HistogramFactorsVisible { get; set; } = new Visibility();
        public Visibility HistogramSeparateFactorsVisible { get; set; } = new Visibility();

        public Visibility[] VisibilityProps;

        public ICommand OpenEvalWindow { get; set; }
        public ICommand Save { get; set; }


        public MainWindowViewModel()
        {
            MaskSize = 3;
            A_Min = R_Min = G_Min = B_Min = 0;
            A_Max = R_Max = G_Max = B_Max = 255;

            SetMaskTab();

            Open = new RelayCommand(LoadImage);
            ApplyOperationCommand = new RelayCommand(ApplyOperation);
            EnlargeOriginalImage = new RelayCommand(() => ShowImageInFullWindow(OriginalImage));
            EnlargeResultImage = new RelayCommand(() => ShowImageInFullWindow(ResultImage));
            SetMaskTabCommand = new RelayCommand(SetMaskTab);
            OpenEvalWindow = new RelayCommand(OnEvalWindowOpen);
            Save = new RelayCommand(SaveResult);

            OriginalHistogramVm = new HistogramVM(OriginalHistogram);
            ResultHistogramVm = new HistogramVM(ResultHistogram);

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

        private void SaveResult()
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
                ResultBitmap.Save(sfd.FileName);
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

        private void OnEvalWindowOpen()
        {
            EvaluationWindowVM vm = new EvaluationWindowVM();
            EvaluationWindow window = new EvaluationWindow
            {
                DataContext = vm
            };
            window.Show();
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

        public async void LoadImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog.Filter = "Image files (*.jpg; *.bmp; *.png)|*.jpg; *.bmp; *.png";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                OriginalBitmap = new System.Drawing.Bitmap(openFileDialog.FileName);

                HistogramSeparateFactorsVisible.Visible = !ImageHelper.IsGreyscale(OriginalBitmap.PixelFormat);

                UseSeparateCanals = ImageHelper.IsGreyscale(OriginalBitmap.PixelFormat) ? false : UseSeparateCanals;
                await Task.Run(() =>
                {
                    OriginalHistogram = ImageProcessor.CreateHistogram(OriginalBitmap);
                });

                OriginalHistogramVm = new HistogramVM(OriginalHistogram);
                OriginalImage = LoadBitmap(OriginalBitmap);
            }
        }

        public void ShowImageInFullWindow(ImageSource imageToShow)
        {
            EnlargedImageWindowVM vm = new EnlargedImageWindowVM(imageToShow);
            EnlargedImageWindow window = new EnlargedImageWindow()
            {
                DataContext = vm
            };
            window.Show();
        }

        public async void ApplyOperation()
        {
            await Task.Run(() =>
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
                        if (UseSeparateCanals)
                        {
                            operation = new UniformProbabilityDensity(OriginalHistogram.Values, R_Min, R_Max, G_Min, G_Max, B_Min, B_Max);
                        }
                        else
                        {
                            operation = new UniformProbabilityDensity(OriginalHistogram.Values, A_Min, A_Max);
                        }
                        break;
                    default:
                        operation = new NegativeOperation();
                        break;
                }

                ResultBitmap = ImageProcessor.ProcessImage(OriginalBitmap, operation, offset);

                ResultHistogram = ImageProcessor.CreateHistogram(ResultBitmap);

                if (ConvertResultToGreyscale)
                {
                    ResultBitmap = ImageProcessor.ProcessImage(ResultBitmap, new GreyscaleOperation());
                }
            });

            ResultImage = LoadBitmap(ResultBitmap);
            ResultHistogramVm = new HistogramVM(ResultHistogram);
        }


        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);

        public static BitmapSource LoadBitmap(System.Drawing.Bitmap source)
        {
            IntPtr ip = source.GetHbitmap();
            BitmapSource bs = null;
            try
            {
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                   IntPtr.Zero, Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ip);
            }

            return bs;
        }
    }
}
