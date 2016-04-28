using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZPO.Core
{
    public static class StackExtensions
    {
        public static void PushRange<T>(this Stack<T> stack, IEnumerable<T> range)
        {
            foreach (var item in range)
                stack.Push(item);
        }

    }
}
