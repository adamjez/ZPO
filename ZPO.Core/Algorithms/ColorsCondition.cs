using System;
using System.Collections.Generic;
using System.Linq;
using ZPO.Core.Colors;

namespace ZPO.Core.Algorithms
{
    public class ColorsCondition : IRegionGrowingCondition
    {
        private readonly uint toleranceMultiplier;
        private readonly List<IColor> compareColors;
        private readonly uint tolerance;

        public ColorsCondition(List<IColor> compareColors, uint tolerance, uint toleranceMultiplier = 1)
        {
            this.compareColors = compareColors;
            this.tolerance = tolerance;
            this.toleranceMultiplier = toleranceMultiplier;
        }

        public bool Compare(IColor pixelColor, int neighborCount)
        {
            var multiplier = neighborCount > 0 ? toleranceMultiplier : 1;

            return compareColors
                .Any(color => Math.Abs(pixelColor.Difference(color)) <= tolerance*multiplier);
        }

    }
}