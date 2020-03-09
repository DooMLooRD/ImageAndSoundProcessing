using ImageProcessing.Core.BasicFilters;
using ImageProcessing.Core.NonLinearFilters;
using ImageProcessing.Core.BasicImageOperations;

using System.Drawing;
using System.Windows;

namespace ImageProcessing.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Bitmap bitmap = new Bitmap("test2.jpg");
            ImageProcessor.ProcessImage(bitmap, new AverageFilter(3), "result.jpg", 3);
            InitializeComponent();
        }
    }
}
