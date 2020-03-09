using ImageProcessing.Core.Helpers;
using ImageProcessing.Core.Interfaces;
using System.Collections.Generic;
using System.Drawing;

namespace ImageProcessing
{
    public class ImageProcessor
    {
        public static unsafe void ProcessImage(Bitmap bitmap, IProcessingOperation processBitmap, string fileName)
        {
            Bitmap resultBitmap;
            using (var bitmapData = new CustomBitmapData(bitmap, true))
            {
                resultBitmap = bitmapData.CopyBitmap;
                for (int y = 1; y < bitmapData.HeightInPixels - 1; y++)
                {
                    for (int x = bitmapData.BytesPerPixel; x < bitmapData.WidthInBytes - bitmapData.BytesPerPixel; x = x + bitmapData.BytesPerPixel)
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
