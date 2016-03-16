using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using ZPO.App.ViewModels;
using ZPO.Core;
using ZPO.Core.Algorithms;
using ZPO.Core.Colors;

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

        public override async void Execute(object parameter)
        {
            if (CanExecute())
            {
                ViewModel.Processing = true;

                var process = new RegionGrowing(ViewModel.CurrentImage, new ColorCreator(ColorTypes.RGB));

                var result = process.Process(ViewModel.CurrentColor.ToRGBColor(), (uint) ViewModel.Tolerance,
                            (uint) ViewModel.NeighborMultiplier, NeighborhoodType.Eight);
                
                ViewModel.SetNewImage(result);

                ViewModel.Processing = false;
            }
        }
    }
}