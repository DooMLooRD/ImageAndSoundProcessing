using ImageProcessing.Core.Helpers;
using ImageProcessing.Core.Interfaces;
using System;

namespace ImageProcessing.Core.NonLinearFilters
{
    public class SobelOperator : IProcessingOperation
    {
        public unsafe void ProcessPixel(byte* firstPixelPtr, int x, int y, int width, int bytesPerPixel, byte* copy = null)
        {
            byte* resultPixelPtr = ImageHelper.SetResultPixelPointer(firstPixelPtr, x, y, width, copy);
            byte*[] a = ImageHelper.GetNeighborhood(firstPixelPtr, x, y, width, bytesPerPixel);

            for (int i = 0; i < bytesPerPixel; i++)
            {
                var result = CalculateSobel(a, i);
                result = ImageHelper.FixOverflow(result);

                resultPixelPtr[i] = (byte)result;
            }
        }

        private unsafe double CalculateSobel(byte*[] a, int i)
        {
            double sx = a[2][i] + (2 * a[3][i]) + a[4][i] - (a[0][i] + (2 * a[7][i]) + a[6][i]);
            double sy = a[0][i] + (2 * a[1][i]) + a[2][i] - (a[6][i] + (2 * a[5][i]) + a[4][i]);
            return Math.Sqrt((sx * sx) + (sy * sy));
        }
    }
}
