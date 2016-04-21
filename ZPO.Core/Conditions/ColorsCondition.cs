using System;
using System.Collections.Generic;
using System.Linq;
using ZPO.Core.Colors;

namespace ZPO.Core.Conditions
{
    public class ColorsCondition : IRegionGrowingCondition
    {
        protected readonly uint ToleranceMultiplier;
        protected readonly List<IColor> CompareColors;
        protected readonly uint Tolerance;

        public ColorsCondition(List<IColor> compareColors, uint tolerance, uint toleranceMultiplier = 1)
        {
            this.CompareColors = compareColors;
            this.Tolerance = tolerance;
            this.ToleranceMultiplier = toleranceMultiplier;
        }

        public virtual bool Compare(IColor pixelColor, int neighborCount)
        {
            var multiplier = neighborCount > 0 ? ToleranceMultiplier : 1;

            return CompareColors
                .Any(color => Math.Abs(pixelColor.Difference(color)) <= Tolerance*multiplier);
        }
    }
}