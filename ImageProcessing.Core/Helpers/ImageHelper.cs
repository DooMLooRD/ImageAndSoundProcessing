using ImageProcessing.Core.Model;
using System.Collections.Generic;
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

        public static byte Normalize(int value, int max)
        {
            return (byte)(10 + (value * (255 - 10) / max));
        }

        public static bool IsGreyscale(PixelFormat pixelFormat)
        {
            return pixelFormat == PixelFormat.Format1bppIndexed ||
                   pixelFormat == PixelFormat.Format8bppIndexed ||
                   pixelFormat == PixelFormat.Format16bppGrayScale;
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

        public static List<int> GetProcessedNeighbourhood(SingleBitmapData bitmapData, int position, int[] imagePixels)
        {
            int x = position % bitmapData.WidthInBytes;
            int y = position / bitmapData.WidthInBytes;
            var neighbourhood = new List<int>();

            for (int j = -2; j < 3; j++)
            {
                for (int k = -2; k < 3; k++)
                {
                    var currY = y + j;
                    var currX = x + k;
                    var currPos = currY * bitmapData.WidthInBytes + currX;
                    if (currY >= 0 && currY < bitmapData.HeightInPixels &&
                        currX >= 0 && currX < bitmapData.WidthInBytes &&
                        imagePixels[currPos] != 0)
                    {
                        neighbourhood.Add(currPos);
                    }
                }
            }

            return neighbourhood;
        }

        public static List<int> GetNeighbourhood(SingleBitmapData bitmapData, int position, HashSet<int> visited, int[] imagePixels)
        {
            int x = position % bitmapData.WidthInBytes;
            int y = position / bitmapData.WidthInBytes;
            var neighbourhood = new List<int>();

            for (int j = -1; j < 2; j++)
            {
                for (int k = -1; k < 2; k++)
                {
                    var currY = y + j;
                    var currX = x + k;
                    var currPos = currY * bitmapData.WidthInBytes + currX;
                    if (currY >= 0 && currY < bitmapData.HeightInPixels &&
                        currX >= 0 && currX < bitmapData.WidthInBytes &&
                        !visited.Contains(currPos) && imagePixels[currPos] == 0)
                    {
                        neighbourhood.Add(currPos);
                    }
                }
            }

            return neighbourhood;
        }

        public unsafe static byte*[] GetNeighborhood(ExtendedBitmapData bitmapData, int x, int y)
        {
            byte*[] a = new byte*[8];

            a[0] = bitmapData.FirstPixelPtr + (y - 1) * bitmapData.WidthInBytes + (x - bitmapData.BytesPerPixel);
            a[1] = bitmapData.FirstPixelPtr + (y - 1) * bitmapData.WidthInBytes + x;
            a[2] = bitmapData.FirstPixelPtr + (y - 1) * bitmapData.WidthInBytes + (x + bitmapData.BytesPerPixel);
            a[3] = bitmapData.FirstPixelPtr + y * bitmapData.WidthInBytes + (x + bitmapData.BytesPerPixel);
            a[4] = bitmapData.FirstPixelPtr + (y + 1) * bitmapData.WidthInBytes + (x + bitmapData.BytesPerPixel);
            a[5] = bitmapData.FirstPixelPtr + (y + 1) * bitmapData.WidthInBytes + x;
            a[6] = bitmapData.FirstPixelPtr + (y + 1) * bitmapData.WidthInBytes + (x - bitmapData.BytesPerPixel);
            a[7] = bitmapData.FirstPixelPtr + y * bitmapData.WidthInBytes + (x - bitmapData.BytesPerPixel);

            return a;
        }

        public unsafe static byte*[] GetWindowPtrs(ExtendedBitmapData bitmapData, int xMiddle, int yMiddle, int windowSize)
        {
            byte*[] a = new byte*[windowSize * windowSize];

            int xStart = xMiddle - (windowSize / 2 * bitmapData.BytesPerPixel);
            int yStart = yMiddle - (windowSize / 2);

            for (int j = 0; j < windowSize; j++)
            {
                for (int k = 0; k < windowSize; k++)
                {
                    a[j * windowSize + k] = bitmapData.FirstPixelPtr + (yStart + j)
                        * bitmapData.WidthInBytes + (xStart + (k * bitmapData.BytesPerPixel));
                }
            }

            return a;
        }

        public unsafe static int[][] GetWindow(ExtendedBitmapData bitmapData, int xMiddle, int yMiddle, int windowSize)
        {
            int[][] a = new int[bitmapData.BytesPerPixel][];

            int xStart = xMiddle - (windowSize / 2 * bitmapData.BytesPerPixel);
            int yStart = yMiddle - (windowSize / 2);

            for (int i = 0; i < bitmapData.BytesPerPixel; i++)
            {
                a[i] = new int[windowSize * windowSize];
                for (int j = 0; j < windowSize; j++)
                {
                    for (int k = 0; k < windowSize; k++)
                    {
                        a[i][j * windowSize + k] = (bitmapData.FirstPixelPtr + (yStart + j) *
                            bitmapData.WidthInBytes + (xStart + (k * bitmapData.BytesPerPixel)))[i];
                    }
                }
            }

            return a;
        }

        public unsafe static byte* SetPixelPointer(ExtendedBitmapData bitmapData, int x, int y)
        {
            return bitmapData.FirstPixelPtr + y * bitmapData.WidthInBytes + x;
        }

        public unsafe static byte* SetResultPixelPointer(ExtendedBitmapData bitmapData, int x, int y)
        {
            byte* currentPixelPtr = bitmapData.FirstPixelPtrCopy;
            currentPixelPtr += y * bitmapData.WidthInBytes + x;
            return currentPixelPtr;
        }

        public static void ConvertToPixelFormat(Bitmap bitmap, out Bitmap result, PixelFormat pixelFormat)
        {
            result = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), pixelFormat);
        }
    }
}
