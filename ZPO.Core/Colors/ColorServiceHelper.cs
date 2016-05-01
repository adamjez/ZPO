using System;

namespace ZPO.Core.Colors
{
    public static class ColorServiceHelper
    {
        public static IColor Add(this IColor value1, IColor value2)
        {
            if (value1 is RGBColor && value2 is RGBColor)
            {
                return RGBColorService.Add((RGBColor)value1, (RGBColor)value2);
            }

            throw new ArgumentException();
        }

        public static int Difference(this IColor value1, IColor value2)
        {
            if (value1 is RGBColor && value2 is RGBColor)
            {
                return RGBColorService.Difference((RGBColor)value1, (RGBColor)value2);
            }

            if (value1 is HSLColor && value2 is HSLColor)
            {
                return HSLColorService.Difference((HSLColor)value1, (HSLColor)value2);
            }

            if (value1 is CIELabColor && value2 is CIELabColor)
            {
                return CIELabColorService.Difference((CIELabColor)value1, (CIELabColor)value2);
            }

            if (value1 is GrayScaleColor && value2 is GrayScaleColor)
            {
                return GrayScaleColorService.Difference((GrayScaleColor)value1, (GrayScaleColor)value2);
            }

            throw new ArgumentException("Arguments doesn't match");
        }



        private static readonly RGBColorService RGBColorService = new RGBColorService();
        private static readonly HSLColorService HSLColorService = new HSLColorService();
        private static readonly CIELabColorService CIELabColorService = new CIELabColorService();
        private static readonly GrayScaleColorService GrayScaleColorService = new GrayScaleColorService();
    }
}