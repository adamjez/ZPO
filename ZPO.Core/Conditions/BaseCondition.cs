using System;
using ZPO.Core.Algorithms;
using ZPO.Core.Colors;

namespace ZPO.Core.Conditions
{
    public abstract class BaseCondition : IColorCondition
    {
        protected readonly bool DynamicThreshold;

        protected BaseCondition(bool dynamicThreshold)
        {
            this.DynamicThreshold = dynamicThreshold;
        }

        protected double GetMultiplier(double rowRatio)
        {
            return 100 * rowRatio;
        }

        public abstract bool Compare(IColor pixelColor, int neighborCount, double rowRatio = -1);
    }
}