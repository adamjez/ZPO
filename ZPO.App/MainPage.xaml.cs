using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using ZPO.App.ViewModels;
using ZPO.Core.Colors;

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
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ColorSpacesComboBox.SelectedIndex = 0;
            ConditionsComboBox.SelectedIndex = 0;
        }

        private void ImageView_Tapped(Object sender, TappedRoutedEventArgs e)
        {
            var viewModel = (MainViewModel)DataContext;

            if (viewModel?.CurrentImage != null)
            {
                viewModel.CurrentColors.Add(GetColorUnderPointer(e.GetPosition(ImageView)));
            }
        }
            
        private void ImageView_PointerMoved(Object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var viewModel = (MainViewModel)DataContext;

            if (viewModel?.CurrentImage != null)
            {
                viewModel.HoverColor = GetColorUnderPointer(e.GetCurrentPoint(ImageView).Position);
            }
        }

        private Color GetColorUnderPointer(Point position)
        {
            var viewModel = (MainViewModel)DataContext;

            // ToDo: Check for image dimension
            var widthFactor = viewModel.CurrentImage.PixelWidth / ImageView.ActualWidth;

            return viewModel.CurrentImage.GetPixel((int)(position.X * widthFactor), (int)(position.Y * widthFactor));

        }

        private void HistoryImage_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var viewModel = (MainViewModel)DataContext;

            if (viewModel != null && sender is Image)
            {
                var image = (Image)sender;

                var bitmap = image.Source as WriteableBitmap;
                if (bitmap != null)
                {
                    viewModel.ImageHistory.Remove(bitmap);
                    viewModel.SetNewImage(bitmap);
                }
            }
        }

        private void ImageView_OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            var viewModel = (MainViewModel)DataContext;

            viewModel.HoverColor = Color.FromArgb(0, 0, 0, 0);
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColorsGridView.SelectedItem != null)
            {
                var viewModel = (MainViewModel)DataContext;

                viewModel.CurrentColors.Remove((Color) ColorsGridView.SelectedItem);
            }
        }
    }
}
