using ImageProcessing.Core.Helpers;
using ImageProcessing.Core.Interfaces;
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

        public unsafe void ProcessPixel(byte* firstPixelPtr, int x, int y, int width, int bytesPerPixel, byte* copy = null)
        {
            byte* resultPixelPtr = ImageHelper.SetResultPixelPointer(firstPixelPtr, x, y, width, copy);
            byte* currentPixelPtr = ImageHelper.SetPixelPointer(firstPixelPtr, x, y, width);

            byte*[] a = ImageHelper.GetNeighborhood(firstPixelPtr, x, y, width, bytesPerPixel);

            for (int i = 0; i < bytesPerPixel; i++)
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
