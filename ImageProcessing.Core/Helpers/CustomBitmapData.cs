using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

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

            OriginalBitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            BytesPerPixel = Image.GetPixelFormatSize(OriginalBitmapData.PixelFormat) / 8;
            HeightInPixels = OriginalBitmapData.Height;
            WidthInBytes = OriginalBitmapData.Width * BytesPerPixel;
            FirstPixelPtr = (byte*)OriginalBitmapData.Scan0;


            if (CreateCopy)
            {
                CopyBitmap = new Bitmap(bitmap.Width, bitmap.Height, bitmap.PixelFormat);
                CopyBitmapData = CopyBitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
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
