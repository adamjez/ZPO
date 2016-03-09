﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZPO.Core
{
    internal class MyColor
    {
        private int _red;
        private int _green;
        private int _blue;

        public MyColor()
        {   }

        public MyColor(int color)
        {
            Red += (color >> 16) & 0x000000FF;
            Green += (color >> 8) & 0x000000FF;
            Blue += (color) & 0x000000FF;
        }

        public int Red
        {
            get { return _red; }
            set
            {
                _red = toValidColor(value);
            }
        }
        public int Green
        {
            get { return _green; }
            set
            {
                _green = toValidColor(value);
            }
        }
        public int Blue {
            get { return _blue; }
            set
            {
                _blue = toValidColor(value);
            }
        }

        public int ToInt()
        {
            return (255 << 24) | (Red << 16) | (Green << 8) | (Blue);
        }

        public static MyColor operator +(MyColor color1, MyColor color2)
        {
            var result = new MyColor();
            result.Red = (color1.Red + color2.Red) / 2;
            result.Green = (color1.Green + color2.Green) / 2;
            result.Blue = (color1.Blue + color2.Blue) / 2;

            return result;
        }


        private int toValidColor(int color)
        {
            return ((color > 255) ? 255 : ((color < 0) ? 0 : color));
        }
    }
}
