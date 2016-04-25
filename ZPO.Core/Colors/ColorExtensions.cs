using System;
using Windows.UI;

namespace ZPO.Core.Colors
{
    public static class ColorExtensions
    {
        public static IColor ConvertTo(this Color color, ColorSpaces type)
        {
            switch (type)
            {
                case ColorSpaces.RGB:
                    return ToRGBColor(color);
                case ColorSpaces.HSL:
                    return ToHSLColor(color);
                default:
                    throw new NotImplementedException("Given color space is not implemented");
            }
        }

        public static Color ToColor(int color)
        {
            return new Color()
            {
                R = (byte) ((color >> 16) & 0x000000FF),
                G = (byte) ((color >> 8) & 0x000000FF),
                B = (byte) ((color) & 0x000000FF)
            };
        }

        public static RGBColor ToRGBColor(this Color color)
        {
            return new RGBColor
            {
                Red = color.R,
                Green = color.G,
                Blue = color.B
            };
        }

        public static HSLColor ToHSLColor(this Color color)
        {
            var result = new HSLColor();

            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;
            double v;
            double m;
            double vm;
            double r2, g2, b2;

            v = Math.Max(r, g);
            v = Math.Max(v, b);
            m = Math.Min(r, g);
            m = Math.Min(m, b);
            result.Lightness = (m + v) / 2.0;
            if (result.Lightness <= 0.0)
            {
                return result;
            }
            vm = v - m;
            result.Saturation = vm;
            if (result.Saturation > 0.0)
            {
                result.Saturation /= (result.Lightness <= 0.5) ? (v + m) : (2.0 - v - m);
            }
            else
            {
                return result;
            }
            r2 = (v - r) / vm;
            g2 = (v - g) / vm;
            b2 = (v - b) / vm;
            if (r == v)
            {
                result.Hue = (g == m ? 5.0 + b2 : 1.0 - g2);
            }
            else if (g == v)
            {
                result.Hue = (b == m ? 1.0 + r2 : 3.0 - b2);
            }
            else
            {
                result.Hue = (r == m ? 3.0 + g2 : 5.0 - r2);
            }
            result.Hue /= 6.0;

            return result;
        }

        public static bool IsFlagged(this int color)
        {
            var red = (color >> 16) & 0x000000FF;
            var green = (color >> 8) & 0x000000FF;
            var blue = (color) & 0x000000FF;

            return red == 255 && green == 255 && blue == 255;
        }

        public static bool IsNeighborFlagged(this int color)
        {
            var alpha = (color >> 24) & 0x000000FF;

            return alpha > 0 && alpha < 255;
        }

        public static int AddNeighborFlag(this int color)
        {
            return (color.GetNeighborMultiplier() + 1 << 24) | 0;
        }

        public static int GetNeighborMultiplier(this int color)
        {
            return (color >> 24) & 0x000000FF;
        }

        public static int Flag()
        {
            const byte red = 255;
            const byte green = 255;
            const byte blue = 255;

            return (255 << 24) | (red << 16) | (green << 8) | blue;
        }



        public static int ClearFlag()
        {
            const byte red = 0;
            const byte green = 0;
            const byte blue = 0;

            return (255 << 24) | (red << 16) | (green << 8) | blue;
        }
    }
}
