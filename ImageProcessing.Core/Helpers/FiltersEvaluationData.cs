namespace ImageProcessing.Core.Helpers
{
    public class FiltersEvaluationData
    {
        public double Mse { get; set; }
        public double Psnr { get; set; }
        public double Mae { get; set; }

        public override string ToString()
        {
            return $"MSE: {Mse}, PSNR: {Psnr}, MAE: {Mae}";
        }
    }
}