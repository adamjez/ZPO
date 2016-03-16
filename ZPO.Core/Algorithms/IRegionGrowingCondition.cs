using ZPO.Core.Colors;

namespace ZPO.Core.Algorithms
{
    public interface IRegionGrowingCondition
    {
        bool Compare(IColor pixelCoor, bool isNeighbor, int multiplier);
    }
}