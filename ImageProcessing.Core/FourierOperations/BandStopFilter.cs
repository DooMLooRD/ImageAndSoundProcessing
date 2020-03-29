using ImageProcessing.Core.Interfaces;
using System;
using System.Numerics;

namespace ImageProcessing.Core.FourierOperations
{
    public class BandStopFilter : IProcessingFourierOperation
    {
        public int FilterRadius { get; set; }
        public int FilterSize { get; set; }

        public BandStopFilter(int filterRarius, int filterSize)
        {
            FilterRadius = filterRarius;
            FilterSize = filterSize;
        }

        public void ProcessImage(Complex[][] complexData)
        {
            double imageHeight = complexData.Length;
            double imageWidth = complexData[0].Length;

            double halfImageHeight = imageHeight / 2;
            double halfImageWidth = imageWidth / 2;

            for (int i = 0; i < imageHeight; i++)
            {
                for (int j = 0; j < imageWidth; j++)
                {
                    if (i == halfImageHeight && j == halfImageWidth)
                    {
                        continue;
                    }

                    var widthFactor = Math.Pow(halfImageWidth - j, 2);
                    var heightFactor = Math.Pow(halfImageHeight - i, 2);
                    var result = Math.Sqrt(widthFactor + heightFactor);

                    if (result < FilterRadius + FilterSize && result > FilterRadius)
                    {
                        complexData[i][j] = 0;
                    }
                }
            }
        }
    }
}
