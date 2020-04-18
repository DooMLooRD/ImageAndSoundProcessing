using ImageProcessing.View.ViewModel.Base;
using Microsoft.Win32;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media;

namespace ImageProcessing.View.ViewModel
{
    public class EnlargedImageWindowVM : BaseViewModel
    {
        public ImageSource Image { get; set; }
        public Bitmap Bitmap { get; set; }
        public ICommand SaveToFileCommand { get; set; }

        public EnlargedImageWindowVM(ImageSource imageToShow, Bitmap bitmap)
        {
            Image = imageToShow;
            Bitmap = bitmap;
            SaveToFileCommand = new RelayCommand(SaveToFile);
        }

        private void SaveToFile()
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
                Bitmap.Save(sfd.FileName);
            }
        }
    }
}