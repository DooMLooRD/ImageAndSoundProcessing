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

        public unsafe void ProcessPixel(CustomBitmapData customBitmap, int x, int y)
        {
            byte* resultPixelPtr = ImageHelper.SetResultPixelPointer(customBitmap, x, y);

            int[][] window = ImageHelper.GetWindow(customBitmap, x, y, _windowSize);

            for (int i = 0; i < customBitmap.BytesPerPixel; i++)
            {
                double result = window[i].Average();
                resultPixelPtr[i] = (byte)ImageHelper.FixOverflow(result);
            }
        }
    }
}
