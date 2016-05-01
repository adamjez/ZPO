using System;
using System.Collections.Generic;
using System.Linq;
using ZPO.Core.Colors;

namespace ZPO.Core.Conditions
{
    /// <summary>
    /// Condition accepts colors to compare. 
    /// When comparing, True is returned when, some difference of pixel color 
    /// and compare colors is smaller than given tolerance. Difference of colors
    /// may be implemented in every color space differently
    /// </summary>
    public class ArithmeticalDistanceCondition : BaseCondition
    {
        protected readonly double NeighborTolerance;
        protected readonly List<IColor> CompareColors;
        protected readonly double Tolerance;

        public ArithmeticalDistanceCondition(List<IColor> compareColors, double tolerance, double neighborTolerance, bool dynamicThreshold = false)
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