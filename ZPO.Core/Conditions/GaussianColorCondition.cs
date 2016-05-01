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
        private double mean;
        private double deviation;
        private double threshold;
        private double neighborThreshold;

        public GaussianColorCondition(IColor compareColor, double tolerance, double neighborTolerance, 
            WriteableBitmap image, ColorCreator colorCreator)
            : base(compareColor, tolerance, neighborTolerance)
        {
            MakeHistogramFromGrayScale(image, colorCreator);
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

                        histogram[(int)parts[0]] ++;
                    }
                }
            }

            // Find local maximum and local minimum from compare color position
            var part = CompareColor.GetParts()[0];

            var maxim = FindPeaks(histogram.ToList(), 20, PeakType.Max);
            var maximum = maxim.FindClosest((int)part);

            var minimums = FindPeaks(histogram.ToList(), 20, PeakType.Min);
            var minimum = minimums.FindClosest((int)part);


            mean = maximum;
            deviation = Math.Abs(maximum - minimum) / 3.0;
            threshold = GaussianFunction(minimum) * Math.Pow(2, -Tolerance);
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
            var part = CompareColor.GetParts()[0];

            var maxim = FindPeaks(histogram.Column(0).ToList(), 20, PeakType.Max);
            var maximum = maxim.FindClosest((int)part);

            var minimums = FindPeaks(histogram.Column(0).ToList(), 20, PeakType.Min);
            var minimum = minimums.FindClosest((int)part);


            mean = maximum;
            deviation = Math.Abs(maximum - minimum) / 3.0;
            threshold = GaussianFunction(minimum) * Math.Pow(2, -Tolerance);
            neighborThreshold = threshold * Math.Pow(2, -NeighborTolerance);
        }


        public static IList<int> FindPeaks(IList<double> values, int rangeOfPeaks, PeakType type)
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
                var item = type == PeakType.Max ? range.Max() : range.Min();
                if ((range.Count() > 0) && (current == item))
                {
                    peaks.Add(i);
                }
            }

            return peaks;
        }

        public enum PeakType
        {
            Max, Min
        }

        public static List<int> PeakinessTest(double[] histogram, double peakinessThres)
        {
            int j = 0;
            List<int> valleys = new List<int>();

            //The start of the valley
            var vA = histogram[j];
            var P = vA;

            //The end of the valley
            var vB = 0.0;

            //The width of the valley, default width is 1
            int W = 1;

            //The sum of the pixels between vA and vB
            var N = 0.0;

            //The measure of the peaks peakiness
            double peakiness = 0.0;

            int peak = 0;
            bool l = false;

            try
            {
                while (j < 254)
                {

                    l = false;
                    vA = histogram[j];
                    P = vA;
                    W = 1;
                    N = vA;

                    int i = j + 1;

                    //To find the peak
                    while (P < histogram[i])
                    {
                        P = histogram[i];
                        W++;
                        N += histogram[i];
                        i++;
                    }


                    //To find the border of the valley other side
                    peak = i - 1;
                    vB = histogram[i];
                    N += histogram[i];
                    i++;
                    W++;

                    l = true;
                    while (vB >= histogram[i])
                    {
                        vB = histogram[i];
                        W++;
                        N += histogram[i];
                        i++;
                    }

                    //Calculate peakiness
                    peakiness = (1 - (double)((vA + vB) / (2.0 * P))) * (1 - ((double)N / (double)(W * P)));

                    if (peakiness > peakinessThres & !valleys.Contains(j))
                    {
                        //peaks.Add(peak);                        
                        valleys.Add(j);
                        valleys.Add(i - 1);
                    }

                    j = i - 1;
                }
            }
            catch (Exception)
            {
                if (l)
                {
                    vB = histogram[255];

                    peakiness = (1 - (double)((vA + vB) / (2.0 * P))) * (1 - ((double)N / (double)(W * P)));

                    if (peakiness > peakinessThres)
                        valleys.Add(255);

                    //peaks.Add(255);
                    return valleys;
                }
            }

            //if(!valleys.Contains(255))
            //    valleys.Add(255);

            return valleys;
        }


        public override bool Compare(IColor pixelColor, int neighborCount, double row = -1)
        {
            var result = GaussianFunction(pixelColor.GetParts()[0]);

            var currentThreshold = neighborCount > 0 ? neighborThreshold : threshold;


            return result >= currentThreshold;
        }

        private double GaussianFunction(double value)
        {
            var result = (1 / (deviation * Math.Sqrt(2 * Math.PI))) * Math.Exp(-Math.Pow(value - mean, 2) / (2 * Math.Pow(deviation, 2)));


            return result;
        }

        public static int[] Cluster(Matrix<double> rawData, int numClusters)
        {
            //double[][] data = Normalized(rawData);
            var data = rawData;
            bool changed = true; bool success = true;
            int[] clustering = InitClustering(data.RowCount, numClusters, 0);
            double[][] means = Allocate(numClusters, data.ColumnCount);
            int maxCount = data.RowCount * 10;
            int ct = 0;
            while (changed == true && success == true && ct < maxCount)
            {
                ++ct;
                success = UpdateMeans(data, clustering, means);
                changed = UpdateClustering(data, clustering, means);
            }
            return clustering;
        }

        private static double Distance(Vector<double> tuple, double[] mean)
        {
            double sumSquaredDiffs = 0.0;
            for (int j = 0; j < tuple.Count; ++j)
                sumSquaredDiffs += Math.Pow((tuple[j] - mean[j]), 2);
            return Math.Sqrt(sumSquaredDiffs);
        }

        private static int MinIndex(double[] distances)
        {
            int indexOfMin = 0;
            double smallDist = distances[0];
            for (int k = 0; k < distances.Length; ++k)
            {
                if (distances[k] < smallDist)
                {
                    smallDist = distances[k];
                    indexOfMin = k;
                }
            }
            return indexOfMin;
        }

        private static bool UpdateClustering(Matrix<double> data, int[] clustering, double[][] means)
        {
            int numClusters = means.Length;
            bool changed = false;

            int[] newClustering = new int[clustering.Length];
            Array.Copy(clustering, newClustering, clustering.Length);

            double[] distances = new double[numClusters];

            for (int i = 0; i < data.RowCount; ++i)
            {
                for (int k = 0; k < numClusters; ++k)
                    distances[k] = Distance(data.Row(i), means[k]);

                int newClusterID = MinIndex(distances);
                if (newClusterID != newClustering[i])
                {
                    changed = true;
                    newClustering[i] = newClusterID;
                }
            }

            if (changed == false)
                return false;

            int[] clusterCounts = new int[numClusters];
            for (int i = 0; i < data.RowCount; ++i)
            {
                int cluster = newClustering[i];
                ++clusterCounts[cluster];
            }

            for (int k = 0; k < numClusters; ++k)
                if (clusterCounts[k] == 0)
                    return false;

            Array.Copy(newClustering, clustering, newClustering.Length);
            return true; // no zero-counts and at least one change
        }

        private static bool UpdateMeans(Matrix<double> data, int[] clustering, double[][] means)
        {
            int numClusters = means.Length;
            int[] clusterCounts = new int[numClusters];
            for (int i = 0; i < data.RowCount; ++i)
            {
                int cluster = clustering[i];
                ++clusterCounts[cluster];
            }

            for (int k = 0; k < numClusters; ++k)
                if (clusterCounts[k] == 0)
                    return false;

            for (int k = 0; k < means.Length; ++k)
                for (int j = 0; j < means[k].Length; ++j)
                    means[k][j] = 0.0;

            for (int i = 0; i < data.RowCount; ++i)
            {
                int cluster = clustering[i];
                for (int j = 0; j < data.ColumnCount; ++j)
                    means[cluster][j] += data[i, j]; // accumulate sum
            }

            for (int k = 0; k < means.Length; ++k)
                for (int j = 0; j < means[k].Length; ++j)
                    means[k][j] /= clusterCounts[k]; // danger of div by 0
            return true;
        }

        private static double[][] Allocate(int numClusters, int numColumns)
        {
            double[][] result = new double[numClusters][];
            for (int k = 0; k < numClusters; ++k)
                result[k] = new double[numColumns];
            return result;
        }

        private static int[] InitClustering(int numTuples, int numClusters, int seed)
        {
            Random random = new Random(seed);
            int[] clustering = new int[numTuples];
            for (int i = 0; i < numClusters; ++i)
                clustering[i] = i;
            for (int i = numClusters; i < clustering.Length; ++i)
                clustering[i] = random.Next(0, numClusters);
            return clustering;
        }

        private static double[][] Normalized(double[][] rawData)
        {
            double[][] result = new double[rawData.Length][];
            for (int i = 0; i < rawData.Length; ++i)
            {
                result[i] = new double[rawData[i].Length];
                Array.Copy(rawData[i], result[i], rawData[i].Length);
            }

            for (int j = 0; j < result[0].Length; ++j)
            {
                double colSum = 0.0;
                for (int i = 0; i < result.Length; ++i)
                    colSum += result[i][j];
                double mean = colSum / result.Length;
                double sum = 0.0;
                for (int i = 0; i < result.Length; ++i)
                    sum += (result[i][j] - mean) * (result[i][j] - mean);
                double sd = sum / result.Length;
                for (int i = 0; i < result.Length; ++i)
                    result[i][j] = (result[i][j] - mean) / sd;
            }
            return result;
        }
    }
}