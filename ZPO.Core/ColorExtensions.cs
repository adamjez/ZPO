using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace ZPO.Core
{
    public static class ColorExtensions
    {
        public static MyColor ToMyColor(this Color color)
        {
            var result = new MyColor();
            result.Red = color.R;
            result.Green = color.G;
            result.Blue = color.B;

            return result;
        }
    }
}
