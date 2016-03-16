using System;
using ZPO.Core.Colors;

namespace ZPO.Core.Algorithms
{
    public class ColorCondition : IRegionGrowingCondition
    {
        private readonly uint toleranceMultiplier;
        private readonly IColor compareColor;
        private readonly uint tolerance;

        public ColorCondition(IColor compareColor, uint tolerance, uint toleranceMultiplier = 1)
        {
            this.compareColor = compareColor;
            this.tolerance = tolerance;
            this.toleranceMultiplier = toleranceMultiplier;
        }

        public bool Compare(IColor pixelColor, bool isNeighbor, int mul)
        {
            var multiplier = isNeighbor ? toleranceMultiplier * mul : 1;
            return Math.Abs(pixelColor.Difference(compareColor)) <= tolerance*multiplier;
        }

    }
}