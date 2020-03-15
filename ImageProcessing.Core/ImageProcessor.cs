using ImageProcessing.Core.Helpers;
using ImageProcessing.Core.Interfaces;
using System.Drawing;

namespace ImageProcessing
{
    public class ImageProcessor
    {

        public static unsafe Bitmap ProcessImage(
            Bitmap bitmap,
            IProcessingOperation processBitmap,
            int offset = 0)
        {
            Bitmap resultBitmap;
            using (var bitmapData = new CustomBitmapData(bitmap, true))
            {
                resultBitmap = bitmapData.CopyBitmap;
                for (int y = offset; y < bitmapData.HeightInPixels - offset; y++)
                {
                    for (int x = offset * bitmapData.BytesPerPixel;
                        x < bitmapData.WidthInBytes - (offset * bitmapData.BytesPerPixel);
                        x += bitmapData.BytesPerPixel)
                    {
                        processBitmap.ProcessPixel(
                            bitmapData.FirstPixelPtr,
                            x, y,
                            bitmapData.OriginalBitmapData.Stride,
                            bitmapData.BytesPerPixel,
                            bitmapData.FirstPixelPtrCopy);
                    }
                }

            }

            return resultBitmap;
        }

        public unsafe long[][] CreateHistogram(Bitmap bitmap, bool isGreyscale)
        {
            long[][] result = new long[256][];

            for (int i = 0; i < 256; i++)
            {
                result[i] = isGreyscale ? new long[1] : new long[3];
            }

            using (var bitmapData = new CustomBitmapData(bitmap, true))
            {
                for (int y = 0; y < bitmapData.HeightInPixels; y++)
                {
                    for (int x = 0; 
                        x < bitmapData.WidthInBytes; 
                        x += bitmapData.BytesPerPixel)
                    {
                        byte* currentPixelPtr = ImageHelper.SetPixelPointer(
                            bitmapData.FirstPixelPtr,
                            x, y,
                            bitmapData.OriginalBitmapData.Stride);

                        if (isGreyscale)
                        {
                            if (bitmapData.BytesPerPixel < 3)
                            {
                                result[currentPixelPtr[0]][0]++;

                            }
                            else
                            {
                                var greyscale = ImageHelper.ConverToGreyscale(
                                    currentPixelPtr[0],
                                    currentPixelPtr[1],
                                    currentPixelPtr[2]);
                                result[(byte)ImageHelper.FixOverflow(greyscale)][0]++;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                result[currentPixelPtr[i]][i]++;
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}
