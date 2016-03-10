using System;
using Windows.UI.Xaml.Media.Imaging;
using ZPO.App.ViewModels;
using ZPO.Core;

namespace ZPO.App.Commands
{
    public class RegionGrowingCommand : ImageProcessCommand
    {
        public RegionGrowingCommand(MainViewModel viewModel) 
            : base(viewModel)
        {
            ViewModel.CurrentColorChanged +=
                (sender, args) => OnExecuteChanged();
        }

        public override bool CanExecute(object parameter = null)
        {
            // Check if we picked some color (Alpha channel equels 0 when no color
            // is picked yet)
            if (ViewModel.CurrentColor.A == 0)
            {
                return false;
            }
            return base.CanExecute(parameter);
        }

        public override void Execute(object parameter)
        {
            if (CanExecute())
            {
                var process = new RegionGrowing(ViewModel.CurrentImage);
                ViewModel.SetNewImage(
                    process.Process(NeighboursType.Eight, ViewModel.CurrentColor, 5));
            }
        }
    }
}