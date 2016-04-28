using System;

namespace ZPO.Core.Colors
{
    public class CIELabColorService : IColorService<CIELabColor>
    {
        public CIELabColor Add(CIELabColor value1, CIELabColor value2)
        {
            //throw new NotImplementedException();
            return new CIELabColor {
                L = (value1.L + value2.L) / 2,
                A = (value1.A + value2.A) / 2,
                B = (value1.B + value2.B) / 2
            };
        }

        public int Difference(CIELabColor value1, CIELabColor value2)
        {
            //throw new NotImplementedException();
            return (int) (Math.Sqrt(Math.Pow(value1.L-value2.L,2) + Math.Pow(value1.A - value2.A, 2) + Math.Pow(value1.B - value2.B, 2)));
        }
    }
}