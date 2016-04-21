using System;
using System.Collections.Generic;
using System.Linq;
using ZPO.Core.Colors;

namespace ZPO.Core.Conditions
{
    public class GaussianColorsCondition : ColorsCondition
    {
        private double threshold = 0.005;
        protected Point3D Mean;
        protected Point3D Deviation;
        public GaussianColorsCondition(List<IColor> compareColors, uint tolerance, uint toleranceMultiplier = 1)
            : base(compareColors, tolerance, toleranceMultiplier)
        {
            // Create gaussian model
            Mean = new Point3D
            {
                X = compareColors.Select(color => color.GetFirstPart()).Average(),
                Y = compareColors.Select(color => color.GetSecondPart()).Average(),
                Z = compareColors.Select(color => color.GetThirdPart()).Average()
            };

            Deviation = new Point3D()
            {
                X = ComputeDeviation(compareColors.Select(color => color.GetFirstPart()), Mean.X),
                Y = ComputeDeviation(compareColors.Select(color => color.GetSecondPart()), Mean.Y),
                Z = ComputeDeviation(compareColors.Select(color => color.GetThirdPart()), Mean.Z)
            };
        }

        public override bool Compare(IColor pixelColor, int neighborCount)
        {
            var result = GaussianFunction(pixelColor.GetFirstPart(), Mean.X, Deviation.X);
            result += GaussianFunction(pixelColor.GetSecondPart(), Mean.Y, Deviation.Y);
            result += GaussianFunction(pixelColor.GetThirdPart(), Mean.Z, Deviation.Z);


            return result > threshold;
        }

        private double GaussianFunction(int value, double mean, double deviation)
        {
            var a = 1 / (deviation * Math.Sqrt(2 * Math.PI));
            var b = -Math.Pow(value - mean, 2) / (2 * Math.Pow(deviation, 2));

            return a * Math.Exp(b);
        }

        private double ComputeDeviation(IEnumerable<int> values, double mean)
        {
            double accumulator = values.Sum(value => Math.Pow(value - mean, 2));

            return Math.Sqrt(accumulator); ;
        }
    }

    public struct Point3D
    {
        public double X;
        public double Y;
        public double Z;
    }
}