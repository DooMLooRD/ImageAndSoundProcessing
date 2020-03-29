using ImageProcessing.Core.Helpers;
using ImageProcessing.Core.Interfaces;
using ImageProcessing.Core.Model;

namespace ImageProcessing.Core.LinearFilters
{
    public class LinearFilter : IProcessingOperation
    {
        private readonly int[] _mask;
        private readonly int _maskSize;

        public LinearFilter(int[] mask, int maskSize)
        {
            _mask = mask;
            _maskSize = maskSize;
        }

        public unsafe void ProcessPixel(ExtendedBitmapData customBitmap, int x, int y)
        {
            byte* resultPixelPtr = ImageHelper.SetResultPixelPointer(customBitmap, x, y);
            byte*[] a = ImageHelper.GetWindowPtrs(customBitmap, x, y, _maskSize);

            for (int i = 0; i < customBitmap.BytesPerPixel; i++)
            {
                var result = 0;
                for (int j = 0; j < _maskSize * _maskSize; j++)
                {
                    result += a[j][i] * _mask[j];
                }

                resultPixelPtr[i] = (byte)ImageHelper.FixOverflow(result);
            }
        }
    }
}
