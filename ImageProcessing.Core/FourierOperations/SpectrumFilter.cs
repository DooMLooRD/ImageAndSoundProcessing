using ImageProcessing.Core.Interfaces;
using System;
using System.Numerics;

namespace ImageProcessing.Core.FourierOperations
{
    public class SpectrumFilter : IProcessingFourierOperation
    {
        private readonly int _l;
        private readonly int _k;

        public SpectrumFilter(int l, int k)
        {
            _l = l;
            _k = k;
        }

        public void ProcessImage(Complex[][] complexData)
        {
            double imageHeight = complexData.Length;
            double imageWidth = complexData[0].Length;

            for (int i = 0; i < imageHeight; i++)
            {
                for (int j = 0; j < imageWidth; j++)
                {
                    complexData[i][j] *= Complex.Exp(
                        new Complex(0,
                        (-i * _k * 2 * Math.PI / imageHeight) +
                        (-j * _l * 2 * Math.PI / imageWidth) +
                        (Math.PI * (_k + _l))));
                }
            }
        }
    }
}
