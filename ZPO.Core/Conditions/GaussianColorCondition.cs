using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Media.Imaging;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using ZPO.Core.Colors;

namespace ZPO.Core.Conditions
{
    public class GaussianColorCondition : ColorCondition
    {
        private double threshold;
        private double neighborThreshold;
        private readonly int partsCount;

        // 1D normal distribution
        private double mean;
        private double deviation;

        // 3D Multivariate normal distribution
        private Vector<double> meanVector;
        private Matrix<double> covarianceMatrix;
        private double preComputedA;



        public GaussianColorCondition(IColor compareColor, double tolerance, double neighborTolerance,
            WriteableBitmap image, ColorCreator colorCreator)
            : base(compareColor, tolerance, neighborTolerance)
        {
            partsCount = compareColor.GetParts().Count;

            if (partsCount == 1)
            {
                MakeHistogramFromGrayScale(image, colorCreator);
            }
            else if (partsCount == 3)
            {
                MakeHistogram(image, colorCreator);
            }
            else
            {
                throw new CreateModelException("Color Space isn't supported");
            }
        }

        private void MakeHistogramFromGrayScale(WriteableBitmap image, ColorCreator colorCreator)
        {
            var histogram = CreateVector.Dense<double>(256);
            using (var srcContext = image.GetBitmapContext(ReadWriteMode.ReadOnly))
            {
                for (int y = 0; y < image.PixelHeight; y++)
                {
                    for (int x = 0; x < image.PixelWidth; x++)
                    {
                        var value = srcContext.Pixels[y * image.PixelWidth + x];
                        var parts = colorCreator.Create(value).GetParts();

                        histogram[(int)parts[0]]++;
                    }
                }
            }

            // Find local maximum and local minimum from compare color position
            var result = GetMeanAndDeviation(histogram, (int)CompareColor.GetParts()[0]);

            mean = result.Item1;
            deviation = result.Item2;
            threshold = GaussianFunction1D(result.Item2) * Math.Pow(2, -Tolerance);
            neighborThreshold = threshold * Math.Pow(2, -NeighborTolerance);
        }

        private void MakeHistogram(WriteableBitmap image, ColorCreator colorCreator)
        {
            var histogram = CreateMatrix.Dense<double>(256, 3);
            using (var srcContext = image.GetBitmapContext(ReadWriteMode.ReadOnly))
            {
                for (int y = 0; y < image.PixelHeight; y++)
                {
                    for (int x = 0; x < image.PixelWidth; x++)
                    {
                        var value = srcContext.Pixels[y * image.PixelWidth + x];
                        var parts = colorCreator.Create(value).GetParts();

                        histogram[(int)parts[0], 0]++;
                        histogram[(int)parts[1], 1]++;
                        histogram[(int)parts[2], 2]++;
                    }
                }
            }

            // Find local maximum and local minimum from compare color position
            var result1 = GetMeanAndDeviation(histogram.Column(0), (int)CompareColor.GetParts()[0]);
            var result2 = GetMeanAndDeviation(histogram.Column(1), (int)CompareColor.GetParts()[1]);
            var result3 = GetMeanAndDeviation(histogram.Column(2), (int)CompareColor.GetParts()[2]);

            meanVector = CreateVector.DenseOfArray(new[] { result1.Item1, result2.Item1, result3.Item1 });
            covarianceMatrix = CreateMatrix.DenseOfDiagonalArray(
                new[]
                {
                    Math.Pow(result1.Item2, 2),
                    Math.Pow(result2.Item2, 2),
                    Math.Pow(result3.Item2, 2)
                });


            preComputedA = 1 / Math.Sqrt(covarianceMatrix.Determinant() * Math.Pow(2 * Math.PI, 3));

            var minimumVector = CreateVector.DenseOfArray(new[]
            {
                result1.Item2, result2.Item2, result3.Item2
            });

            threshold = GaussianFunction(minimumVector) * Math.Pow(2, -Tolerance);
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


        public override bool Compare(IColor pixelColor, int neighborCount, double row = -1)
        {
            var result = 0.0;
            if (partsCount == 1)
            {
                result = GaussianFunction1D(pixelColor.GetParts()[0]);
            }
            else
            {
                result = GaussianFunction(pixelColor.GetParts());
            }

            var currentThreshold = neighborCount > 0 ? neighborThreshold : threshold;

            return result >= currentThreshold;
        }

        private double GaussianFunction1D(double value)
        {
            var result = (1 / (deviation * Math.Sqrt(2 * Math.PI))) *
                Math.Exp(-Math.Pow(value - mean, 2) / (2 * Math.Pow(deviation, 2)));

            return result;
        }

        private double GaussianFunction(Vector<double> value)
        {
            var centeredValues = value - meanVector;
            var b = -1.0 / 2.0 * centeredValues.ToRowMatrix() * covarianceMatrix.Inverse() * centeredValues;

            return preComputedA * Math.Exp(b[0]);
        }

    }
}