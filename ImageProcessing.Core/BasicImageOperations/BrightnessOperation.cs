
using ImageProcessing.Core.Helpers;
using ImageProcessing.Core.Interfaces;

namespace ImageProcessing.Core.BasicImageOperations
{
    public class BrightnessOperation : IProcessingOperation
    {
        private readonly int _brightnessFactor;

        public BrightnessOperation(int brightnessFactor)
        {
            _brightnessFactor = brightnessFactor;
        }

        public unsafe void ProcessPixel(byte* firstPixelPtr, int x, int y, int width, int bytesPerPixel, byte* copy = null)
        {
            byte* resultPixelPtr = ImageHelper.SetResultPixelPointer(firstPixelPtr, x, y, width, copy);
            byte* currentPixelPtr = ImageHelper.SetPixelPointer(firstPixelPtr, x, y, width);

            for (int i = 0; i < bytesPerPixel; i++)
            {
                resultPixelPtr[i] = (byte)ImageHelper.FixOverflow(currentPixelPtr[i] + _brightnessFactor);
            }
        }
    }
}
