using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using ZPO.Core.Colors;

namespace ZPO.Core.Conditions
{
    public class GaussianColorsCondition : ColorsCondition
    {
        private readonly double preComputedA;
        private readonly double threshold;
        private readonly double neighborThreshold;
        protected Matrix<double> CovarianceMatrix;
        protected Vector<double> MeanVector;
        public GaussianColorsCondition(List<IColor> compareColors, double tolerance, double neighborTolerance = 0)
            : base(compareColors, tolerance, neighborTolerance)
        {
            if (!compareColors.Any())
            {
                throw new ArgumentException("compareColors have to have at least 1 color",nameof(compareColors));
            }

            var partsCount = CompareColors.First().GetParts().Count;
            MeanVector = CreateVector.Dense<double>(partsCount);
            for (var i = 0; i < partsCount; ++i)
            {
                MeanVector[i] = compareColors.Average(color => color.GetParts()[i]);
            }

            var colorsMatrix = CreateMatrix.Dense<double>(compareColors.Count, partsCount);
            for (var i = 0; i < partsCount; ++i)
            {
                colorsMatrix.SetColumn(i, CreateVector.DenseOfArray(
                    compareColors.Select(color => color.GetParts()[i] - MeanVector[i]).ToArray()));
            }

            var covarianceMultiplier = compareColors.Count == 1 ? 0 : 1.0 / (compareColors.Count - 1);
            CovarianceMatrix = covarianceMultiplier * colorsMatrix.Transpose() * colorsMatrix;

            preComputedA = 1 / Math.Sqrt(CovarianceMatrix.Determinant() * Math.Pow(2 * Math.PI, 3));

            

            this.threshold = compareColors.Min(color => GaussianFunction(color.GetParts())) * Math.Pow(2, -tolerance);
            this.neighborThreshold = threshold * Math.Pow(2, -neighborTolerance);
        }


        public override bool Compare(IColor pixelColor, int neighborCount, double rowRatio = -1)
        {
            var result = GaussianFunction(pixelColor.GetParts());

            var currentThreshold = neighborCount > 0 ? neighborThreshold : threshold;

            return result >= currentThreshold;
        }

        private double GaussianFunction(Vector<double> value)
        {
            var centeredValues = value - MeanVector;
            var b = -1.0 / 2.0 * centeredValues.ToRowMatrix() * CovarianceMatrix.Inverse() * centeredValues;

            return preComputedA * Math.Exp(b[0]);
        }
    }
}