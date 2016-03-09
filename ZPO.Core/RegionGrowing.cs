using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace ZPO.Core
{
    public interface IRegionGrowingCondition
    {
    }
    public class RegionGrowing
    {
        private List<IRegionGrowingCondition> _conditions = new List<IRegionGrowingCondition>();
        private WriteableBitmap _bitmap;

        public RegionGrowing(WriteableBitmap bitmap)
        {
            _bitmap = bitmap;
        }
        public WriteableBitmap Process(NeighboursType type, Color color, uint tolerance)
        {
            var mycolor = color.ToMyColor();

            using (var srcContext = _bitmap.GetBitmapContext(ReadWriteMode.ReadOnly))
            {
                var w = srcContext.Width;
                var h = srcContext.Height;
                var result = BitmapFactory.New(w, h);

                using (var resultContext = result.GetBitmapContext())
                {
                    var changing = true;
                    while (changing)
                    {
                        changing = false;
                        var index = 0;

                        for (var y = 0; y < h; y++)
                        {
                            for (var x = 0; x < w; x++)
                            {
                                int resultColor = 0;
                                var resultMyColor = new MyColor(srcContext.Pixels[index]);
                                if (Math.Abs(resultMyColor - mycolor) < tolerance || resultContext.Pixels[index] != 0)
                                {
                                    //if (resultContext.Pixels[index] == 0)
                                    {
                                        resultColor = MyColor.White().ToInt();

                                        // White color
                                        if (Grow(srcContext.Pixels[index], resultContext.Pixels[index], mycolor, tolerance))
                                        {
                                            changing = true;
                                        }

                                        // Grow to neiboorhg
                                        if (x > 0 && y > 0 && x < w - 1 && y < h - 1)
                                        {
                                            if (Grow(srcContext.Pixels[index - 1], resultContext.Pixels[index - 1], mycolor, tolerance * 10))
                                            {
                                                resultContext.Pixels[index - 1] = MyColor.White().ToInt();
                                                changing = true;
                                            }

                                            if (Grow(srcContext.Pixels[index + 1], resultContext.Pixels[index + 1], mycolor, tolerance * 10))
                                            {
                                                resultContext.Pixels[index + 1] = MyColor.White().ToInt();
                                                changing = true;
                                            }

                                            if (Grow(srcContext.Pixels[index + w], resultContext.Pixels[index + w], mycolor, tolerance * 10))
                                            {
                                                resultContext.Pixels[index + w] = MyColor.White().ToInt();
                                                changing = true;
                                            }

                                            if (Grow(srcContext.Pixels[index - w], resultContext.Pixels[index - w], mycolor, tolerance * 10))
                                            {
                                                resultContext.Pixels[index - w] = MyColor.White().ToInt();
                                                changing = true;
                                            }
                                        }
                                    }   
                                }
                                else
                                {
                                    if(resultContext.Pixels[index] == 0)
                                        resultColor = MyColor.Black().ToInt();
                                }
                                if (resultContext.Pixels[index] == 0)
                                    resultContext.Pixels[index] = resultColor;

                                index++;
                            }
                        }
                    }
                }
                return result;
            }
        }

        public void AddCondition(IRegionGrowingCondition condition)
        {
            _conditions.Add(condition);
        }

        private bool Grow(int color1, int color2, MyColor compare, uint tolerance)
        {
            var tmpColor = new MyColor(color1);

            if (color2 == MyColor.White().ToInt())
                return false;

            if (Math.Abs(tmpColor - compare) < tolerance)
                return true;

            return false;
        }

    }

    public enum NeighboursType
    {
        Four,
        Eight
    }
}
