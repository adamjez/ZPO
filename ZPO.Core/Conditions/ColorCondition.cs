using System;
using ZPO.Core.Algorithms;
using ZPO.Core.Colors;

namespace ZPO.Core.Conditions
{
    public class ColorCondition : IColorCondition
    {
        protected readonly double NeighborTolerance;
        protected readonly IColor CompareColor;
        protected readonly double Tolerance;

        public ColorCondition(IColor compareColor, double tolerance, double neighborTolerance = 0)
        {
            this.CompareColor = compareColor;
            this.Tolerance = tolerance;
            this.NeighborTolerance = neighborTolerance;
        }

        public virtual bool Compare(IColor pixelColor, int neighborCount, double row = -1)
        {
            var multiplier = neighborCount > 0 ? NeighborTolerance : 1;
            return Math.Abs(pixelColor.Difference(CompareColor)) <= Tolerance*multiplier;
        }
    }
}