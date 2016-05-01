using System;
using Windows.UI;

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
            else if (colorSpace == ColorSpaces.GrayScale)
                return ColorExtensions.ToColor(value).ToGrayScaleColor();
            throw new ArgumentException();
        }
    }
}