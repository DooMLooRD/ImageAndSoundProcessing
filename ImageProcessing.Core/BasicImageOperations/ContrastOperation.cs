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

        public unsafe void ProcessPixel(CustomBitmapData customBitmap, int x, int y)
        {
            byte* resultPixelPtr = ImageHelper.SetResultPixelPointer(customBitmap, x, y);
            byte* currentPixelPtr = ImageHelper.SetPixelPointer(customBitmap, x, y);

            var intercept = MathHelper.CalculateLinearIntercept(_slopeFactor);

            for (int i = 0; i < customBitmap.BytesPerPixel; i++)
            {
                resultPixelPtr[i] = (byte)ImageHelper.FixOverflow(_slopeFactor * currentPixelPtr[i] + intercept);
            }
        }
    }
}
