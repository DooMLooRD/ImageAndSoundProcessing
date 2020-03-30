using ImageProcessing.Core.Helpers;
using ImageProcessing.Core.Interfaces;
using ImageProcessing.Core.Model;

namespace ImageProcessing.Core.HistogramOperations
{
    public class UniformProbabilityDensity : IProcessingOperation
    {
        private readonly long[][] _histogramValues;
        private readonly int[] _gMin;
        private readonly int[] _gMax;

        public UniformProbabilityDensity(long[][] histogramValues, int aMin, int aMax)
            : this(histogramValues, aMin, aMax, aMin, aMax, aMin, aMax)
        {
        }

        public UniformProbabilityDensity(long[][] histogramValues, int rMin, int rMax, int gMin, int gMax, int bMin, int bMax)
        {
            _histogramValues = histogramValues;
            _gMin = new[] { rMin, gMin, bMin };
            _gMax = new[] { rMax, gMax, bMax };
        }

        public unsafe void ProcessPixel(ExtendedBitmapData bitmapData, int x, int y)
        {
            byte* resultPixelPtr = ImageHelper.SetResultPixelPointer(bitmapData, x, y);

            byte* currentPixelPtr = ImageHelper.SetPixelPointer(bitmapData, x, y);

            double n = bitmapData.CopyBitmap.Width * bitmapData.CopyBitmap.Height;
            for (int i = 0; i < bitmapData.BytesPerPixel; i++)
            {
                var result = _gMin[i] + (_gMax[i] - _gMin[i]) * CalculateHistogramSum(i, currentPixelPtr[i]) / n;
                resultPixelPtr[i] = (byte)result;
            }
        }

        private double CalculateHistogramSum(int color, int f)
        {
            double sum = 0;
            for (int m = 0; m <= f; m++)
            {
                sum += _histogramValues[m][color];
            }
            return sum;
        }
    }
}
