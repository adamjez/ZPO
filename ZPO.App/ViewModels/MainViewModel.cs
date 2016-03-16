using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using ZPO.App.Commands;
using ZPO.App.Extensions;
using ZPO.Core;

namespace ZPO.App.ViewModels
{
    public class MainViewModel : NotificationBase
    {
        public MainViewModel()
        {
            ImageHistory = new ObservableQueue<WriteableBitmap>(12);
            ToGrayCommand = new ActionImageProcessCommand(this, ImageEdgeDetection.MakeGrayscale);
            HorizontalEdgeDetectionCommand = 
                new ActionImageProcessCommand(this, ImageEdgeDetection.HorizontalEdgeDetection);
            VerticalEdgeDetectionCommand = 
                new ActionImageProcessCommand(this, ImageEdgeDetection.VerticalEdgeDetection);
            BothEdgesDetectionCommand =
                new ActionImageProcessCommand(this, ImageEdgeDetection.EdgeDetection);
            RegionGrowingCommand = new RegionGrowingCommand(this);
            LoadImageCommand = new LoadImageCommand(this);
            // Default tolerance value
            Tolerance = 20;
            NeighborMultiplier = 20;
        }

        public ICommand BothEdgesDetectionCommand { get; private set; }
        public ICommand HorizontalEdgeDetectionCommand { get; private set; }
        public ICommand VerticalEdgeDetectionCommand { get; private set; }
        public ICommand ToGrayCommand { get; private set; }
        public ICommand RegionGrowingCommand { get; private set; }
        public ICommand LoadImageCommand { get; private set; }

        ObservableCollection<WriteableBitmap> _imageHistory
           = new ObservableCollection<WriteableBitmap>();
        public ObservableCollection<WriteableBitmap> ImageHistory
        {
            get { return _imageHistory; }
            set { SetProperty(ref _imageHistory, value); }
        }

        WriteableBitmap _currentImage;
        public WriteableBitmap CurrentImage
        {
            get { return _currentImage; }
            private set { SetProperty(ref _currentImage, value); }
        }

        Color _currentColor;
        public Color CurrentColor
        {
            get { return _currentColor; }
            set
            {
                if (SetProperty(ref _currentColor, value))
                {
                    CurrentColorChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        Color _hoverColor;
        public Color HoverColor
        {
            get { return _hoverColor; }
            set { SetProperty(ref _hoverColor, value); }
        }

        int _tolerance;
        public int Tolerance
        {
            get { return _tolerance; }
            set { SetProperty(ref _tolerance, value); }
        }

        int _neighborMultiplier;
        public int NeighborMultiplier
        {
            get { return _neighborMultiplier; }
            set { SetProperty(ref _neighborMultiplier, value); }
        }

        bool _processing;
        public bool Processing
        {
            get { return _processing; }
            set { SetProperty(ref _processing, value); }
        }


        public void SetNewImage(WriteableBitmap image)
        {
            if(CurrentImage != null)
                ImageHistory.Insert(0, CurrentImage);

            CurrentImage = image;

            CurrentImageChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler CurrentImageChanged;
        public event EventHandler CurrentColorChanged;
    }
}
