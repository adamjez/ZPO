using MathNet.Numerics.LinearAlgebra.Double;

namespace ZPO.Core.Colors
{
    public class GrayScaleColor : IColor
    {
        public int Intensity { get; set; }
        public int ToInt()
        {
            return (255 << 24) | (Intensity << 16) | (Intensity << 8) | (Intensity);
        }

        public Vector GetParts() => DenseVector.OfArray(new double[] { Intensity });
    }
}