using System;
using Windows.UI.Xaml.Media.Imaging;

namespace ZPO.Core
{
    public static class ImageBlur
    {
        public static WriteableBitmap GaussianBlur(WriteableBitmap source)
        {
            int[,] kernel = { { 1, 2, 1 },
                              { 2, 8, 2 },
                              { 1, 2, 1 } };

            return source.MyConvolute(kernel, 20);
        }
    }
}