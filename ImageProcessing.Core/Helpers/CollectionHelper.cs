﻿using System;
using System.Numerics;

namespace ImageProcessing.Core.Helpers
{
    public class CollectionHelper
    {
        public static Complex[][] SwapRowColumn(Complex[][] data)
        {
            int swappedCols = data.Length;
            int swappedRows = data[0].Length;

            Complex[][] swappedData = new Complex[swappedRows][];

            for (int i = 0; i < swappedRows; i++)
            {
                swappedData[i] = new Complex[swappedCols];
            }

            for (int x = 0; x < swappedCols; x++)
            {
                for (int y = 0; y < swappedRows; y++)
                {
                    swappedData[y][x] = data[x][y];
                }
            }

            return swappedData;
        }

        public static void ApplyOperation(Complex[][] data, Func<Complex, Complex> operation)
        {
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data[i].Length; j++)
                {
                    data[i][j] = operation(data[i][j]);
                }
            }
        }

        public static Complex[][] Create2DComplexArray(int width, int height)
        {
            var data = new Complex[height][];

            for (int i = 0; i < height; i++)
            {
                data[i] = new Complex[width];
            }

            return data;
        }
    }
}
