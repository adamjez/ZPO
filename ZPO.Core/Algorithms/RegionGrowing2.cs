using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using ZPO.Core.Colors;
using ZPO.Core.Conditions;

namespace ZPO.Core.Algorithms
{
    public class RegionGrowing2 : IRegionGrowing
    {
        private readonly ColorCreator colorCreator;
        private readonly byte[] sourceBuffer;
        private readonly int bitmapWidth;
        private readonly int bitmapHeight;
        private readonly int pixelsCount;

        public RegionGrowing2(WriteableBitmap bitmap, ColorCreator colorCreator)
        {
            this.colorCreator = colorCreator;
            bitmapWidth = bitmap.PixelWidth;
            bitmapHeight = bitmap.PixelHeight;
            pixelsCount = bitmapHeight * bitmapWidth;
            sourceBuffer = bitmap.ToByteArray();
            Conditions = new List<IRegionGrowingCondition>();
        }

        public List<IRegionGrowingCondition> Conditions { get; private set; }

        public async Task<WriteableBitmap> ProcessAsync(NeighborhoodType type)
        {
            var result = await Task.Factory.StartNew(() => Process(type));

            var resultBitmap = BitmapFactory.New(bitmapWidth, bitmapHeight);
            resultBitmap.FromByteArray(result);
            resultBitmap.RemoveAlphaChannel();
            return resultBitmap;
        }

        public byte[] Process(NeighborhoodType type)
        {
            var capacity = sourceBuffer.Length;
            var resultBuffer = new byte[capacity];
            Array.Clear(resultBuffer, 0, capacity);

            var stack = new Stack<int>();

            //Parallel.For(0, bitmapHeight, y =>
            for (int y = 0; y < bitmapHeight; y++)
            {
                for (int x = 0; x < bitmapWidth; x++)
                {
                    var currentIndex = y * bitmapWidth + x;
                    stack.Push(currentIndex);
                    while (stack.Any())
                    {
                        currentIndex = stack.Pop();

                        var realIndex = currentIndex * 4;

                        var resultColor = resultBuffer.ToInt(realIndex);

                        if (resultColor.IsFlagged() || resultColor.IsSeenFlagged())
                        {
                            continue;
                        }

                        var pixelColor = colorCreator.Create(sourceBuffer.ToInt(realIndex));
                        var hasNeighbor = stack.Count > 0 ? 1 : 0;
                        if (Conditions.Any(cond => cond.Compare(pixelColor, hasNeighbor)))
                        {
                            resultColor = ColorExtensions.Flag();
                            resultColor.ToArray(resultBuffer, realIndex);

                            stack.PushRange(AddNeighbor(currentIndex, resultBuffer, type));
                        }
                        else if (hasNeighbor > 0)
                        {
                            resultColor = ColorExtensions.AddSeenFlag();
                            resultColor.ToArray(resultBuffer, realIndex);
                        }
                    }
                }
            }
            //Debug.WriteLine($"Iterations: {iterations}");
            return resultBuffer;
        }

        private IEnumerable<int> AddNeighbor(int index, byte[] result, NeighborhoodType type)
        {
            if (index > 0)
            {
                // Add Left
                yield return index - 1;


                if (index > bitmapWidth)
                {
                    // Add Upper
                    yield return index - bitmapWidth;

                }
            }

            if (index + 1 < pixelsCount)
            {
                // Add Right
                yield return index + 1;

                if (index + bitmapWidth < pixelsCount)
                {
                    // Add Bottom
                    yield return index + bitmapWidth;
                }
            }

            //if (type == NeighborhoodType.Eight)
            //{
            //    if (index - bitmapWidth - 1 > 0)
            //    {
            //        SetIfNotFlagged(result, index - bitmapWidth - 1);
            //    }

            //    if (index - bitmapWidth + 1 > 0)
            //    {
            //        SetIfNotFlagged(result, index - bitmapWidth + 1);
            //    }

            //    if (index + bitmapWidth - 1 < pixelsCount)
            //    {
            //        SetIfNotFlagged(result, index + bitmapWidth - 1);
            //    }

            //    if (index + bitmapWidth + 1 < pixelsCount)
            //    {
            //        SetIfNotFlagged(result, index + bitmapWidth + 1);
            //    }
            //}

        }
    }
}