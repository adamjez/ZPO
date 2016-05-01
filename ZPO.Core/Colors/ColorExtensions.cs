using System;
using Windows.UI;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace ZPO.Core.Colors
{
    /// <summary>
    /// Contains methods for creating color of given type from int value
    /// Contains static methods for flagging and retrieving flags from int type color
    /// </summary>
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
                case ColorSpaces.CIELab:
                    return ToCIELabColor(color);
                case ColorSpaces.GrayScale:
                    return ToGrayScaleColor(color);
                default:
                    throw new NotImplementedException("Given color space is not implemented");
            }
        }

        public static Color ToColor(int color)
        {
            return new Color()
            {
                R = (byte)((color >> 16) & 0x000000FF),
                G = (byte)((color >> 8) & 0x000000FF),
                B = (byte)((color) & 0x000000FF)
            };
        }

        public static GrayScaleColor ToGrayScaleColor(this Color color)
        {
            return new GrayScaleColor()
            {
                Intensity = (color.R * 6966 + color.G * 23436 + color.B * 2366) >> 15
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

        public static CIELabColor ToCIELabColor(this Color color)
        {
            // Source: http://www.easyrgb.com/index.php?X=MATH&H=07#text7
            var r = color.R / 255.0;
            var g = color.G / 255.0;
            var b = color.B / 255.0;

            if (r > 0.04045) r = Math.Pow(((r + 0.055) / 1.055), 2.4);
            else r = r / 12.92;
            if (g > 0.04045) g = Math.Pow(((g + 0.055) / 1.055), 2.4);
            else g = g / 12.92;
            if (b > 0.04045) b = Math.Pow(((b + 0.055) / 1.055), 2.4);
            else b = b / 12.92;

            r = r * 100;
            g = g * 100;
            b = b * 100;

            var X = r * 0.4124 + g * 0.3576 + b * 0.1805;
            var Y = r * 0.2126 + g * 0.7152 + b * 0.0722;
            var Z = r * 0.0193 + g * 0.1192 + b * 0.9505;

            var x = X / 95.047;
            var y = Y / 100.000;
            var z = Z / 108.883;

            if (x > 0.008856) x = Math.Pow(x, 1.0 / 3.0);
            else x = (7.787 * x) + (16.0 / 116.0);
            if (y > 0.008856) y = Math.Pow(y, 1.0 / 3.0);
            else y = (7.787 * y) + (16.0 / 116.0);
            if (z > 0.008856) z = Math.Pow(z, 1.0 / 3.0);
            else z = (7.787 * z) + (16.0 / 116.0);

            return new CIELabColor()
            {
                L = (116 * y) - 16,
                A = 500 * (x - y),
                B = 200 * (y - z)
            };
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

        public static int AddSeenFlag()
        {
            return (9 << 24) | 0;
        }

        public static bool IsSeenFlagged(this int color)
        {
            var alpha = (color >> 24) & 0x000000FF;

            return alpha == 9;
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
