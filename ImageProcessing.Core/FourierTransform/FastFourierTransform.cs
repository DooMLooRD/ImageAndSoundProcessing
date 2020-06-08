using ImageProcessing.Core.Helpers;
using ImageProcessing.Core.Model;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;

namespace ImageProcessing.Core.FourierTransform
{
    public unsafe class FastFourierTransform
    {
        public static Bitmap IFFT2D(Complex[][] data, Bitmap originalBitmap)
        {
            FourierHelper.Conjugate(data);
            for (int i = 0; i < data.Length; i++)
            {
                FFT1D(data[i]);
            }
            FourierHelper.Conjugate(data);

            CollectionHelper.ApplyOperation(data, c => c / data[0].Length);

            var swappedData = CollectionHelper.SwapRowColumn(data);

            FourierHelper.Conjugate(swappedData);
            for (int i = 0; i < swappedData.Length; i++)
            {
                FFT1D(swappedData[i]);
            }
            FourierHelper.Conjugate(swappedData);

            CollectionHelper.ApplyOperation(swappedData, c => c / swappedData[0].Length);

            using (var bitmapData = new SingleBitmapData(originalBitmap, PixelFormat.Format8bppIndexed))
            {
                for (int y = 0; y < bitmapData.HeightInPixels; y++)
                {
                    for (int x = 0;
                        x < bitmapData.WidthInBytes;
                        x += bitmapData.BytesPerPixel)
                    {
                        bitmapData.FirstPixelPtr[x + y * bitmapData.WidthInBytes] = (byte)ImageHelper.FixOverflow(swappedData[y][x].Real);
                    }
                }

                return bitmapData.Bitmap;
            }
        }

        public static ImageComponents FFT2D(Bitmap bitmap)
        {
            using (SingleBitmapData bitmapData = new SingleBitmapData(bitmap, PixelFormat.Format8bppIndexed))
            {
                var data = CollectionHelper.Create2DArray<Complex>(bitmapData.WidthInBytes, bitmapData.HeightInPixels);

                for (int y = 0; y < bitmapData.HeightInPixels; y++)
                {
                    for (int x = 0;
                        x < bitmapData.WidthInBytes;
                        x += bitmapData.BytesPerPixel)
                    {
                        data[y][x] = bitmapData.FirstPixelPtr[x + y * bitmapData.WidthInBytes];
                    }
                }

                for (int i = 0; i < data.Length; i++)
                {
                    FFT1D(data[i]);
                }

                var swappedData = CollectionHelper.SwapRowColumn(data);

                for (int i = 0; i < swappedData.Length; i++)
                {
                    FFT1D(swappedData[i]);
                }

                return new ImageComponents(swappedData, bitmap);
            }
        }

        private static void FFT1D(Complex[] buffer)
        {
            int bits = (int)Math.Log(buffer.Length, 2);

            for (int j = 1; j < buffer.Length; j++)
            {
                int swapPos = BitReverse(j, bits);
                if (swapPos <= j)
                {
                    continue;
                }
                var temp = buffer[j];
                buffer[j] = buffer[swapPos];
                buffer[swapPos] = temp;
            }


            for (int N = 2; N <= buffer.Length; N <<= 1)
            {
                for (int i = 0; i < buffer.Length; i += N)
                {
                    for (int k = 0; k < N / 2; k++)
                    {
                        int evenIndex = i + k;
                        int oddIndex = i + k + (N / 2);
                        var even = buffer[evenIndex];
                        var odd = buffer[oddIndex];

                        double term = -2 * Math.PI * k / N;
                        Complex exp = new Complex(Math.Cos(term), Math.Sin(term)) * odd;

                        buffer[evenIndex] = even + exp;
                        buffer[oddIndex] = even - exp;
                    }
                }
            }
        }

        private static int BitReverse(int n, int bits)
        {
            int reversedN = n;
            int count = bits - 1;

            n >>= 1;
            while (n > 0)
            {
                reversedN = (reversedN << 1) | (n & 1);
                count--;
                n >>= 1;
            }

            return (reversedN << count) & ((1 << bits) - 1);
        }
    }
}
