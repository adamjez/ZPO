using Windows.UI.Xaml.Media.Imaging;

namespace ZPO.Core
{
    public static class ImageEdgeDetection
    {
        public static WriteableBitmap EdgeDetection(WriteableBitmap source)
        {
            var verticalKernel = VerticalEdgeDetection(source);
            var horizontalKernel = HorizontalEdgeDetection(source);

            return verticalKernel.Add(horizontalKernel);
        }

        public static WriteableBitmap HorizontalEdgeDetection(WriteableBitmap source)
        {
            int[,] horizontalKernel = { { -1, -2, -1 },
                               { 0, 0, 0 },
                               { 1, 2, 1 } };

            return source.MyConvolute(horizontalKernel, 1);
        }

        public static WriteableBitmap VerticalEdgeDetection(WriteableBitmap source)
        {
            int[,] verticalKernel = { { -1, 0, 1 },
                               { -2, 0, 2 },
                               { -1, 0, 1 } };

            return source.MyConvolute(verticalKernel, 1);
        }

        public static WriteableBitmap MakeGrayscale(WriteableBitmap original)
        {
            return original.Gray();
        }

    }
}
