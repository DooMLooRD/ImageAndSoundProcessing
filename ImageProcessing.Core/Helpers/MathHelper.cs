namespace ImageProcessing.Core.Helpers
{
    class MathHelper
    {
        public static double CalculateLinearIntercept(double slope)
        {
            return 128 - (slope * 128);
        }
    }
}
