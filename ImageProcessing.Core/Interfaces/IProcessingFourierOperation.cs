using System.Numerics;

namespace ImageProcessing.Core.Interfaces
{
    public interface IProcessingFourierOperation
    {
        unsafe void ProcessImage(Complex[][] complexData);
    }
}
