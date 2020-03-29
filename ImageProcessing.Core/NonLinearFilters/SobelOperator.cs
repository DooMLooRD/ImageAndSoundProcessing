using ImageProcessing.Core.Helpers;
using ImageProcessing.Core.Interfaces;
using ImageProcessing.Core.Model;
using System;

namespace ImageProcessing.Core.NonLinearFilters
{
    public class SobelOperator : IProcessingOperation
    {
        public unsafe void ProcessPixel(ExtendedBitmapData customBitmap, int x, int y)
        {
            byte* resultPixelPtr = ImageHelper.SetResultPixelPointer(customBitmap, x, y);
            byte*[] a = ImageHelper.GetNeighborhood(customBitmap, x, y);

            for (int i = 0; i < customBitmap.BytesPerPixel; i++)
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
