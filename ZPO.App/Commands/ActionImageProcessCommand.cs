﻿using System;
using Windows.UI.Xaml.Media.Imaging;
using ZPO.App.ViewModels;

namespace ZPO.App.Commands
{
    public class ActionImageProcessCommand : ImageProcessCommand
    {
        private readonly Func<WriteableBitmap, WriteableBitmap> _action;

        public ActionImageProcessCommand(MainViewModel viewModel, Func<WriteableBitmap, WriteableBitmap> action)
            : base(viewModel)
        {
            _action = action;
        }

        public override void Execute(object parameter)
        {
            if (CanExecute())
            {
                ViewModel.SetNewImage(
                    _action(ViewModel.CurrentImage));
            }
        }
    }
}