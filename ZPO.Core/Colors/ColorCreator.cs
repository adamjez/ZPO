using System;

namespace ZPO.Core.Colors
{
    public class ColorCreator
    {
        private readonly ColorSpaces colorSpace;

        public ColorCreator(ColorSpaces colorSpace)
        {
            this.colorSpace = colorSpace;
        }

        public IColor Create(int value)
        {
            if(colorSpace == ColorSpaces.RGB)
                return ColorExtensions.ToColor(value).ToRGBColor();
            else if(colorSpace == ColorSpaces.HSL)
                return ColorExtensions.ToColor(value).ToHSLColor();
            else if (colorSpace == ColorSpaces.CIELab)
                return ColorExtensions.ToColor(value).ToCIELabColor();
            throw new ArgumentException();
        }
    }
}