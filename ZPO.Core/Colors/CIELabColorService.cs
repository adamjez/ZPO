using System;

namespace ZPO.Core.Colors
{
    public class CIELabColorService : IColorService<CIELabColor>
    {
        public CIELabColor Add(CIELabColor value1, CIELabColor value2)
        {
            throw new NotImplementedException();
        }

        public int Difference(CIELabColor value1, CIELabColor value2)
        {
            return (int)(Math.Abs(value1.A - value2.A) + Math.Abs(value1.B - value2.B)+
                Math.Abs(value1.L - value2.L)/2);
        }
    }
}