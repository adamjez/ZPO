﻿using Windows.UI.Xaml.Media.Imaging;

namespace ZPO.Core
{
    public static class BitmapExtensions
    {
        public static WriteableBitmap CreateCopy(this WriteableBitmap bitmap)
        {
            return BitmapFactory.New(bitmap.PixelWidth, bitmap.PixelHeight);
        }
    }
}
