using System;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using ZPO.App.Extensions;
using ZPO.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ZPO.App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void LoadImageButton_Click(Object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".bmp");
            openPicker.FileTypeFilter.Add(".png");

            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                ImageView.Source = await file.AsWriteableImageAsync();

            }
            else
            {
                //
            }
        }

        private void ProcessButton_Click(Object sender, RoutedEventArgs e)
        {
            if (ImageView.Source != null && ImageView.Source is WriteableBitmap)
            {
                ImageView.Source = ImageProcessing.EdgeDetection((WriteableBitmap)ImageView.Source);
            }

        }

        private void EdgeDetectionButton1_Click(Object sender, RoutedEventArgs e)
        {
            if (ImageView.Source != null && ImageView.Source is WriteableBitmap)
            {
                ImageView.Source = ImageProcessing.VerticalEdgeDetection((WriteableBitmap)ImageView.Source);
            }
        }

        private void EdgeDetection2_Click(Object sender, RoutedEventArgs e)
        {
            if (ImageView.Source != null && ImageView.Source is WriteableBitmap)
            {
                ImageView.Source = ImageProcessing.HorizontalEdgeDetection((WriteableBitmap)ImageView.Source);
            }
        }

        private void ImageView_Tapped(Object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (ImageView.Source != null && ImageView.Source is WriteableBitmap)
            {
                var bitmap = (WriteableBitmap)ImageView.Source;
                // ToDo: Check for image dimension
                var widthFactor = bitmap.PixelWidth / ImageView.ActualWidth;

                var position = e.GetPosition(ImageView);
                var color = bitmap.GetPixel((int)(position.X * widthFactor), (int)(position.Y * widthFactor));
                ColorGrid.Background = new SolidColorBrush(color);
            }
        }

        private void RegionGrowingButton_Click(Object sender, RoutedEventArgs e)
        {
            if (ImageView.Source != null && ImageView.Source is WriteableBitmap && ColorGrid.Background is SolidColorBrush)
            {
                var grower = new RegionGrowing((WriteableBitmap)ImageView.Source);
                var color = ((SolidColorBrush)ColorGrid.Background).Color;
                ImageView.Source = grower.Process(NeighboursType.Four, color, 10);
            }
        }

        private void ToGrayButton_Click(Object sender, RoutedEventArgs e)
        {
            if (ImageView.Source != null && ImageView.Source is WriteableBitmap)
            {
                ImageView.Source = ImageProcessing.MakeGrayscale((WriteableBitmap)ImageView.Source);

            }
        }
    }
}
