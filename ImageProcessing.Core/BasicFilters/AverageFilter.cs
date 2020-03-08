using ImageProcessing.Core.Helpers;
using ImageProcessing.Core.Interfaces;
using System.Linq;

namespace ImageProcessing.Core.BasicFilters
{
    public class AverageFilter : IProcessingOperation
    {
        private readonly int _windowSize;

        public AverageFilter(int windowSize)
        {
            _windowSize = windowSize;
        }

        public unsafe void ProcessPixel(byte* firstPixelPtr, int x, int y, int width, int bytesPerPixel, byte* copy = null)
        {
            byte* resultPixelPtr = ImageHelper.SetResultPixelPointer(firstPixelPtr, x, y, width, copy);

            int[][] window = ImageHelper.GetWindow(firstPixelPtr, x, y, width, bytesPerPixel, _windowSize);

            for (int i = 0; i < bytesPerPixel; i++)
            {
                double result = window[i].Average();
                resultPixelPtr[i] = (byte)ImageHelper.FixOverflow(result);
            }
        }
    }
}
