using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media.Imaging;
using MathNet.Numerics.LinearAlgebra;
using ZPO.Core.Colors;

namespace ZPO.Core.Conditions
{
    /// <summary>
    /// Method that is futher described in documentation
    /// This method only support grayscale and RGB color space
    /// In constructor this actions are made:
    ///     1. Histogram of analyzed image is created
    ///     2. Histogram is smoothed by sliding window
    ///     3. Maximum and minimum peaks are found in histogram
    ///     4. Peaks which are connected with given compare color are selected
    ///     5. From peaks are derived parameters of normal distribution
    /// </summary>
    public class GaussianColorCondition : ColorCondition
    {
        private double threshold;
        private double neighborThreshold;
        private double baseThreshold;
        private readonly int partsCount;

        private Vector<double> meanVector;
        private Vector<double> deviationVector;

        public GaussianColorCondition(IColor compareColor, double tolerance, double neighborTolerance,
            WriteableBitmap image, ColorCreator colorCreator, bool dynamicThreshold = false)
            : base(compareColor, tolerance, neighborTolerance, dynamicThreshold)
        {
            partsCount = compareColor.GetParts().Count;

            if (compareColor is RGBColor || compareColor is GrayScaleColor)
            {
                MakeHistogram(image, colorCreator);
            }
            else
            {
                throw new CreateModelException("Color Space isn't supported");
            }
        }
        
        private void MakeHistogram(WriteableBitmap image, ColorCreator colorCreator)
        {
            var histogram = CreateMatrix.Dense<double>(256, partsCount);
            using (var srcContext = image.GetBitmapContext(ReadWriteMode.ReadOnly))
            {
                for (int y = 0; y < image.PixelHeight; y++)
                {
                    for (int x = 0; x < image.PixelWidth; x++)
                    {
                        var value = srcContext.Pixels[y * image.PixelWidth + x];
                        var parts = colorCreator.Create(value).GetParts();

                        for (int index = 0; index < parts.Count; index++)
                        {
                            histogram[(int)parts[index], index]++;
                        }
                    }
                }
            }

            meanVector = CreateVector.Dense<double>(partsCount);
            deviationVector = CreateVector.Dense<double>(partsCount);
            // Find local maximum and local minimum from compare color position
            for (int index = 0; index < partsCount; index++)
            {
                var result = GetMeanAndDeviation(histogram.Column(index), 
                    (int)Color.GetParts()[index]);

                meanVector[index] = result.Item1;
                deviationVector[index] = result.Item2;
            }

            baseThreshold = GaussianFunction(meanVector - deviationVector);
            threshold = baseThreshold * Math.Pow(2, -Tolerance);
            neighborThreshold = threshold * Math.Pow(2, -NeighborTolerance);
        }

        public static Tuple<double, double> GetMeanAndDeviation(Vector<double> histogram, int colorValue)
        {
            var histogramList = SmoothHistogram(histogram.ToList(), 6);
            var minimums = FindPeaks(histogramList, 16, PeakType.Min);

            //var minimum = minimums.FindClosest(colorValue);

            var minimum1 = minimums.FindClosestToLeft(colorValue);
            var minimum2 = minimums.FindClosestToRight(colorValue, 255);

            var maxim = FindPeaks(histogramList, 16, PeakType.Max);
            var maximum = maxim.FindClosestBetween(minimum1, minimum2);

            if (maximum == null)
            {
                throw new CreateModelException("Couldn't find maxim and/or minim");
            }
            var max = (int)maximum;

            var deviation = ((Math.Abs(max - minimum1) + Math.Abs(max - minimum2)) / 2.0) / 3.0;

            return new Tuple<double, double>(max, deviation);
        }


        public static List<int> FindPeaks(IList<double> values, int rangeOfPeaks, PeakType type)
        {
            List<int> peaks = new List<int>();

            int checksOnEachSide = rangeOfPeaks / 2;
            for (int i = 0; i < values.Count; i++)
            {
                var current = values[i];
                IEnumerable<double> range = values;

                if (i > checksOnEachSide)
                {
                    range = range.Skip(i - checksOnEachSide);
                }

                range = range.Take(rangeOfPeaks);
                var list = range as IList<double> ?? range.ToList();
                var item = type == PeakType.Max ? list.Max() : list.Min();
                if (list.Any() && ((int)current == (int)item))
                {
                    peaks.Add(i);
                }
            }

            return peaks;
        }

        public static List<double> SmoothHistogram(List<double> histogram, int windowSize)
        {
            int winMidSize = windowSize / 2;

            for (int i = winMidSize; i < histogram.Count - windowSize; ++i)
            {
                double mean = 0;
                for (int j = i - winMidSize; j <= (i + winMidSize); ++j)
                {
                    mean += histogram[j];
                }

                histogram[i] = mean / windowSize;
            }

            return histogram;
        }

        public enum PeakType
        {
            Max,
            Min
        }

        public override bool Compare(IColor pixelColor, int neighborCount, double rowRatio = -1)
        {
            var result = GaussianFunction(pixelColor.GetParts());

            var currentThreshold = 0.0;
            if (DynamicThreshold)
            {
                currentThreshold = baseThreshold * Math.Pow(2, -GetMultiplier(rowRatio));
                if (neighborCount > 0)
                {
                    currentThreshold = currentThreshold * Math.Pow(2, -NeighborTolerance);
                }
            }
            else
            {
                currentThreshold = neighborCount > 0 ? neighborThreshold : threshold;
            }

            return result >= currentThreshold;
        }

        private double GaussianFunction(Vector<double> value)
        {
            var result = 1.0;
            for (int index = 0; index < value.Count; index++)
            {
                result *= GaussianDistribution(value[index], 
                    meanVector[index], deviationVector[index]);
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double GaussianDistribution(double value, double mean, double deviation)
        {
            return (1 / (deviation * Math.Sqrt(2 * Math.PI))) *
                Math.Exp(-Math.Pow(value - mean, 2) / (2 * Math.Pow(deviation, 2)));
        }
    }
}