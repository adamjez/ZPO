using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using ZPO.App.Extensions;
using ZPO.App.ViewModels;

namespace ZPO.App.Commands
{
    public class SaveImageCommand : ICommand
    {
        private readonly MainViewModel _viewModel;

        public event EventHandler CanExecuteChanged;

        public SaveImageCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel;

            viewModel.CurrentImageChanged +=
                (sender, args) => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public Boolean CanExecute(Object parameter) => _viewModel.CurrentImage != null;

        public async void Execute(Object parameter)
        {
            var savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("JPG", new List<string>() { ".jpg" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New image";

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
                // write to file
                using (InMemoryRandomAccessStream ras = new InMemoryRandomAccessStream())
                {
                    await _viewModel.CurrentImage.ToStreamAsJpeg(ras);
                    var destStream = await file.OpenStreamForWriteAsync();
                    await ras.AsStreamForRead().CopyToAsync(destStream);
                }

                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                Windows.Storage.Provider.FileUpdateStatus status =
                    await CachedFileManager.CompleteUpdatesAsync(file);

            }
        }
    }
}