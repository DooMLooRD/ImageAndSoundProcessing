using ImageProcessing.Core.BasicImageOperations;
using ImageProcessing.View.ViewModel;
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
            DataContext = new MainWindowViewModel();
            InitializeComponent();
        }
    }
}
