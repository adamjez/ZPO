using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace ZPO.Core
{
    public static class ImageProcessing
    {
        public static WriteableBitmap EdgeDetection(WriteableBitmap source)
        {
            var vertResult = VerticalEdgeDetection(source);
            var horzResult = HorizontalEdgeDetection(source);

            return vertResult.Add(horzResult);
        }

        public static WriteableBitmap HorizontalEdgeDetection(WriteableBitmap source)
        { 
            int[,] horzKernel = { { -1, -2, -1 },
                               { 0, 0, 0 },
                               { 1, 2, 1 } };

            return source.MyConvolute(horzKernel, 1);
        }

        public static WriteableBitmap VerticalEdgeDetection(WriteableBitmap source)
        {
            int[,] vertKernel = { { -1, 0, 1 },
                               { -2, 0, 2 },
                               { -1, 0, 1 } };

            return source.MyConvolute(vertKernel, 1);
        }

        public static WriteableBitmap MakeGrayscale(WriteableBitmap original)
        {
            return original.Gray();
        }

    }
}
