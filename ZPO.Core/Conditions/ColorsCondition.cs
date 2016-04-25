using System;
using System.Collections.Generic;
using System.Linq;
using ZPO.Core.Colors;

namespace ZPO.Core.Conditions
{
    public class ColorsCondition : IRegionGrowingCondition
    {
        protected readonly double NeighborTolerance;
        protected readonly List<IColor> CompareColors;
        protected readonly double Tolerance;

        public ColorsCondition(List<IColor> compareColors, double tolerance, double neighborTolerance = 1)
        {
            this.CompareColors = compareColors;
            this.Tolerance = tolerance;
            this.NeighborTolerance = neighborTolerance;
        }

        public virtual bool Compare(IColor pixelColor, int neighborCount)
        {
            var multiplier = neighborCount > 0 ? NeighborTolerance : 1;

            return CompareColors
                .Any(color => Math.Abs(pixelColor.Difference(color)) <= Tolerance*multiplier);
        }
    }
}