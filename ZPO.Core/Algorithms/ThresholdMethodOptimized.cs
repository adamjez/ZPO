using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using ZPO.Core.Colors;
using ZPO.Core.Conditions;

namespace ZPO.Core.Algorithms
{
    /// <summary>
    /// Optimized version of ThresholdMethod
    /// Iterates through image, if some pixel meets conditions, than is flagged.
    /// Neighbor pixels are added to priority list and are checked right after it.
    /// </summary>
    public class ThresholdMethodOptimized : IThresholdingMethod
    {
        private readonly ColorCreator colorCreator;
        private readonly byte[] sourceBuffer;
        private readonly int bitmapWidth;
        private readonly int bitmapHeight;
        private readonly int pixelsCount;

        public ThresholdMethodOptimized(WriteableBitmap bitmap, ColorCreator colorCreator)
        {
            this.colorCreator = colorCreator;
            bitmapWidth = bitmap.PixelWidth;
            bitmapHeight = bitmap.PixelHeight;
            pixelsCount = bitmapHeight * bitmapWidth;
            sourceBuffer = bitmap.ToByteArray();
            Conditions = new List<IColorCondition>();
        }

        public List<IColorCondition> Conditions { get; private set; }

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
                        if (Conditions.Any(cond => cond.Compare(pixelColor, hasNeighbor, y / (double)bitmapHeight)))
                        {
                            resultColor = ColorExtensions.Flag();
                            resultColor.ToArray(resultBuffer, realIndex);

                            if (type != NeighborhoodType.None)
                            {
                                stack.PushRange(AddNeighbor(currentIndex, type));
                            }
                        }
                        else if (hasNeighbor > 0)
                        {
                            resultColor = ColorExtensions.AddSeenFlag();
                            resultColor.ToArray(resultBuffer, realIndex);
                        }
                    }
                }
            }

            return resultBuffer;
        }

        private IEnumerable<int> AddNeighbor(int index, NeighborhoodType type)
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

            if (type == NeighborhoodType.Eight)
            {
                if (index - bitmapWidth - 1 >= 0)
                {
                    yield return index - bitmapWidth - 1;
                }

                if (index - bitmapWidth + 1 >= 0)
                {
                    yield return index - bitmapWidth + 1;
                }

                if (index + bitmapWidth - 1 < pixelsCount)
                {
                    yield return index + bitmapWidth - 1;
                }

                if (index + bitmapWidth + 1 < pixelsCount)
                {
                    yield return index + bitmapWidth + 1;
                }
            }

        }
    }
}