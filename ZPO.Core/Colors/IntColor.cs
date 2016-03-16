using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZPO.Core.Colors
{
    public static class IntColor
    {
        public static int CreateWhite()
        {
            return (255 << 24) | (255 << 16) | (255 << 8) | (255);
        }

        public static int CreateBlack()
        {
            return (255 << 24) | (0 << 16) | (0 << 8) | (0);
        }
    }
}
