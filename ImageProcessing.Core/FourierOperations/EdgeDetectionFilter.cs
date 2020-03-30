using ImageProcessing.Core.Interfaces;
using System;
using System.Numerics;

namespace ImageProcessing.Core.FourierOperations
{
    public class EdgeDetectionFilter : IProcessingFourierOperation
    {
        private const double NOISE = 0.0001;
        private const double EPSILON = 0.0000001;

        private readonly double _firstLineAngle;
        private readonly double _secondLineAngle;
        private readonly int _filterRadius;

        public EdgeDetectionFilter(double filterAngle, double filterAngleOffset, int filterRadius)
        {
            _filterRadius = filterRadius;

            filterAngleOffset %= 360;
            filterAngleOffset = filterAngleOffset < 0 ? 360 + filterAngleOffset : filterAngleOffset;
            filterAngleOffset = (filterAngleOffset + NOISE) * 2 * Math.PI / 360;

            filterAngle /= 2;
            filterAngle = (filterAngle + NOISE) * 2 * Math.PI / 360;

            _firstLineAngle = filterAngle + filterAngleOffset;
            _secondLineAngle = -filterAngle + filterAngleOffset;
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
                        continue;
                    }

                    var firstLineValue = Math.Tan(_firstLineAngle) * (j - halfImageWidth);
                    var secondLineValue = Math.Tan(_secondLineAngle) * (j - halfImageWidth);

                    bool firstHalfStartAssert;
                    bool firstHalfEndAssert;
                    bool secondHalfStartAssert;
                    bool secondHalfEndAssert;

                    var currentValue = i - halfImageHeight;

                    if ((_firstLineAngle - Math.PI / 2 > EPSILON && _secondLineAngle - Math.PI / 2 < EPSILON) ||
                        (_firstLineAngle - 1.5 * Math.PI > EPSILON && _secondLineAngle - 1.5 * Math.PI < EPSILON))
                    {
                        firstHalfStartAssert = firstLineValue > currentValue;
                        firstHalfEndAssert = secondLineValue > currentValue;
                        secondHalfStartAssert = secondLineValue < currentValue;
                        secondHalfEndAssert = firstLineValue < currentValue;
                    }
                    else if (_firstLineAngle - Math.PI > EPSILON && _firstLineAngle - 1.5 * Math.PI < EPSILON)
                    {
                        firstHalfStartAssert = firstLineValue > currentValue;
                        firstHalfEndAssert = secondLineValue < currentValue;
                        secondHalfStartAssert = firstLineValue < currentValue;
                        secondHalfEndAssert = secondLineValue > currentValue;
                    }
                    else
                    {
                        firstHalfStartAssert = firstLineValue < currentValue;
                        firstHalfEndAssert = secondLineValue > currentValue;
                        secondHalfStartAssert = firstLineValue > currentValue;
                        secondHalfEndAssert = secondLineValue < currentValue;
                    }

                    if (!((firstHalfStartAssert && firstHalfEndAssert) || (secondHalfStartAssert && secondHalfEndAssert)))
                    {
                        complexData[i][j] = 0;
                    }
                }
            }
        }
    }
}
