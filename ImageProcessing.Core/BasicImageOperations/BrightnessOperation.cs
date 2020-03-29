
using ImageProcessing.Core.Helpers;
using ImageProcessing.Core.Interfaces;
using ImageProcessing.Core.Model;

namespace ImageProcessing.Core.BasicImageOperations
{
    public class BrightnessOperation : IProcessingOperation
    {
        private readonly int _brightnessFactor;

        public BrightnessOperation(int brightnessFactor)
        {
            _brightnessFactor = brightnessFactor;
        }

        public unsafe void ProcessPixel(ExtendedBitmapData customBitmap, int x, int y)
        {
            byte* resultPixelPtr = ImageHelper.SetResultPixelPointer(customBitmap, x, y);
            byte* currentPixelPtr = ImageHelper.SetPixelPointer(customBitmap, x, y);

            for (int i = 0; i < customBitmap.BytesPerPixel; i++)
            {
                resultPixelPtr[i] = (byte)ImageHelper.FixOverflow(currentPixelPtr[i] + _brightnessFactor);
            }
        }
    }
}
