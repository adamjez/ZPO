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
    /// <summary>
    /// Base thresholding method which iterates through whole image and checks
    /// if any condition succeeds. If yes, that pixel is flagged and neighboring pixels
    /// are flagged as Neighbor so when is pixel selected toleration is increased
    /// If any changes in flagging pixels happens, the iterations over image repeats
    /// </summary>
    public class ThresholdingMethod : IThresholdingMethod
    {
        private readonly ColorCreator colorCreator;
        private readonly byte[] sourceBuffer;
        private readonly int bitmapWidth;
        private readonly int bitmapHeight;
        private readonly int pixelsCount;

        public ThresholdingMethod(WriteableBitmap bitmap, ColorCreator colorCreator)
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

            var change = true;
            var iterations = 0;
            while (change)
            {
                iterations++;

                change = false;

                //Parallel.For(0, bitmapHeight, y =>
                for (int y = 0; y < bitmapHeight; y++)
                {
                    for (int x = 0; x < bitmapWidth; x++)
                    {
                        var currentIndex = y * bitmapWidth + x;
                        var realIndex = currentIndex * 4;

                        var resultColor = resultBuffer.ToInt(realIndex);

                        if (resultColor.IsFlagged())
                        {
                            continue;
                        }

                        var pixelColor = colorCreator.Create(sourceBuffer.ToInt(realIndex));
                        if (Conditions.Any(cond => cond.Compare(pixelColor, resultColor.GetNeighborMultiplier())))
                        {
                            change = true;

                            resultColor = ColorExtensions.Flag();
                            resultColor.ToArray(resultBuffer, realIndex);

                            if (type != NeighborhoodType.None)
                            {
                                AddNeighbor(currentIndex, resultBuffer, type);
                            }
                        }
                    }
                }
            }
            Debug.WriteLine($"Iterations: {iterations}");
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
