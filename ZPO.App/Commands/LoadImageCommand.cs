using System;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using ZPO.App.ViewModels;
using ZPO.App.Extensions;

namespace ZPO.App.Commands
{
    public class LoadImageCommand : ICommand
    {
        private readonly MainViewModel _viewModel;

        public event EventHandler CanExecuteChanged { add { } remove { } }

        public LoadImageCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel;
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
                _viewModel.CurrentColors.Clear();
                _viewModel.SetNewImage(await file.AsWriteableImageAsync());
            }
        }
    }
}