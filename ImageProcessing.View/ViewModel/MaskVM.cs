using ImageProcessing.View.ViewModel.Base;
using System.Drawing;
using System.Windows.Media;

namespace ImageProcessing.View.ViewModel
{
    public class MaskVM : BaseViewModel
    {
        public ImageSource Image { get; set; }
        public Bitmap Bitmap { get; set; }
    }
}
