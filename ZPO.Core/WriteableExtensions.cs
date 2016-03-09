using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace ZPO.Core
{
    public static class WriteableExtensions
    {
        public static WriteableBitmap Add(this WriteableBitmap bmp1, WriteableBitmap bmp2)
        {
            if (bmp1.PixelHeight != bmp2.PixelHeight || bmp1.PixelWidth != bmp2.PixelHeight)
            {
                throw new ArgumentException("Bitmaps have to have same dimension", nameof(bmp2));
            }

            using (var srcContext1 = bmp1.GetBitmapContext(ReadWriteMode.ReadOnly))
            using (var srcContext2 = bmp2.GetBitmapContext(ReadWriteMode.ReadOnly))
            {
                var w = srcContext1.Width;
                var h = srcContext1.Height;
                var result = BitmapFactory.New(w, h);

                using (var resultContext = result.GetBitmapContext())
                {
                    for (var index = 0; index < h * w; index++)
                    {
                        var sumColor = new MyColor(srcContext1.Pixels[index]) + new MyColor(srcContext2.Pixels[index]);
                        resultContext.Pixels[index] = sumColor.ToInt();
                    }
                }

                return result;
            }
        }

        public static void RemoveAlphaChannel(this WriteableBitmap bmp)
        {
            bmp.ForEach((x, y, c) => Windows.UI.Color.FromArgb(255, c.R, c.G, c.B));
        }

        // Source: Upravene verze z knihovny WritableBitmapEx
        // Uprava: Nepocita se alpha channel a vysledna barva se da do absolutni hodnoty
        public static WriteableBitmap MyConvolute(this WriteableBitmap bmp, int[,] kernel, int kernelFactorSum)
        {
            var kh = kernel.GetUpperBound(0) + 1;
            var kw = kernel.GetUpperBound(1) + 1;

            if ((kw & 1) == 0)
            {
                throw new InvalidOperationException("Kernel width must be odd!");
            }
            if ((kh & 1) == 0)
            {
                throw new InvalidOperationException("Kernel height must be odd!");
            }

            using (var srcContext = bmp.GetBitmapContext(ReadWriteMode.ReadOnly))
            {
                var w = srcContext.Width;
                var h = srcContext.Height;
                var result = BitmapFactory.New(w, h);

                using (var resultContext = result.GetBitmapContext())
                {
                    var pixels = srcContext.Pixels;
                    var resultPixels = resultContext.Pixels;
                    var index = 0;
                    var kwh = kw >> 1;
                    var khh = kh >> 1;

                    for (var y = 0; y < h; y++)
                    {
                        for (var x = 0; x < w; x++)
                        {
                            var r = 0;
                            var g = 0;
                            var b = 0;

                            for (var kx = -kwh; kx <= kwh; kx++)
                            {
                                var px = kx + x;
                                // Repeat pixels at borders
                                if (px < 0)
                                {
                                    px = 0;
                                }
                                else if (px >= w)
                                {
                                    px = w - 1;
                                }

                                for (var ky = -khh; ky <= khh; ky++)
                                {
                                    var py = ky + y;
                                    // Repeat pixels at borders
                                    if (py < 0)
                                    {
                                        py = 0;
                                    }
                                    else if (py >= h)
                                    {
                                        py = h - 1;
                                    }

                                    var col = pixels[py * w + px];
                                    var k = kernel[ky + kwh, kx + khh];
                                    r += ((col >> 16) & 0x000000FF) * k;
                                    g += ((col >> 8) & 0x000000FF) * k;
                                    b += ((col) & 0x000000FF) * k;
                                }
                            }

                            var tr = Math.Abs(r / kernelFactorSum);
                            var tg = Math.Abs(g / kernelFactorSum);
                            var tb = Math.Abs(b / kernelFactorSum);

                            // Clamp to byte boundaries
                            var br = (byte)((tr > 255) ? 255 : ((tr < 0) ? 0 : tr));
                            var bg = (byte)((tg > 255) ? 255 : ((tg < 0) ? 0 : tg));
                            var bb = (byte)((tb > 255) ? 255 : ((tb < 0) ? 0 : tb));

                            resultPixels[index++] = (255 << 24) | (br << 16) | (bg << 8) | (bb);
                        }
                    }
                    return result;
                }
            }
        }
    }
}
