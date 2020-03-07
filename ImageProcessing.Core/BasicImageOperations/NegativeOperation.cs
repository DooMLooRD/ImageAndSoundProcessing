using ImageProcessing.Core.Helpers;
using ImageProcessing.Core.Interfaces;

namespace ImageProcessing.Core.BasicImageOperations
{
    public class NegativeOperation : IProcessingOperation
    {
        public unsafe void ProcessPixel(byte* firstPixelPtr, int x, int y, int width, int bytesPerPixel, byte* copy = null)
        {
            byte* resultPixelPtr = ImageHelper.SetResultPixelPointer(firstPixelPtr, x, y, width, copy);
            byte* currentPixelPtr = ImageHelper.SetPixelPointer(firstPixelPtr, x, y, width);

            for (int i = 0; i < bytesPerPixel; i++)
            {
                resultPixelPtr[i] = (byte)(255 - currentPixelPtr[i]);
            }
        }
    }
}
