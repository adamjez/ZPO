using System.Collections;
using System.Collections.Generic;
using Windows.UI.Xaml.Media.Imaging;
using ZPO.Core.Colors;

namespace ZPO.Core
{
    public class ColorEnumerator : IEnumerator<IColor>
    {
        private BitmapContext context;
        private int index;
        private readonly ColorCreator colorCreator;

        public ColorEnumerator(WriteableBitmap bitmap, ColorCreator colorCreator)
        {
            this.context = bitmap.GetBitmapContext();
            this.colorCreator = colorCreator;
        }

        public bool MoveNext()
        {
            var result = false;
            if (index + 1 < context.Length)
            {
                index++;
                result = true;
            }
            return result;
        }

        public void Reset()
        {
            index = 0;
        }

        public IColor Current => colorCreator.Create(context.Pixels[index]);

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            context.Dispose();
        }
    }
}