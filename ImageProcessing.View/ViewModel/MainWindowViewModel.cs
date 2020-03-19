using ImageProcessing.Core.BasicFilters;
using ImageProcessing.Core.BasicImageOperations;
using ImageProcessing.Core.Constants;
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
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageProcessing.View.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        public System.Drawing.Bitmap OriginalBitmap { get; set; }
        public ImageSource OriginalImage { get; set; }
        //public ImageSource LargeImage { get; set; }

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
        public bool ConvertResultToGreyscale { get; set; } = false;

        public int MaskSize { get; set; }
        public ICommand SetMaskTabCommand { get; set; }
        public DataView Mask { get; set; }

        public Visibility WindowSizeVisible { get; set; } = new Visibility();
        public Visibility BrightnessFactorVisible { get; set; } = new Visibility();
        public Visibility ContrastFactorVisible { get; set; } = new Visibility();
        public Visibility MaskVisible { get; set; } = new Visibility();
        public Visibility UolisNormalizationVisible { get; set; } = new Visibility();

        public Visibility[] VisibilityProps;

        private const string SobelOperator = "Sobel Operator";

        public MainWindowViewModel()
        {
            MaskSize = 3;
            SetMaskTab();
            Open = new RelayCommand(LoadImage);
            ApplyOperationCommand = new RelayCommand(ApplyOperation);
            EnlargeOriginalImage = new RelayCommand(() => ShowImageInFullWindow(OriginalImage));
            EnlargeResultImage = new RelayCommand(() => ShowImageInFullWindow(ResultImage));
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
                Algorithms.UolisOperator };
            SelectedOperation = Algorithms.Greyscale;
            VisibilityProps = new[] { WindowSizeVisible, BrightnessFactorVisible, ContrastFactorVisible, MaskVisible, UolisNormalizationVisible };
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

        public void LoadImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog.Filter = "Image files (*.jpg)|*.jpg";
            if (openFileDialog.ShowDialog() == true)
            {
                OriginalBitmap = new System.Drawing.Bitmap(openFileDialog.FileName);
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

        public void ApplyOperation()
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
                default:
                    operation = new NegativeOperation();
                    break;
            }
            var result = ImageProcessor.ProcessImage(OriginalBitmap, operation, offset);

            if (ConvertResultToGreyscale)
            {
                result = ImageProcessor.ProcessImage(result, new GreyscaleOperation());
            }

            ResultImage = LoadBitmap(result);
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
