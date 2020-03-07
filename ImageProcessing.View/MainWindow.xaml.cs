using ImageProcessing.Core.NonLinearFilters;
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
            Bitmap bitmap = new Bitmap("test.jpg");
            ImageProcessor.ProcessImage(bitmap, new UolisOperator(10000), "result.jpg");
            InitializeComponent();
        }
    }
}
