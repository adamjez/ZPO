using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;


namespace ZPO.App.Extensions
{
    public static class FileExtensions
    {
        public static async Task<WriteableBitmap> AsWriteableImageAsync(this StorageFile file)
        {
            using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);

                var result = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                await result.SetSourceAsync(fileStream);

                return result;
            }
        }
    }
}
