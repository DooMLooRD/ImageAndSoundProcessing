using ImageProcessing.Core.Interfaces;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageProcessing
{
    public class ImageProcessor
    {
        public static unsafe void ProcessImage(Bitmap bitmap, IProcessingOperation processBitmap, string fileName)
        {
            var result = new Bitmap(bitmap.Width, bitmap.Height);

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            BitmapData bitmapDataCopy = result.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            int bytesPerPixel = Image.GetPixelFormatSize(bitmapData.PixelFormat) / 8;
            int heightInPixels = bitmapData.Height;
            int widthInBytes = bitmapData.Width * bytesPerPixel;

            byte* firstPixelPtr = (byte*)bitmapData.Scan0;
            byte* firstPixelPtrCopy = (byte*)bitmapDataCopy.Scan0;

            for (int y = 1; y < heightInPixels - 1; y++)
            {
                for (int x = bytesPerPixel; x < widthInBytes - bytesPerPixel; x = x + bytesPerPixel)
                {
                    processBitmap.ProcessPixel(firstPixelPtr, x, y, bitmapData.Stride, bytesPerPixel, firstPixelPtrCopy);
                }
            }

            bitmap.UnlockBits(bitmapData);
            result.UnlockBits(bitmapDataCopy);

            result.Save(fileName);
        }
    }
}
