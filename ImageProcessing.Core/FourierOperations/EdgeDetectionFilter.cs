using ImageProcessing.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ImageProcessing.Core.FourierOperations
{
    public class EdgeDetectionFilter : IProcessingFourierOperation
    {
        private const double NOISE = 0.0001;
        private const double EPISLON = 0.0000001;

        public double FilterAngle { get; set; }
        public double FilterAngleOffset { get; set; }
        public int FilterRadius { get; set; }


        private double _firstHalfStartAngle;
        private double _firstHalfEndAngle;
        private double _secondHalfStartAngle;
        private double _secondHalfEndAngle;

        public EdgeDetectionFilter(double filterAngle, double filterAngleOffset, int filterRadius)
        {
            filterAngleOffset %= 180;
            filterAngleOffset = filterAngleOffset > 90 ? filterAngleOffset - 180 : filterAngleOffset;

            FilterAngle = (filterAngle + NOISE) * 2 * Math.PI / 360;
            FilterAngleOffset = (filterAngleOffset + NOISE) * 2 * Math.PI / 360;
            FilterRadius = filterRadius;

            _firstHalfStartAngle = FilterAngle + FilterAngleOffset;
            _firstHalfEndAngle = Math.PI * 2 - FilterAngle + FilterAngleOffset;
            _secondHalfStartAngle = Math.PI - FilterAngle + FilterAngleOffset;
            _secondHalfEndAngle = Math.PI + FilterAngle + FilterAngleOffset;
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

                    var widthFactor = Math.Pow((halfImageWidth - j) / FilterRadius, 2);
                    var heightFactor = Math.Pow((halfImageHeight - i) / FilterRadius, 2);

                    if (Math.Sqrt(widthFactor + heightFactor) < 1)
                    {
                        complexData[i][j] = 0;
                        continue;
                    }

                    var firstHalfStart = Math.Tan(_firstHalfStartAngle) * (j - halfImageWidth);
                    var firstHalfEnd = Math.Tan(_firstHalfEndAngle) * (j - halfImageWidth);
                    var secondHalfStart = Math.Tan(_secondHalfStartAngle) * (j - halfImageWidth);
                    var secondHalfEnd = Math.Tan(_secondHalfEndAngle) * (j - halfImageWidth);

                    bool firstHalfStartAssert;
                    bool firstHalfEndAssert;
                    bool secondHalfStartAssert;
                    bool secondHalfEndAssert;

                    var currentValue = i - halfImageHeight;

                    if (_firstHalfStartAngle - Math.PI / 2 < EPISLON && _secondHalfStartAngle - Math.PI / 2 > EPISLON)
                    {
                        firstHalfStartAssert = firstHalfStart < currentValue;
                        firstHalfEndAssert = firstHalfEnd > currentValue;
                        secondHalfStartAssert = secondHalfStart < currentValue;
                        secondHalfEndAssert = secondHalfEnd > currentValue;
                    }
                    else
                    {
                        firstHalfStartAssert = firstHalfStart > currentValue;
                        firstHalfEndAssert = secondHalfStart > currentValue;
                        secondHalfStartAssert = firstHalfEnd < currentValue;
                        secondHalfEndAssert = secondHalfEnd < currentValue;
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
