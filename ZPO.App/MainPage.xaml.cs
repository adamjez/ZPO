using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using ZPO.App.ViewModels;

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

        /// <summary>
        /// Hack for creating default values of comboboxes
        /// </summary>
        private void MainPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            NeighborhoodTypeComboBox.SelectedIndex = 2;
            PathMethodComboBox.SelectedIndex = 1;
            ColorSpacesComboBox.SelectedIndex = 0;
            ConditionsComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Adds color under pointer on main image to viewmodel
        /// </summary>
        private void ImageView_Tapped(Object sender, TappedRoutedEventArgs e)
        {
            var viewModel = (MainViewModel)DataContext;

            if (viewModel?.CurrentImage != null)
            {
                viewModel.CurrentColors.Add(GetColorUnderPointer(e.GetPosition(ImageView)));
            }
        }

        /// <summary>
        /// Adds color under pointer on main image to viewmodel hover color
        /// </summary>
        private void ImageView_PointerMoved(Object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var viewModel = (MainViewModel)DataContext;

            if (viewModel?.CurrentImage != null)
            {
                viewModel.HoverColor = GetColorUnderPointer(e.GetCurrentPoint(ImageView).Position);
            }
        }

        /// <summary>
        /// Resolve color under main image with given position
        /// </summary>
        /// <param name="position">Position of cursor under image</param>
        /// <returns></returns>
        private Color GetColorUnderPointer(Point position)
        {
            var viewModel = (MainViewModel)DataContext;

            var widthFactor = viewModel.CurrentImage.PixelWidth / ImageView.ActualWidth;
            var heightFactor = viewModel.CurrentImage.PixelHeight / ImageView.ActualHeight;

            return viewModel.CurrentImage.GetPixel((int)(position.X * widthFactor), (int)(position.Y * heightFactor));

        }

        /// <summary>
        /// Handles tapping on images in history, resolves tapped image and 
        /// sets it to current image in viewmodel
        /// </summary>
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

        /// <summary>
        /// Handles situation when cursor isn't under image so its reset hover color
        /// </summary>
        private void ImageView_OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            var viewModel = (MainViewModel)DataContext;

            viewModel.HoverColor = Color.FromArgb(0, 0, 0, 0);
        }

        /// <summary>
        /// Handles tapping at colors from gridview and removes it from selected colors
        /// </summary>
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
