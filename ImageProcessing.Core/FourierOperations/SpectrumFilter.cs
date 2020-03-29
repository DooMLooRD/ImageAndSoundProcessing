using ImageProcessing.Core.Interfaces;
using System;
using System.Numerics;

namespace ImageProcessing.Core.FourierOperations
{
    public class SpectrumFilter : IProcessingFourierOperation
    {
        public int L { get; set; }
        public int K { get; set; }

        public SpectrumFilter(int l, int k)
        {
            L = l;
            K = k;
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
                        (-i * K * 2 * Math.PI / imageHeight) +
                        (-j * L * 2 * Math.PI / imageWidth) +
                        (Math.PI * (K + L))));
                }
            }
        }
    }
}
