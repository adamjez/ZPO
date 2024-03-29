﻿using System;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using ZPO.App.ViewModels;
using ZPO.App.Extensions;

namespace ZPO.App.Commands
{
    public class LoadImageCommand : ICommand
    {
        private readonly MainViewModel viewModel;

        public event EventHandler CanExecuteChanged { add { } remove { } }

        public LoadImageCommand(MainViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Boolean CanExecute(Object parameter) => true;

        public async void Execute(Object parameter)
        {
            FileOpenPicker openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".bmp");
            openPicker.FileTypeFilter.Add(".png");

            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                viewModel.CurrentColors.Clear();
                try
                {
                    viewModel.SourceImage = await file.AsWriteableImageAsync();
                }
                catch (Exception exc)
                {
                    viewModel.ResultMessage = "Couldn't open given file: " + exc.Message;
                    return;
                }
                viewModel.SetNewImage(viewModel.SourceImage);
                viewModel.ImageHistory.Clear();
            }
        }
    }
}