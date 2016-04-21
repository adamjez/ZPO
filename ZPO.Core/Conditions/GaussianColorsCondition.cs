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
            var a = 1/(Deviation.X*Math.Sqrt(2*Math.PI));
            var b = -Math.Pow(pixelColor.GetFirstPart() - Mean.X, 2)/(2*Math.Pow(Deviation.X, 2));

            var result = a*Math.Exp(b);
            return result > threshold;
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