using System;
using ZPO.Core.Algorithms;
using ZPO.Core.Colors;

namespace ZPO.Core.Conditions
{
    public class ColorCondition : IRegionGrowingCondition
    {
        private readonly double neighborTolerance;
        private readonly IColor compareColor;
        private readonly double tolerance;

        public ColorCondition(IColor compareColor, double tolerance, double neighborTolerance = 0)
        {
            this.compareColor = compareColor;
            this.tolerance = tolerance;
            this.neighborTolerance = neighborTolerance;
        }

        public bool Compare(IColor pixelColor, int neighborCount)
        {
            var multiplier = neighborCount > 0 ? neighborTolerance : 1;
            return Math.Abs(pixelColor.Difference(compareColor)) <= tolerance*multiplier;
        }
    }
}