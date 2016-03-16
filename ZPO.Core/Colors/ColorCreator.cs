using System;
using ZPO.Core.Colors;

namespace ZPO.Core
{
    public class ColorCreator
    {
        private readonly ColorTypes colorType;

        public ColorCreator(ColorTypes colorType)
        {
            this.colorType = colorType;
        }

        public IColor Create(int value)
        {
            if(colorType == ColorTypes.RGB)
                return new RGBColor(value);

            throw new ArgumentException();
        }
    }
}