using System;
using ImageProcessing.Core.Helpers;
using ImageProcessing.Core.Interfaces;
using System.Drawing;
using System.Drawing.Imaging;

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
            using (var bitmapData = new CustomBitmapData(bitmap))
            {
                resultBitmap = bitmapData.CopyBitmap;
                for (int y = offset; y < bitmapData.HeightInPixels - offset; y++)
                {
                    for (int x = offset * bitmapData.BytesPerPixel;
                        x < bitmapData.WidthInBytes - offset * bitmapData.BytesPerPixel;
                        x += bitmapData.BytesPerPixel)
                    {
                        processBitmap.ProcessPixel(bitmapData, x, y);
                    }
                }

            }

            ImageHelper.ConvertToPixelFormat(resultBitmap, out Bitmap convertedResult, bitmap.PixelFormat);

            return convertedResult;
        }

        public static unsafe Histogram CreateHistogram(Bitmap bitmap)
        {
            long[][] result = new long[256][];

            for (int i = 0; i < 256; i++)
            {
                result[i] = new long[3];
            }

            using (var bitmapData = new CustomBitmapData(bitmap))
            {
                for (int y = 0; y < bitmapData.HeightInPixels; y++)
                {
                    for (int x = 0;
                        x < bitmapData.WidthInBytes;
                        x += bitmapData.BytesPerPixel)
                    {
                        byte* currentPixelPtr = ImageHelper.SetPixelPointer(bitmapData, x, y);

                        for (int i = 0; i < 3; i++)
                        {
                            result[currentPixelPtr[i]][i]++;
                        }
                    }
                }
            }

            return new Histogram
            {
                IsGreyscale = bitmap.PixelFormat == PixelFormat.Format8bppIndexed
                || bitmap.PixelFormat == PixelFormat.Format1bppIndexed
                || bitmap.PixelFormat == PixelFormat.Format16bppGrayScale,

                Values = result
            };
        }

        public static unsafe FiltersEvaluationData FiltersEvaluation(Bitmap originalBitmap, Bitmap resultBitmap)
        {
            using var bitmapData = new CustomBitmapData(originalBitmap);
            using var resultBitmapData = new CustomBitmapData(resultBitmap);

            var sum = 0.0;
            var abs = 0.0;

            const double k = 255.0;

            for (int y = 0; y < bitmapData.HeightInPixels; y++)
            {
                for (int x = 0;
                    x < bitmapData.WidthInBytes;
                    x += bitmapData.BytesPerPixel)
                {
                    byte* currentPixelPtr = ImageHelper.SetPixelPointer(bitmapData, x, y);
                    byte* rCurrentPixelPtr = ImageHelper.SetPixelPointer(resultBitmapData, x, y);
                    var pixelValue = 0;
                    var rPixelValue = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        pixelValue += currentPixelPtr[i];
                        rPixelValue += rCurrentPixelPtr[i];
                    }
                    pixelValue /= 3;
                    rPixelValue /= 3;
                    sum += Math.Pow(pixelValue - rPixelValue, 2);
                    abs += Math.Abs(pixelValue - rPixelValue);
                }
            }

            var nm = bitmapData.OriginalBitmap.Width * bitmapData.OriginalBitmap.Height;

            var fraction = 1.0 / nm;
            
            var mse = fraction * sum;
            var psnr = 10.0 * Math.Log10(Math.Pow(k, 2) / mse);
            var mae = fraction * abs;

            return new FiltersEvaluationData
            {
                Mse = mse,
                Psnr = psnr,
                Mae = mae
            };
        }
    }
}
