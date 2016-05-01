using System;

namespace ZPO.Core.Colors
{
    public class GrayScaleColorService : IColorService<GrayScaleColor>
    {
        public GrayScaleColor Add(GrayScaleColor value1, GrayScaleColor value2)
        {
            throw new NotImplementedException();
        }

        public int Difference(GrayScaleColor value1, GrayScaleColor value2)
        {
            return Math.Abs(value1.Intensity - value2.Intensity);
        }
    }
}