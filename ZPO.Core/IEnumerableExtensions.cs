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

        public static int FindClosestToLeft(this IEnumerable<int> list, int item)
        {
            return list.TakeWhile(p => p < item).LastOrDefault();
        }

        public static int FindClosestToRight(this IEnumerable<int> list, int item, int max)
        {
            var result = list.SkipWhile(p => p <= item).FirstOrDefault();
            return result == 0 ? max : result;
        }

        public static int? FindClosestBetween(this List<int> list, int item1, int item2)
        {
            var result = list.FirstOrDefault(p => item1 < p && p < item2);
            return result == 0 ? (int?) null : result;
        }
    }
}
