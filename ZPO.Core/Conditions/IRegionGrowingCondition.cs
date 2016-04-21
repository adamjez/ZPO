using ZPO.Core.Colors;

namespace ZPO.Core.Conditions
{
    public interface IRegionGrowingCondition
    {
        bool Compare(IColor pixelCoor, int neighborCount);
    }
}