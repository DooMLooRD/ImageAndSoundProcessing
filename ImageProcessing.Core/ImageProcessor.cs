using ImageProcessing.Core.Helpers;
using ImageProcessing.Core.Interfaces;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageProcessing
{
    public class ImageProcessor
    {
        public static unsafe void ProcessImage(Bitmap bitmap, IProcessingOperation processBitmap, string fileName, int windowSize)
        {
            int offset = windowSize / 2;
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

            resultBitmap.Save(fileName);
        }

        //public unsafe IEnumerable<(int, ColorValues)> CreateHistogram()
        //{

        //}
    }
}
