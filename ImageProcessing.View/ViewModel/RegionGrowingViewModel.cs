using ImageProcessing.Core.Segmentation;
using ImageProcessing.View.Helpers;
using ImageProcessing.View.ViewModel.Base;
using ImageProcessing.View.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace ImageProcessing.View.ViewModel
{
    public class RegionGrowingViewModel : BaseViewModel
    {
        public int Threshold { get; set; } = 50;
        public int MinPixelNumber { get; set; } = 400;

        public ICommand SetMaskFolderCommand { get; set; }
        public ICommand ApplyOperationCommand { get; set; }
        public ICommand SetMaskCommand { get; set; }
        public ICommand EnlargeOriginalImage { get; set; }
        public ICommand EnlargeSegmentedImage { get; set; }
        public ICommand EnlargeImageWithMask { get; set; }

        public ImageSource OriginalImage { get; set; }
        public Bitmap OriginalBitmap { get; set; }

        public ImageSource SegmentedImage { get; set; }
        public Bitmap SegmentedBitmap { get; set; }

        public ImageSource ImageWithMask { get; set; }
        public Bitmap ImageWithMaskBitmap { get; set; }

        public ObservableCollection<MaskVM> Masks { get; set; }

        public string SaveFolderPath { get; set; }

        public RegionGrowingViewModel()
        {
            SetMaskFolderCommand = new RelayCommand(SetMaskFolder);
            ApplyOperationCommand = new RelayCommand(ApplyOperation);
            SetMaskCommand = new RelayCommand<Bitmap>(SetMask);
            EnlargeOriginalImage = new RelayCommand(() => ShowImageInFullWindow(OriginalImage, OriginalBitmap));
            EnlargeSegmentedImage = new RelayCommand(() => ShowImageInFullWindow(SegmentedImage, SegmentedBitmap));
            EnlargeImageWithMask = new RelayCommand(() => ShowImageInFullWindow(ImageWithMask, ImageWithMaskBitmap));
        }

        public void LoadOriginalImage(Bitmap bitmap)
        {
            OriginalBitmap = bitmap;
            OriginalImage = ImageGdiHelper.LoadBitmap(bitmap);
        }

        private void ShowImageInFullWindow(ImageSource imageSource, Bitmap bitmap)
        {
            EnlargedImageWindowVM vm = new EnlargedImageWindowVM(imageSource, bitmap);
            EnlargedImageWindow window = new EnlargedImageWindow()
            {
                DataContext = vm
            };
            window.Show();
        }

        private void SetMask(Bitmap bitmap)
        {
            var regionGrowing = new RegionGrowing();
            var result = regionGrowing.SetMask(OriginalBitmap, bitmap);
            ImageWithMaskBitmap = result;
            ImageWithMask = ImageGdiHelper.LoadBitmap(result);
        }

        private async void ApplyOperation()
        {
            var regionGrowing = new RegionGrowing();

            var results = new List<Bitmap>();

            await Task.Run(() =>
            {
                results = regionGrowing.CreateMasks(OriginalBitmap, Threshold, MinPixelNumber, SaveFolderPath);
            });

            SegmentedBitmap = results[0];
            SegmentedImage = ImageGdiHelper.LoadBitmap(results[0]);

            Masks = new ObservableCollection<MaskVM>();

            foreach (var mask in results.Skip(1))
            {
                Masks.Add(new MaskVM { Image = ImageGdiHelper.LoadBitmap(mask), Bitmap = mask });
            }
        }

        private void SetMaskFolder()
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                SaveFolderPath = folderDialog.SelectedPath;
            }
        }
    }
}
