using ImageProcessing.Core.Helpers;
using ImageProcessing.Core.Interfaces;
using ImageProcessing.Core.Model;
using System;

namespace ImageProcessing.Core.NonLinearFilters
{
    public class UolisOperator : IProcessingOperation
    {
        private readonly int _normalizationMultiplier;

        public UolisOperator(int normalizationMultiplier)
        {
            _normalizationMultiplier = normalizationMultiplier;
        }

        public unsafe void ProcessPixel(ExtendedBitmapData customBitmap, int x, int y)
        {
            byte* resultPixelPtr = ImageHelper.SetResultPixelPointer(customBitmap, x, y);
            byte* currentPixelPtr = ImageHelper.SetPixelPointer(customBitmap, x, y);

            byte*[] a = ImageHelper.GetNeighborhood(customBitmap, x, y);

            for (int i = 0; i < customBitmap.BytesPerPixel; i++)
            {
                double power = Math.Pow(currentPixelPtr[i], 4);
                double divider = a[1][i] * a[3][i] * a[5][i] * a[7][i];

                var result = 0.25 * Math.Log(power / divider);
                result = ImageHelper.FixOverflow(_normalizationMultiplier * result);

                resultPixelPtr[i] = (byte)result;
            }
        }
    }
}
