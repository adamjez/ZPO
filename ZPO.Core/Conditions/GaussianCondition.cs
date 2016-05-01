using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using ZPO.Core.Colors;

namespace ZPO.Core.Conditions
{
    /// <summary>
    /// From given compare colors tries to create normal distribution
    /// First its create vector of means values and covariance matrix
    /// It support all color spaces (variable number of parts - grayscale 1 part
    /// rgb 3 parts cielab 2 parts). Base threshold is created from minimal value
    /// of normal distribution for given colors. 
    /// </summary>
    public class GaussianCondition : ArithmeticalDistanceCondition
    {
        private readonly double preComputedA;
        private readonly double threshold;
        private readonly double neighborThreshold;
        private readonly double baseThreshold;

        protected Matrix<double> CovarianceMatrix;
        protected Vector<double> MeanVector;
        public GaussianCondition(List<IColor> compareColors, double tolerance, double neighborTolerance = 0, bool dynamicThreshold = false)
            : base(compareColors, tolerance, neighborTolerance, dynamicThreshold)
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


            this.baseThreshold = compareColors.Min(color => GaussianFunction(color.GetParts()));

            if (double.IsNaN(baseThreshold))
            {
                throw new CreateModelException("Couldn't create normal distribution from these colors");
            }

            this.threshold = baseThreshold * Math.Pow(2, -tolerance);
            this.neighborThreshold = threshold * Math.Pow(2, -neighborTolerance);
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
            var centeredValues = value - MeanVector;
            var b = -1.0 / 2.0 * centeredValues.ToRowMatrix() * CovarianceMatrix.Inverse() * centeredValues;

            return preComputedA * Math.Exp(b[0]);
        }
    }
}