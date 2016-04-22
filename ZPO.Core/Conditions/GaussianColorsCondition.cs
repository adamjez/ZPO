using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Automation;
using MathNet.Numerics.LinearAlgebra;
using ZPO.Core.Colors;

namespace ZPO.Core.Conditions
{
    public class GaussianColorsCondition : ColorsCondition
    {
        private double threshold = 0.005;
        protected Matrix<double> CovarianceMatrix;
        protected Vector<double> MeanVector;
        public GaussianColorsCondition(List<IColor> compareColors, uint tolerance, uint toleranceMultiplier = 1)
            : base(compareColors, tolerance, toleranceMultiplier)
        {
            MeanVector = CreateVector.DenseOfArray(
                new double[]
                {
                    compareColors.Select(color => color.GetFirstPart()).Average(),
                    compareColors.Select(color => color.GetSecondPart()).Average(),
                    compareColors.Select(color => color.GetThirdPart()).Average()
                });

            var firstPartList = compareColors.Select(color => color.GetFirstPart()).ToList();
            var secondPartList = compareColors.Select(color => color.GetSecondPart()).ToList();
            var thirdPartList = compareColors.Select(color => color.GetThirdPart()).ToList();

            CovarianceMatrix = CreateMatrix.DenseOfArray(new double[,]
            {
                {
                    ComputeDeviation(firstPartList, MeanVector[0], firstPartList, MeanVector[0]),
                    ComputeDeviation(firstPartList, MeanVector[0], secondPartList, MeanVector[1]),
                    ComputeDeviation(firstPartList, MeanVector[0], thirdPartList, MeanVector[2])
                },
                {
                    ComputeDeviation(secondPartList, MeanVector[1], firstPartList, MeanVector[0]),
                    ComputeDeviation(secondPartList, MeanVector[1], secondPartList, MeanVector[1]),
                    ComputeDeviation(secondPartList, MeanVector[1], thirdPartList, MeanVector[2])
                },
                {
                    ComputeDeviation(thirdPartList, MeanVector[2], firstPartList, MeanVector[0]),
                    ComputeDeviation(thirdPartList, MeanVector[2], secondPartList, MeanVector[1]),
                    ComputeDeviation(thirdPartList, MeanVector[2], thirdPartList, MeanVector[2])
                }
            });
        }

        private double min = 1;
        private double max = 0;

        public override bool Compare(IColor pixelColor, int neighborCount)
        {
            var result = GaussianFunction3D(
                CreateVector.DenseOfArray(
                    new double[]
                    {
                        pixelColor.GetFirstPart(),
                        pixelColor.GetSecondPart(),
                        pixelColor.GetThirdPart()
                    }));

            if (result > max)
            {
                Debug.WriteLine($"new max is {result}");
            }
            if (result < min)
            {
                Debug.WriteLine($"new min is {result}");
            }
            max = Math.Max(result, max);
            min = Math.Min(result, min);

            return result > threshold;
        }

        private double GaussianFunction3D(Vector<double> value)
        {
            var a = 1 / Math.Sqrt(CovarianceMatrix.Determinant() * Math.Pow(2 * Math.PI, 3));
            var b = -1.0 / 2.0 * (value - MeanVector).ToRowMatrix() * CovarianceMatrix.Inverse() * (value - MeanVector);

            return a * Math.Exp(b[0]);
        }

        private double ComputeDeviation(List<int> values1, double mean1, List<int> values2, double mean2)
        {
            if (values1.Count() != values2.Count())
                throw new ArgumentException("Arguments have to have same dimensions");

            double accumulator = 0;
            var size = values1.Count;
            for (int i = 0; i < size; i++)
            {
                accumulator += (values1[i] - mean1) * (values2[i] - mean2);
            }

            return accumulator / size;
        }
    }
}