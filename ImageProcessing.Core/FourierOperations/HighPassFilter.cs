using ImageProcessing.Core.Interfaces;
using System;
using System.Numerics;

namespace ImageProcessing.Core.FourierOperations
{
    public class HighPassFilter : IProcessingFourierOperation
    {
        private readonly int _filterRadius;

        public HighPassFilter(int filterRarius)
        {
            _filterRadius = filterRarius;
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

                    var widthFactor = Math.Pow((halfImageWidth - j) / _filterRadius, 2);
                    var heightFactor = Math.Pow((halfImageHeight - i) / _filterRadius, 2);

                    if (Math.Sqrt(widthFactor + heightFactor) < 1)
                    {
                        complexData[i][j] = 0;
                    }
                }
            }
        }
    }
}
