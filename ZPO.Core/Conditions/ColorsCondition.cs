using System;
using System.Collections.Generic;
using System.Linq;
using ZPO.Core.Colors;

namespace ZPO.Core.Conditions
{
    public class ColorsCondition : BaseCondition
    {
        protected readonly double NeighborTolerance;
        protected readonly List<IColor> CompareColors;
        protected readonly double Tolerance;

        public ColorsCondition(List<IColor> compareColors, double tolerance, double neighborTolerance, bool dynamicThreshold = false)
            : base(dynamicThreshold)
        {
            this.CompareColors = compareColors;
            this.Tolerance = tolerance;
            this.NeighborTolerance = neighborTolerance;
        }

        public override bool Compare(IColor pixelColor, int neighborCount, double rowRatio = -1)
        {
            var currentTolerance = Tolerance + neighborCount*NeighborTolerance;
            if (DynamicThreshold && rowRatio >= 0)
            {
                currentTolerance = GetMultiplier(rowRatio) + neighborCount * NeighborTolerance;
            }

            return CompareColors
                .Any(color => Math.Abs(pixelColor.Difference(color)) <= currentTolerance);
        }
    }
}