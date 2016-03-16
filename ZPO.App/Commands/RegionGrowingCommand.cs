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
            // Check if we picked some color (Alpha channel equals 0 when no color
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
                process.Conditions.Add(new ColorCondition(ViewModel.CurrentColor.ToRGBColor(), 
                    (uint)ViewModel.Tolerance, (uint)ViewModel.NeighborMultiplier));

                var result = await process.ProcessAsync(NeighborhoodType.Eight);

                var resultBitmap = ViewModel.CurrentImage.CreateCopy();
                resultBitmap.FromByteArray(result);
                resultBitmap.RemoveAlphaChannel();

                ViewModel.SetNewImage(resultBitmap);

                ViewModel.Processing = false;
            }
        }
    }
}