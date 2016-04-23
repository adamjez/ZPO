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

            var colorsMatrix = CreateMatrix.DenseOfColumnArrays(
                new double[][]
                {
                        compareColors.Select(color => color.GetFirstPart() - MeanVector[0]).ToArray(),
                        compareColors.Select(color => color.GetSecondPart() - MeanVector[1]).ToArray(),
                        compareColors.Select(color => color.GetThirdPart() - MeanVector[2]).ToArray(),
                });

            CovarianceMatrix = 1.0 / compareColors.Count * colorsMatrix.Transpose() * colorsMatrix;

            //for(int i = 0 ; i < CovarianceMatrix.RowCount; ++i)
            //    for (int j = 0; j < CovarianceMatrix.ColumnCount; ++j)
            //        CovarianceMatrix[i,j] = Math.Sqrt(CovarianceMatrix[i,j]);


            var standardDevitation = MeanVector - CovarianceMatrix.Diagonal();
            this.threshold = GaussianFunction3D(standardDevitation);
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

            var centeredValues = value - MeanVector;
            var b = -1.0 / 2.0 * centeredValues.ToRowMatrix() * CovarianceMatrix.Inverse() * centeredValues;

            return a * Math.Exp(b[0]);
        }
    }
}