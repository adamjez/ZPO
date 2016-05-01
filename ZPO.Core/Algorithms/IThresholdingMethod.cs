using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using ZPO.Core.Conditions;

namespace ZPO.Core.Algorithms
{
    public interface IThresholdingMethod
    {
        List<IColorCondition> Conditions { get; }
        Task<WriteableBitmap> ProcessAsync(NeighborhoodType type);
    }
}