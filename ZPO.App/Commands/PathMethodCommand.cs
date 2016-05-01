using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using ZPO.App.Enums;
using ZPO.App.ViewModels;
using ZPO.Core;
using ZPO.Core.Algorithms;
using ZPO.Core.Colors;
using ZPO.Core.Conditions;

namespace ZPO.App.Commands
{
    public class PathMethodCommand : ImageProcessCommand
    {
        public PathMethodCommand(MainViewModel viewModel)
            : base(viewModel)
        {
            ViewModel.CurrentColors.CollectionChanged +=
                (sender, args) => OnExecuteChanged();
        }

        public override bool CanExecute(object parameter = null)
        {
            // Check if we picked some color (Alpha channel equals 0 when no color
            // is picked yet)
            if (!ViewModel.CurrentColors.Any())
            {
                return false;
            }
            return base.CanExecute(parameter);
        }

        public override async void Execute(object parameter)
        {
            if (CanExecute())
            {
                Stopwatch sw = new Stopwatch();

                sw.Start();

                ViewModel.Processing = true;

                var sucess = true;
                IThresholdingMethod process = new ThresholdMethodOptimized(ViewModel.SourceImage,
                    new ColorCreator(ViewModel.SelectedColorSpace));

                if (ViewModel.ThresholdMethod == PathMethods.Base)
                {
                    process = new ThresholdingMethod(ViewModel.SourceImage,
                        new ColorCreator(ViewModel.SelectedColorSpace));
                }

                var colors = ViewModel.CurrentColors
                    .Select(color => color.ConvertTo(ViewModel.SelectedColorSpace))
                    .ToList();

                IColorCondition condition = null;
                if (ViewModel.SelectedCondition == ConditionType.NormalDistributionFromColors)
                {
                    condition = new GaussianColorsCondition(colors, ViewModel.Tolerance,
                        ViewModel.NeighborTolerance);
                }
                else if (ViewModel.SelectedCondition == ConditionType.ArithmeticalDistance)
                {
                    condition = new ColorsCondition(colors, ViewModel.Tolerance,
                        ViewModel.NeighborTolerance, ViewModel.DynamicThreshold);
                }
                else if (ViewModel.SelectedCondition == ConditionType.NormalDistributionFromColor)
                {
                    try
                    {
                        condition = new GaussianColorCondition(colors.First(), ViewModel.Tolerance,
                            ViewModel.NeighborTolerance, ViewModel.SourceImage,
                            new ColorCreator(ViewModel.SelectedColorSpace));
                    }
                    catch (CreateModelException exc)
                    {
                        ViewModel.ResultMessage = "Couldn't create model: " + exc.Message;
                        sucess = false;
                    }

                }

                if (sucess)
                {
                    process.Conditions.Add(condition);

                    var result = await process.ProcessAsync(NeighborhoodType.Four);

                    ViewModel.SetNewImage(result);

                    ViewModel.ResultMessage = string.Empty;
                }

                ViewModel.Processing = false;

                sw.Stop();

                Debug.WriteLine("Elapsed={0}", sw.Elapsed);
            }
        }
    }
}