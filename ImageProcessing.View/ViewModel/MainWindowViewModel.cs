using ImageProcessing.Core.BasicFilters;
using ImageProcessing.Core.BasicImageOperations;
using ImageProcessing.Core.Interfaces;
using ImageProcessing.Core.NonLinearFilters;
using ImageProcessing.View.ViewModel.Base;
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
        public ImageSource ResultImage { get; set; }

        public IEnumerable<string> Operations { get; set; }
        public string SelectedOperation { get; set; }

        public ICommand LoadFileCommand { get; set; }
        public ICommand ApplyOperationCommand { get; set; }

        public int WindowSize { get; set; } = 3;
        public double ContrastFactor { get; set; } = 10;
        public int BrightnessFactor { get; set; } = 100;
        public int UolisFactor { get; set; } = 1000;

        public int MaskX { get; set; }
        public int MaskY { get; set; }
        public ICommand SetMaskTabCommand { get; set; }
        public DataView Mask { get; set; }

        public MainWindowViewModel()
        {
            MaskX = 3;
            MaskY = 3;
            SetMaskTab();
            LoadFileCommand = new RelayCommand(LoadImage);
            ApplyOperationCommand = new RelayCommand(ApplyOperation);
            SetMaskTabCommand = new RelayCommand(SetMaskTab);
            Operations = new[] { "Negative", "Brightness (**)", "Contrast (***)", "Average Filter (*)", "Median Filter (*)", "Sobel Operator", "Uolis Operator (****)" };
            SelectedOperation = "Negative";
        }

        public void SetMaskTab()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("#", Type.GetType("System.Int32"));

            for (int i = 0; i < MaskX; i++)
            {
                dataTable.Columns.Add($"{i + 1}", Type.GetType("System.Int32"));
            }

            for (int i = 0; i < MaskY; i++)
            {
                var row = dataTable.NewRow();
                row["#"] = i + 1;
                for (int j = 0; j < MaskX; j++)
                {
                    row[$"{j + 1}"] = 0;
                }
                dataTable.Rows.Add(row);
            }
            Mask = dataTable.AsDataView();
        }

        public int[][] ReadMask()
        {
            var tab = new int[MaskY][];
            var rows = Mask.Table.Rows;
            for (int i = 0; i < MaskY; i++)
            {
                tab[i] = new int[MaskX];
                for (int j = 0; j < MaskX; j++)
                {
                    tab[i][j] = (int)rows[i][j + 1];
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

        public void ApplyOperation()
        {
            IProcessingOperation operation;
            var offset = 0;
            switch (SelectedOperation)
            {
                case "Negative":
                    operation = new NegativeOperation();
                    break;
                case "Brightness (**)":
                    operation = new BrightnessOperation(BrightnessFactor);
                    break;
                case "Contrast (***)":
                    operation = new ContrastOperation(ContrastFactor);
                    break;
                case "Average Filter (*)":
                    operation = new AverageFilter(WindowSize);
                    offset = WindowSize / 2;
                    break;
                case "Median Filter (*)":
                    operation = new MedianFilter(WindowSize);
                    offset = WindowSize / 2;
                    break;
                case "Sobel Operator":
                    operation = new SobelOperator();
                    offset = 1;
                    break;
                case "Uolis Operator (****)":
                    operation = new UolisOperator(UolisFactor);
                    offset = 1;
                    break;
                default:
                    operation = new NegativeOperation();
                    break;
            }
            var result = ImageProcessor.ProcessImage(OriginalBitmap, operation, offset);
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
