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

            throw new ArgumentException();
        }



        private static readonly RGBColorService RGBColorService = new RGBColorService();
        //private static CMYKColorService CMYKColorService = new CMYKColorService();
        public static RGBColor Add(this RGBColor value1, RGBColor value2)
        {
            return RGBColorService.Add(value1, value2);
        }

        public static int Difference(this RGBColor value1, RGBColor value2)
        {
            return RGBColorService.Difference(value1, value2);
        }
    }
}