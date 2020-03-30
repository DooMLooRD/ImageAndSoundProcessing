using ImageProcessing.Core.Interfaces;
using System;
using System.Numerics;

namespace ImageProcessing.Core.FourierOperations
{
    public class LowPassFilter : IProcessingFourierOperation
    {
        private readonly int _filterRadius;

        public LowPassFilter(int filterRarius)
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

                    var widthFactor = Math.Pow((halfImageWidth - j) / (halfImageWidth - _filterRadius), 2);
                    var heightFactor = Math.Pow((halfImageHeight - i) / (halfImageHeight - _filterRadius), 2);

                    if (Math.Sqrt(widthFactor + heightFactor) > 1)
                    {
                        complexData[i][j] = new Complex(0, 0);
                    }
                }
            }
        }
    }
}
