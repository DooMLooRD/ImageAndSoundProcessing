using ImageProcessing.Core.Helpers;
using ImageProcessing.Core.Interfaces;
using System.Drawing;

namespace ImageProcessing
{
    public class ImageProcessor
    {

        public static unsafe Bitmap ProcessImage(Bitmap bitmap, IProcessingOperation processBitmap, int offset = 0)
        {
            Bitmap resultBitmap;
            using (var bitmapData = new CustomBitmapData(bitmap, true))
            {
                resultBitmap = bitmapData.CopyBitmap;
                for (int y = offset; y < bitmapData.HeightInPixels - offset; y++)
                {
                    for (int x = offset * bitmapData.BytesPerPixel; x < bitmapData.WidthInBytes - (offset * bitmapData.BytesPerPixel); x = x + bitmapData.BytesPerPixel)
                    {
                        processBitmap.ProcessPixel(bitmapData.FirstPixelPtr, x, y, bitmapData.OriginalBitmapData.Stride, bitmapData.BytesPerPixel, bitmapData.FirstPixelPtrCopy);
                    }
                }

            }

            return resultBitmap;
        }

        //public unsafe IEnumerable<(int, ColorValues)> CreateHistogram()
        //{

        //}
    }
}
