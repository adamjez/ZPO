﻿using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace ZPO.Core
{
    public static class BitmapExtensions
    {
        public static ColorContext GetColorContext(this WriteableBitmap bitmap, ColorCreator colorCreator)
        {
            return new ColorContext(bitmap, colorCreator);
        }

        public static WriteableBitmap CreateCopy(this WriteableBitmap bitmap)
        {
            return BitmapFactory.New(bitmap.PixelWidth, bitmap.PixelHeight);
        }
    }
}