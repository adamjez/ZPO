using System;
using MathNet.Numerics.LinearAlgebra.Double;

namespace ZPO.Core.Colors
{
    public class HSLColor : IColor
    {
        public double Hue { get; set; }
        public double Saturation { get; set; }
        public double Lightness { get; set; }
        public int ToInt()
        {
            double v;
            double r, g, b;
            double h = Hue, sl = Saturation, l = Lightness;
            r = l;   // default to gray
            g = l;
            b = l;
            v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);
            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0;
                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }
            var red = Convert.ToByte(r * 255.0f);
            var green = Convert.ToByte(g * 255.0f);
            var blue = Convert.ToByte(b * 255.0f);

            return (255 << 24) | (red << 16) | (green << 8) | (blue);
        }

        public Vector GetParts() => DenseVector.OfArray(new double[] { Hue * 255, Saturation * 255/*, Lightness * 255*/ });
    }
}