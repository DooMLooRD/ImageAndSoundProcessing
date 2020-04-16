using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageProcessing.Core.Model
{
    public unsafe class SingleBitmapData : IDisposable
    {
        public BitmapData BitmapData { get; set; }
        public Bitmap Bitmap { get; set; }

        public int BytesPerPixel { get; set; }
        public int HeightInPixels { get; set; }
        public int WidthInBytes { get; set; }

        public byte* FirstPixelPtr { get; set; }

        public SingleBitmapData(Bitmap bitmap, bool cloneBitmap)
        {
            Bitmap = cloneBitmap ? bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), bitmap.PixelFormat) : bitmap;

            BitmapData = Bitmap.LockBits(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height),
                ImageLockMode.ReadWrite, Bitmap.PixelFormat);
            BytesPerPixel = Image.GetPixelFormatSize(Bitmap.PixelFormat) / 8;
            HeightInPixels = BitmapData.Height;
            WidthInBytes = BitmapData.Width * BytesPerPixel;
            FirstPixelPtr = (byte*)BitmapData.Scan0;
        }

        public SingleBitmapData(Bitmap bitmap, PixelFormat pixelFormat)
        {
            Bitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), pixelFormat);

            BitmapData = Bitmap.LockBits(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height),
                ImageLockMode.ReadWrite, Bitmap.PixelFormat);
            BytesPerPixel = Image.GetPixelFormatSize(Bitmap.PixelFormat) / 8;
            HeightInPixels = BitmapData.Height;
            WidthInBytes = BitmapData.Width * BytesPerPixel;
            FirstPixelPtr = (byte*)BitmapData.Scan0;
        }


        public void Dispose()
        {
            Bitmap.UnlockBits(BitmapData);
        }
    }
}
