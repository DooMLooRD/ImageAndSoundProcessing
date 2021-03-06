﻿using ImageProcessing.Core.Helpers;
using ImageProcessing.Core.Interfaces;
using ImageProcessing.Core.Model;

namespace ImageProcessing.Core.BasicImageOperations
{
    public class GreyscaleOperation : IProcessingOperation
    {
        public unsafe void ProcessPixel(ExtendedBitmapData customBitmap, int x, int y)
        {
            byte* resultPixelPtr = ImageHelper.SetResultPixelPointer(customBitmap, x, y);
            byte* currentPixelPtr = ImageHelper.SetPixelPointer(customBitmap, x, y);

            for (int i = 0; i < customBitmap.BytesPerPixel; i++)
            {
                var greyscale = ImageHelper.ConverToGreyscale(currentPixelPtr[0], currentPixelPtr[1], currentPixelPtr[2]);
                resultPixelPtr[i] = (byte)ImageHelper.FixOverflow(greyscale);
            }
        }
    }
}
