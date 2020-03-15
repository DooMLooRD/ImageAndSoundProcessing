using ImageProcessing.Core.Helpers;
using ImageProcessing.Core.Interfaces;

namespace ImageProcessing.Core.BasicImageOperations
{
    public class GreyscaleOperation : IProcessingOperation
    {
        public unsafe void ProcessPixel(byte* firstPixelPtr, int x, int y, int width, int bytesPerPixel, byte* copy = null)
        {
            byte* resultPixelPtr = ImageHelper.SetResultPixelPointer(firstPixelPtr, x, y, width, copy);
            byte* currentPixelPtr = ImageHelper.SetPixelPointer(firstPixelPtr, x, y, width);

            for (int i = 0; i < bytesPerPixel; i++)
            {
                if (bytesPerPixel < 3)
                {
                    resultPixelPtr[i] = currentPixelPtr[i];
                }
                else
                {
                    var greyscale = ImageHelper.ConverToGreyscale(currentPixelPtr[0], currentPixelPtr[1], currentPixelPtr[2]);
                    resultPixelPtr[i] = (byte)ImageHelper.FixOverflow(greyscale);
                }
            }
        }
    }
}
