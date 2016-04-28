using System;
using System.Collections.Generic;
using System.Linq;
using ZPO.Core.Colors;

namespace ZPO.Core.Conditions
{
    public class ColorsCondition : IColorCondition
    {
        protected readonly double NeighborTolerance;
        protected readonly List<IColor> CompareColors;
        protected readonly double Tolerance;
        protected readonly bool DynamicThreshold;

        public ColorsCondition(List<IColor> compareColors, double tolerance, double neighborTolerance = 1, bool dynamicThreshold = false)
        {
            this.CompareColors = compareColors;
            this.Tolerance = tolerance;
            this.NeighborTolerance = neighborTolerance;
            this.DynamicThreshold = dynamicThreshold;
        }

        public virtual bool Compare(IColor pixelColor, int neighborCount, double row = -1)
        {
            var currentTolerance = Tolerance + neighborCount*NeighborTolerance;
            if (DynamicThreshold && row > 0)
            {
                currentTolerance = 100*row + neighborCount * NeighborTolerance;
            }

            return CompareColors
                .Any(color => Math.Abs(pixelColor.Difference(color)) <= currentTolerance);
        }
    }
}