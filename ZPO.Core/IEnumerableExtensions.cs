using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZPO.Core
{
    public static class IEnumerableExtensions
    {
        public static int FindClosest(this IEnumerable<int> list, int item)
        {
            return list.Aggregate((x, y) => Math.Abs(x - item) < Math.Abs(y - item) ? x : y);
        }
    }
}
