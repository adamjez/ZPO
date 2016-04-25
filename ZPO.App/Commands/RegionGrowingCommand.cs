using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using ZPO.App.ViewModels;
using ZPO.Core;
using ZPO.Core.Algorithms;
using ZPO.Core.Colors;
using ZPO.Core.Conditions;

namespace ZPO.App.Commands
{
    public class RegionGrowingCommand : ImageProcessCommand
    {
        public RegionGrowingCommand(MainViewModel viewModel)
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
                ViewModel.Processing = true;

                var process = new RegionGrowing(ViewModel.CurrentImage, new ColorCreator(ViewModel.SelectedColorSpace));

                var colors = ViewModel.CurrentColors.Select(color => color.ConvertTo(ViewModel.SelectedColorSpace)).ToList();

                process.Conditions.Add(new GaussianColorsCondition(colors, ViewModel.Tolerance, ViewModel.NeighborTolerance));

                var result = await process.ProcessAsync(NeighborhoodType.Eight);

                ViewModel.SetNewImage(result);

                ViewModel.Processing = false;
            }
        }
    }
}