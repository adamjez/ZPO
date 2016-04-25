using MathNet.Numerics.LinearAlgebra.Double;

namespace ZPO.Core.Colors
{
    public interface IColor
    {
        int ToInt();
        Vector GetParts();
    }
}