using ImageProcessing.Core.Model;

namespace ImageProcessing.Core.Interfaces
{
    public interface IProcessingOperation
    {
        unsafe void ProcessPixel(ExtendedBitmapData customBitmap, int x, int y);
    }
}
