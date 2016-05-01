using Windows.UI.Xaml.Media.Imaging;

namespace ZPO.Core
{
    public static class ImageProcessing
    {
        public static WriteableBitmap MakeGrayscale(WriteableBitmap original)
        {
            return original.Gray();
        }
    }
}
