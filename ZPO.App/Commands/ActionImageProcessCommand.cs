using System;
using Windows.UI.Xaml.Media.Imaging;
using ZPO.App.ViewModels;

namespace ZPO.App.Commands
{
    public class ActionImageProcessCommand : ImageProcessCommand
    {
        private readonly Func<WriteableBitmap, WriteableBitmap> action;

        public ActionImageProcessCommand(MainViewModel viewModel, Func<WriteableBitmap, WriteableBitmap> action)
            : base(viewModel)
        {
            this.action = action;
        }

        public override void Execute(object parameter)
        {
            if (CanExecute())
            {
                ViewModel.SetNewImage(action(ViewModel.CurrentImage));
            }
        }
    }
}