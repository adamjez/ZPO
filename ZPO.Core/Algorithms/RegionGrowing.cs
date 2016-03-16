using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media.Imaging;
using ZPO.Core.Colors;
using Buffer = System.Buffer;

namespace ZPO.Core.Algorithms
{
    public class RegionGrowing
    {
        private readonly ColorCreator _colorCreator;
        private readonly byte[] sourceBuffer;
        private readonly int bitmapWidth;
        private readonly int bitmapHeight;
        private readonly int pixelsCount;

        public RegionGrowing(WriteableBitmap bitmap, ColorCreator colorCreator)
        {
            _colorCreator = colorCreator;
            bitmapWidth = bitmap.PixelWidth;
            bitmapHeight = bitmap.PixelHeight;
            pixelsCount = bitmapHeight * bitmapWidth;
            sourceBuffer = bitmap.ToByteArray();
            Conditions = new List<IRegionGrowingCondition>();
        }

        public List<IRegionGrowingCondition> Conditions { get; private set; }

        public async Task<byte[]> ProcessAsync(NeighborhoodType type)
        {
            return await Task.Factory.StartNew(() => Process(type));
        }

        public byte[] Process(NeighborhoodType type)
        {
            var capacity = sourceBuffer.Length;
            var resultBuffer = new byte[capacity];
            Array.Clear(resultBuffer, 0, capacity);

            var change = true;
            //var iterations = 0;
            while (change)
            {
                //iterations++;
                //if (iterations == 10)
                //    break;
                change = false;
                var currentIndex = 0;
                while (currentIndex < pixelsCount)
                {
                    var realIndex = currentIndex * 4;

                    ++currentIndex;

                    var resultColor = resultBuffer.ToInt(realIndex);

                    if (resultColor.IsFlagged())
                    {
                        continue;
                    }

                    var pixelColor = _colorCreator.Create(sourceBuffer.ToInt(realIndex));
                    if (Conditions.Any(cond => cond.Compare(pixelColor, resultColor.GetNeighborMultiplier())))
                    {
                        change = true;

                        resultColor = ColorExtensions.Flag();
                        resultColor.ToArray(resultBuffer, realIndex);

                        AddNeighbor(currentIndex, resultBuffer, type);
                    }
                }
            }

            return resultBuffer;
        }

        private void AddNeighbor(int index, byte[] result, NeighborhoodType type)
        {
            if (index > 0)
            {
                // Add Left
                SetIfNotFlagged(result, index - 1);

                if (index > bitmapWidth)
                {
                    // Add Upper
                    SetIfNotFlagged(result, index - bitmapWidth);
                }
            }

            if (index + 1 < pixelsCount)
            {
                // Add Right
                SetIfNotFlagged(result, index + 1);

                if (index + bitmapWidth < pixelsCount)
                {
                    // Add Bottom
                    SetIfNotFlagged(result, index + bitmapWidth);
                }
            }

            if (type == NeighborhoodType.Eight)
            {
                if (index - bitmapWidth - 1 > 0)
                {
                    SetIfNotFlagged(result, index - bitmapWidth - 1);
                }

                if (index - bitmapWidth + 1 > 0)
                {
                    SetIfNotFlagged(result, index - bitmapWidth + 1);
                }

                if (index + bitmapWidth - 1 < pixelsCount)
                {
                    SetIfNotFlagged(result, index + bitmapWidth - 1);
                }

                if (index + bitmapWidth + 1 < pixelsCount)
                {
                    SetIfNotFlagged(result, index + bitmapWidth + 1);
                }
            }

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetIfNotFlagged(byte[] result, int index)
        {
            var realIndex = index * 4;
            var color = result.ToInt(realIndex);
            if (!color.IsFlagged())
            {
                color = color.AddNeighborFlag();
                color.ToArray(result, realIndex);
            }
        }
    }
}
