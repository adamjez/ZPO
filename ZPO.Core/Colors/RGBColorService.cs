using System;

namespace ZPO.Core.Colors
{
    public class RGBColorService : IColorService<RGBColor>
    {
        public RGBColor Add(RGBColor value1, RGBColor value2)
        {
            return new RGBColor
            {
                Red = (value1.Red + value2.Red) / 2,
                Green = (value1.Green + value2.Green) / 2,
                Blue = (value1.Blue + value2.Blue) / 2
            };
        }

        public int Difference(RGBColor value1, RGBColor value2)
        {
            return Math.Abs(value1.Red - value2.Red) + 
                Math.Abs(value1.Green - value2.Green) + 
                Math.Abs(value1.Blue - value2.Blue);
        }
    }
}