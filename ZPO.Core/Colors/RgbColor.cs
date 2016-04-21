﻿namespace ZPO.Core.Colors
{
    public class RGBColor : IColor
    {
        private int _red;
        private int _green;
        private int _blue;

        public RGBColor()
        { }

        public RGBColor(int color)
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
                _red = ToValidColor(value);
            }
        }
        public int Green
        {
            get { return _green; }
            set
            {
                _green = ToValidColor(value);
            }
        }
        public int Blue {
            get { return _blue; }
            set
            {
                _blue = ToValidColor(value);
            }
        }

        public int ToInt()
        {
            return (255 << 24) | (Red << 16) | (Green << 8) | (Blue);
        }

        public IColor FromInt(int value)
        {
            throw new System.NotImplementedException();
        }

        public int GetFirstPart() => Red;

        public int GetSecondPart() => Green;

        public int GetThirdPart() => Blue;

        public bool IsFlagged()
        {
            return Red == 255 && Green == 255 && Blue == 255;
        }

        public bool IsNeighborFlagged()
        {
            return Red == 1 && Green == 1 && Blue == 1;
        }

        public void Flag()
        {
            Red = 255; Green = 255; Blue = 255;
        }

        public void CreateBlack()
        {
            Red = 0; Green = 0; Blue = 0;
        }

        public void NeighborFlag()
        {
            Red = 1; Green = 1; Blue = 1;
        }

        private int ToValidColor(int color)
        {
            return (color > 255) ? 255 : ((color < 0) ? 0 : color);
        }
    }
 
}
