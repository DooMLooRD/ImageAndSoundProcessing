using ImageProcessing.Core.Helpers;
using ImageProcessing.Core.Interfaces;

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

        public unsafe void ProcessPixel(byte* firstPixelPtr, int x, int y, int width, int bytesPerPixel, byte* copy = null)
        {
            byte* resultPixelPtr = ImageHelper.SetResultPixelPointer(firstPixelPtr, x, y, width, copy);
            byte*[] a = ImageHelper.GetWindowPtrs(firstPixelPtr, x, y, width, bytesPerPixel, _maskSize);

            for (int i = 0; i < bytesPerPixel; i++)
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
