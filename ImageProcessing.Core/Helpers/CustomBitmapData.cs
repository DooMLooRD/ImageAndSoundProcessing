using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageProcessing.Core.Helpers
{
    public unsafe class CustomBitmapData : IDisposable
    {
        public BitmapData OriginalBitmapData { get; set; }
        public BitmapData CopyBitmapData { get; set; }
        public Bitmap OriginalBitmap { get; set; }
        public Bitmap CopyBitmap { get; set; }

        public int BytesPerPixel { get; set; }
        public int HeightInPixels { get; set; }
        public int WidthInBytes { get; set; }

        public byte* FirstPixelPtr { get; set; }
        public byte* FirstPixelPtrCopy { get; set; }

        public bool CreateCopy { get; set; }

        public CustomBitmapData(Bitmap bitmap, bool createCopy)
        {
            CreateCopy = createCopy;
            OriginalBitmap = bitmap;

            if (OriginalBitmap.PixelFormat == PixelFormat.Format8bppIndexed || OriginalBitmap.PixelFormat == PixelFormat.Format1bppIndexed)
            {
                ImageHelper.ConvertToPixelFormat(bitmap, out Bitmap result, PixelFormat.Format24bppRgb);
                OriginalBitmap = result;
            }

            OriginalBitmapData = OriginalBitmap.LockBits(new Rectangle(0, 0, OriginalBitmap.Width, OriginalBitmap.Height), ImageLockMode.ReadWrite, OriginalBitmap.PixelFormat);
            BytesPerPixel = Image.GetPixelFormatSize(OriginalBitmapData.PixelFormat) / 8;
            HeightInPixels = OriginalBitmapData.Height;
            WidthInBytes = OriginalBitmapData.Width * BytesPerPixel;
            FirstPixelPtr = (byte*)OriginalBitmapData.Scan0;

            if (CreateCopy)
            {
                CopyBitmap = new Bitmap(OriginalBitmap.Width, OriginalBitmap.Height, OriginalBitmap.PixelFormat);
                CopyBitmapData = CopyBitmap.LockBits(new Rectangle(0, 0, OriginalBitmap.Width, OriginalBitmap.Height), ImageLockMode.ReadWrite, OriginalBitmap.PixelFormat);
                FirstPixelPtrCopy = (byte*)CopyBitmapData.Scan0;
            }
        }

        public void Dispose()
        {
            OriginalBitmap.UnlockBits(OriginalBitmapData);

            if (CreateCopy)
            {
                CopyBitmap.UnlockBits(CopyBitmapData);
            }
        }
    }
}
