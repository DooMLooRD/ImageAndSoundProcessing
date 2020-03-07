namespace ImageProcessing.Core.Interfaces
{
    public interface IProcessingOperation
    {
        unsafe void ProcessPixel(byte* firstPixelPtr, int x, int y, int width, int bytesPerPixel, byte* copy = null);
    }
}
