using ImageProcessing.Core.Model;
using System.Numerics;

namespace ImageProcessing.Core.Helpers
{
    public class MathHelper
    {
        public static double CalculateLinearIntercept(double slope)
        {
            return 128 - (slope * 128);
        }

        public static (double Min, double Max) CalculateMinMax(Complex[][] data, Spectrum spectrum)
        {
            var result = (Min: double.MaxValue, Max: double.MinValue);
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data[i].Length; j++)
                {
                    var value = 0.0;
                    switch (spectrum)
                    {
                        case Spectrum.Phase:
                            value = data[i][j].Phase;
                            break;
                        case Spectrum.Magnitude:
                            value = data[i][j].Magnitude;
                            break;
                    }
                    result.Min = value < result.Min ? value : result.Min;
                    result.Max = value > result.Max ? value : result.Max;
                }
            }

            return result;
        }
    }
}
