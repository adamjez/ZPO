using Windows.UI;

namespace ZPO.Core.Colors
{
    public static class ColorExtensions
    {
        public static RGBColor ToRGBColor(this Color color)
        {
            var result = new RGBColor
            {
                Red = color.R,
                Green = color.G,
                Blue = color.B
            };

            return result;
        }

        public static bool IsFlagged(this int color)
        {
            var red = (color >> 16) & 0x000000FF;
            var green = (color >> 8) & 0x000000FF;
            var blue = (color) & 0x000000FF;

            return red == 255 && green == 255 && blue == 255;
        }

        public static bool IsNeighborFlagged(this int color)
        {
            var alpha = (color >> 24) & 0x000000FF;

            return alpha > 0 && alpha < 255;
        }

        public static int AddNeighborFlag(this int color)
        {  
            return (color.GetNeighborMultiplier() + 1 << 24) | 0;
        }

        public static int GetNeighborMultiplier(this int color)
        {
            return (color >> 24) & 0x000000FF;
        }

        public static int Flag()
        {
            const byte red = 255;
            const byte green = 255;
            const byte blue = 255;

            return (255 << 24) | (red << 16) | (green << 8) | blue;
        }



        public static int ClearFlag()
        {
            const byte red = 0;
            const byte green = 0;
            const byte blue = 0;

            return (255 << 24) | (red << 16) | (green << 8) | blue;
        }
    }
}
