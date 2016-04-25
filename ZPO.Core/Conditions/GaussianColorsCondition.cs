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
        private readonly double preComputedA;
        private readonly double threshold;
        protected Matrix<double> CovarianceMatrix;
        protected Vector<double> MeanVector;
        public GaussianColorsCondition(List<IColor> compareColors, double tolerance, double neighborTolerance = 0)
            : base(compareColors, tolerance, neighborTolerance)
        {
            MeanVector = CreateVector.DenseOfArray(
                new double[]
                {
                    compareColors.Select(color => color.GetFirstPart()).Average(),
                    compareColors.Select(color => color.GetSecondPart()).Average(),
                    compareColors.Select(color => color.GetThirdPart()).Average()
                });

            var colorsMatrix = CreateMatrix.DenseOfColumnArrays(
                new double[][]
                {
                        compareColors.Select(color => color.GetFirstPart() - MeanVector[0]).ToArray(),
                        compareColors.Select(color => color.GetSecondPart() - MeanVector[1]).ToArray(),
                        compareColors.Select(color => color.GetThirdPart() - MeanVector[2]).ToArray(),
                });

            var covarianceMultiplier = compareColors.Count == 1 ? 0 : 1.0 / (compareColors.Count - 1);
            CovarianceMatrix = covarianceMultiplier * colorsMatrix.Transpose() * colorsMatrix;

            preComputedA = 1 / Math.Sqrt(CovarianceMatrix.Determinant() * Math.Pow(2 * Math.PI, 3));
            this.threshold = GaussianFunction(MeanVector) * Math.Pow(2, -tolerance);
        }

        private double min = 1;
        private double max = 0;

        public override bool Compare(IColor pixelColor, int neighborCount)
        {
            var result = GaussianFunction(
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

            var multiplier = neighborCount > 0 ? NeighborTolerance : 1;

            return multiplier * result > threshold;
        }

        private double GaussianFunction(Vector<double> value)
        {
            var centeredValues = value - MeanVector;
            var b = -1.0 / 2.0 * centeredValues.ToRowMatrix() * CovarianceMatrix.Inverse() * centeredValues;

            return preComputedA * Math.Exp(b[0]);
        }
    }
}