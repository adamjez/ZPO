using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Media.Imaging;
using ZPO.Core.Colors;

namespace ZPO.Core
{
    public class ColorContext : IEnumerable<IColor>
    {
        private readonly WriteableBitmap bitmap;
        private readonly ColorCreator colorCreator;

        public ColorContext(WriteableBitmap bitmap, ColorCreator colorCreator)
        {
            this.bitmap = bitmap;
            this.colorCreator = colorCreator;
        }

        public IEnumerator<IColor> GetEnumerator()
        {
            return new ColorEnumerator(bitmap, colorCreator);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}