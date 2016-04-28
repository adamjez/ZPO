using System;

namespace ZPO.Core.Colors
{
    public class HSLColorService : IColorService<HSLColor>
    {
        public HSLColor Add(HSLColor value1, HSLColor value2)
        {
            throw new NotImplementedException();
        }

        public int Difference(HSLColor value1, HSLColor value2)
        {
            return (int)(((Math.Abs(value1.Hue - value2.Hue) +
                Math.Abs(value1.Saturation - value2.Saturation)  +
                Math.Abs(value1.Lightness - value2.Lightness) / 2) * 255) * 2);

        }
    }
}