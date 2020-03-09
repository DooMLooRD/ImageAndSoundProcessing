using System.Drawing;
using System.Drawing.Imaging;

namespace ImageProcessing.Core.Helpers
{
    public class ImageHelper
    {
        public static double ConverToGreyscale(double red, double green, double blue)
        {
            double sum = 0;

            sum += 0.21 * red;
            sum += 0.71 * green;
            sum += 0.071 * blue;

            return sum;
        }

        public static double FixOverflow(double value)
        {
            if (value > 255)
            {
                value = 255;
            }
            else if (value < 0)
            {
                value = 0;
            }

            return value;
        }

        public unsafe static byte*[] GetNeighborhood(byte* firstPixelPtr, int x, int y, int width, int bytesPerPixel)
        {
            byte*[] a = new byte*[8];

            a[0] = firstPixelPtr + (y - 1) * width + (x - bytesPerPixel);
            a[1] = firstPixelPtr + (y - 1) * width + x;
            a[2] = firstPixelPtr + (y - 1) * width + (x + bytesPerPixel);
            a[3] = firstPixelPtr + y * width + (x + bytesPerPixel);

            a[4] = firstPixelPtr + (y + 1) * width + (x + bytesPerPixel);
            a[5] = firstPixelPtr + (y + 1) * width + x;
            a[6] = firstPixelPtr + (y + 1) * width + (x - bytesPerPixel);
            a[7] = firstPixelPtr + y * width + (x - bytesPerPixel);

            return a;
        }

        public unsafe static int[][] GetWindow(byte* firstPixelPtr, int xMiddle, int yMiddle, int rowWidth, int bytesPerPixel, int windowSize)
        {
            int[][] a = new int[bytesPerPixel][];

            int xStart = xMiddle - (windowSize / 2 * bytesPerPixel);
            int yStart = yMiddle - (windowSize / 2);

            for (int i = 0; i < bytesPerPixel; i++)
            {
                a[i] = new int[windowSize * windowSize];
                for (int j = 0; j < windowSize; j++)
                {
                    for (int k = 0; k < windowSize; k++)
                    {
                        a[i][j * windowSize + k] = (firstPixelPtr + (yStart + j) * rowWidth + (xStart + (k * bytesPerPixel)))[i];
                    }
                }
            }

            return a;
        }
        public unsafe static byte* SetPixelPointer(byte* firstPixelPtr, int x, int y, int width)
        {
            return firstPixelPtr + y * width + x;
        }

        public unsafe static byte* SetResultPixelPointer(byte* firstPixelPtr, int x, int y, int width, byte* copy = null)
        {
            byte* currentPixelPtr = copy == null ? firstPixelPtr : copy;
            currentPixelPtr += y * width + x;
            return currentPixelPtr;
        }

        public static void ConvertToPixelFormat(Bitmap bitmap, out Bitmap result, PixelFormat pixelFormat)
        {
            result = new Bitmap(bitmap.Width, bitmap.Height, pixelFormat);

            Graphics g = Graphics.FromImage(result);
            g.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
            g.Dispose();
        }
    }
}
