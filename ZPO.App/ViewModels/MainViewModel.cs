using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using ZPO.App.Commands;
using ZPO.App.Enums;
using ZPO.App.Extensions;
using ZPO.Core;
using ZPO.Core.Colors;

namespace ZPO.App.ViewModels
{
    /// <summary>
    /// MainViewModel contains all properties and commands from MainPage
    /// </summary>
    public class MainViewModel : NotificationBase
    {
        /// <summary>
        /// Creates default values of properties and initialize commands
        /// </summary>
        public MainViewModel()
        {
            ImageHistory = new ObservableQueue<WriteableBitmap>(12);
            ToGrayCommand = new ActionImageProcessCommand(this, ImageProcessing.MakeGrayscale);
            BlurCommand = new ActionImageProcessCommand(this, ImageBlur.GaussianBlur);
            DilateCommand = new ActionImageProcessCommand(this, Morphology.Dilate);
            ErodeCommand = new ActionImageProcessCommand(this, Morphology.Erode);
            RegionGrowingCommand = new PathMethodCommand(this);
            LoadImageCommand = new LoadImageCommand(this);
            SaveImageCommand = new SaveImageCommand(this);
            // Default tolerance value
            Tolerance = 10;
            NeighborTolerance = 10;
        }

        public ICommand BlurCommand { get; private set; }
        public ICommand DilateCommand { get; private set; }
        public ICommand ErodeCommand { get; private set; }
        public ICommand ToGrayCommand { get; private set; }
        public ICommand RegionGrowingCommand { get; private set; }
        public ICommand LoadImageCommand { get; private set; }
        public ICommand SaveImageCommand { get; private set; }


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

        public WriteableBitmap SourceImage { get; set; }

        ObservableCollection<Color> currentColors
           = new ObservableCollection<Color>();
        public ObservableCollection<Color> CurrentColors
        {
            get { return currentColors; }
            set { SetProperty(ref currentColors, value); }
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

        int _neighborTolerance;
        public int NeighborTolerance
        {
            get { return _neighborTolerance; }
            set { SetProperty(ref _neighborTolerance, value); }
        }

        bool _processing;

        public bool Processing
        {
            get { return _processing; }
            set { SetProperty(ref _processing, value); }
        }

        ColorSpaces selectedColorSpace = Core.Colors.ColorSpaces.HSL;

        public ColorSpaces SelectedColorSpace
        {
            get { return selectedColorSpace; }
            set
            {
                CheckRestrictions();
                SetProperty(ref selectedColorSpace, value);
            }
        }


        public IEnumerable<ColorSpaces> ColorSpaces 
            => Enum.GetValues(typeof(ColorSpaces)).Cast<ColorSpaces>();

        private ConditionType selectedCondition;
        public ConditionType SelectedCondition
        {
            get { return selectedCondition; }
            set
            {
                CheckRestrictions();
                SetProperty(ref selectedCondition, value);
            }
        }

        public IEnumerable<ConditionType> Conditions
            => Enum.GetValues(typeof(ConditionType)).Cast<ConditionType>();

        private PathMethods thresholdMethod;

        public PathMethods ThresholdMethod
        {
            get { return thresholdMethod; }
            set { SetProperty(ref thresholdMethod, value); }
        }

        public IEnumerable<PathMethods> ThresholdMethods
            => Enum.GetValues(typeof(PathMethods)).Cast<PathMethods>();

        private NeighborhoodType neighborhoodType;

        public NeighborhoodType NeighborhoodType
        {
            get { return neighborhoodType; }
            set { SetProperty(ref neighborhoodType, value); }
        }

        public IEnumerable<NeighborhoodType> NeighborhoodTypes
            => Enum.GetValues(typeof(NeighborhoodType)).Cast<NeighborhoodType>();

        private bool moreActions;
        public bool MoreActions
        {
            get { return moreActions; }
            set { SetProperty(ref moreActions, value); }
        }

        private bool dynamicThreshold;

        public bool DynamicThreshold
        {
            get { return dynamicThreshold; }
            set { SetProperty(ref dynamicThreshold, value); }
        }

        private string resultMessage;
        public string ResultMessage
        {
            get { return resultMessage; }
            set { SetProperty(ref resultMessage, value); }
        }

        /// <summary>
        /// Dirty hack to restrict condition type with color space
        /// </summary>
        private void CheckRestrictions()
        {
            if (SelectedCondition == ConditionType.NormalDistributionFromColor)
            {
                if (SelectedColorSpace != Core.Colors.ColorSpaces.RGB ||
                    SelectedColorSpace != Core.Colors.ColorSpaces.GrayScale)
                {
                    // Have to set direct to private field to not to get stackoverflow exception
                    // It's fallback value to not allow unsupported color space with given condition
                    selectedColorSpace = Core.Colors.ColorSpaces.GrayScale;
                }
            }
        }

        /// <summary>
        /// Sets new image to UI and adds old to history stack
        /// </summary>
        /// <param name="image">new image</param>
        public void SetNewImage(WriteableBitmap image)
        {
            if(CurrentImage != null)
                ImageHistory.Insert(0, CurrentImage);

            CurrentImage = image;

            CurrentImageChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler CurrentImageChanged;
    }
}
