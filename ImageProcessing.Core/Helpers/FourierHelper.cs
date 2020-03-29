using System;
using System.Numerics;

namespace ImageProcessing.Core.Helpers
{
    public class FourierHelper
    {
        public static void SwapQuadrants(Complex[][] complexData)
        {
            int size = complexData.Length;

            for (int x = 0; x < size / 2; x++)
            {
                for (int y = 0; y < size / 2; y++)
                {
                    Complex temp = complexData[x][y];
                    complexData[x][y] = complexData[x + size / 2][y + size / 2];
                    complexData[x + size / 2][y + size / 2] = temp;
                }
            }

            for (int x = size / 2; x < size; x++)
            {
                for (int y = 0; y < size / 2; y++)
                {
                    Complex temp = complexData[x][y];
                    complexData[x][y] = complexData[x - size / 2][y + size / 2];
                    complexData[x - size / 2][y + size / 2] = temp;
                }
            }
        }

        public static double LogNormalize(double value, double max, double expectedMax)
        {
            return Math.Log(1 + value) / Math.Log(1 + max) * expectedMax;
        }

        public static void Conjugate(Complex[][] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data[i].Length; j++)
                {
                    data[i][j] = Complex.Conjugate(data[i][j]);
                }
            }
        }
    }
}
