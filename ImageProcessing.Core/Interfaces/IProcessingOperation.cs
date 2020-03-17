using ImageProcessing.Core.Helpers;

namespace ImageProcessing.Core.Interfaces
{
    public interface IProcessingOperation
    {
        unsafe void ProcessPixel(CustomBitmapData customBitmap, int x, int y);
    }
}
