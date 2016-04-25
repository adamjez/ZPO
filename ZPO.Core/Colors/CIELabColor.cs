using System;
using MathNet.Numerics.LinearAlgebra.Double;

namespace ZPO.Core.Colors
{
    public class CIELabColor : IColor
    {

        public double L { get; set; }
        public double A { get; set; }
        public double B { get; set; }
        public int ToInt()
        {
            var y = (L + 16) / 116;
            var x = A / 500 + y;
            var z = y - B / 200;

            if (Math.Pow(y, 3) > 0.008856) y = Math.Pow(y, 3);
            else y = (y - 16.0 / 116.0) / 7.787;
            if (Math.Pow(x, 3) > 0.008856) x = Math.Pow(x, 3);
            else x = (x - 16.0 / 116.0) / 7.787;
            if (Math.Pow(z, 3) > 0.008856) z = Math.Pow(z, 3);
            else z = (z - 16.0 / 116.0) / 7.787;

            x = 95.047 * x;
            y = 100.000 * y;
            z = 108.883 * z;

            var x2 = x / 100;
            var y2 = y / 100;
            var z2 = z / 100;

            var r = x2*3.2406 + y2*-1.5372 + z2*-0.4986;
            var g = x2*-0.9689 + y2*1.8758 + z2*0.0415;
            var b = x2*0.0557 + y2*-0.2040 + z2*1.0570;

            if (r > 0.0031308) r = 1.055*Math.Pow(r, 1/2.4) - 0.055;
            else r = 12.92*r;
            if (g > 0.0031308) g = 1.055* Math.Pow(g, 1 / 2.4) - 0.055;
            else g = 12.92*g;
            if (b > 0.0031308) b = 1.055* Math.Pow(b, 1 / 2.4) - 0.055;
            else b = 12.92*b;

            var red = Convert.ToByte(r * 255.0f);
            var green = Convert.ToByte(g * 255.0f);
            var blue = Convert.ToByte(b * 255.0f);

            return (255 << 24) | (red << 16) | (green << 8) | (blue);
        }

        public Vector GetParts() => DenseVector.OfArray(new double[] {A, B });
    }
}