using ZPO.Core.Colors;

namespace ZPO.Core.Conditions
{
    public abstract class ColorCondition : BaseCondition
    {
        protected IColor Color;
        protected double Tolerance;
        protected double NeighborTolerance;

        protected ColorCondition(IColor color, double tolerance, double neighborTolerance, bool dynamicThreshold)
            : base(dynamicThreshold)
        {
            Color = color;
            Tolerance = tolerance;
            NeighborTolerance = neighborTolerance;
        }
    }
}