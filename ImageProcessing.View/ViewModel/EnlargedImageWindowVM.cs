using ImageProcessing.View.ViewModel.Base;
using System.Windows.Media;

namespace ImageProcessing.View.ViewModel
{
    public class EnlargedImageWindowVM : BaseViewModel
    {
        public ImageSource Image { get; set; }
        public EnlargedImageWindowVM(ImageSource imageToShow)
        {
            Image = imageToShow;
        }
    }
}
