using ImageProcessing.Core.Helpers;
using ImageProcessing.Core.Interfaces;

namespace ImageProcessing.Core.BasicImageOperations
{
    public class ContrastOperation : IProcessingOperation
    {
        private readonly double _slopeFactor;

        public ContrastOperation(double slopeFactor)
        {
            _slopeFactor = slopeFactor;
        }

        public unsafe void ProcessPixel(byte* firstPixelPtr, int x, int y, int width, int bytesPerPixel, byte* copy = null)
        {
            byte* resultPixelPtr = ImageHelper.SetResultPixelPointer(firstPixelPtr, x, y, width, copy);
            byte* currentPixelPtr = ImageHelper.SetPixelPointer(firstPixelPtr, x, y, width);

            var intercept = MathHelper.CalculateLinearIntercept(_slopeFactor);

            for (int i = 0; i < bytesPerPixel; i++)
            {
                resultPixelPtr[i] = (byte)ImageHelper.FixOverflow(_slopeFactor * currentPixelPtr[i] + intercept);
            }
        }
    }
}
