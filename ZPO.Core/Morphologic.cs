using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace ZPO.Core
{
    public static class Morphology
    {
        public enum MorphologyType
        {
            Erosion,
            Dilation
        }

        public static WriteableBitmap Dilate(WriteableBitmap sourceBitmap)
        {
            return DilateAndErodeFilter(sourceBitmap, 3, MorphologyType.Dilation);
        }

        public static WriteableBitmap Erode(WriteableBitmap sourceBitmap)
        {
            return DilateAndErodeFilter(sourceBitmap, 3, MorphologyType.Erosion);
        }

        public static WriteableBitmap DilateAndErodeFilter(
                           WriteableBitmap sourceBitmap,
                           int matrixSize,
                           MorphologyType morphType,
                           bool applyBlue = true,
                           bool applyGreen = true,
                           bool applyRed = true)
        {
            byte[] pixelBuffer = sourceBitmap.ToByteArray();

            var stride = sourceBitmap.PixelWidth * 4;

            byte[] resultBuffer = new byte[sourceBitmap.PixelHeight * stride];


            var filterOffset = (matrixSize - 1) / 2;
            int calcOffset = 0;


            int byteOffset = 0;


            byte blue = 0;
            byte green = 0;
            byte red = 0;


            byte morphResetValue = 0;


            if (morphType == MorphologyType.Erosion)
            {
                morphResetValue = 255;
            }


            for (int offsetY = filterOffset; offsetY <
                sourceBitmap.PixelHeight - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX <
                    sourceBitmap.PixelWidth - filterOffset; offsetX++)
                {
                    byteOffset = offsetY * stride + offsetX * 4;


                    blue = morphResetValue;
                    green = morphResetValue;
                    red = morphResetValue;


                    if (morphType == MorphologyType.Dilation)
                    {
                        for (int filterY = -filterOffset;
                            filterY <= filterOffset; filterY++)
                        {
                            for (int filterX = -filterOffset;
                                filterX <= filterOffset; filterX++)
                            {
                                calcOffset = byteOffset +
                                             (filterX * 4) +
                                (filterY * stride);


                                if (pixelBuffer[calcOffset] > blue)
                                {
                                    blue = pixelBuffer[calcOffset];
                                }


                                if (pixelBuffer[calcOffset + 1] > green)
                                {
                                    green = pixelBuffer[calcOffset + 1];
                                }


                                if (pixelBuffer[calcOffset + 2] > red)
                                {
                                    red = pixelBuffer[calcOffset + 2];
                                }
                            }
                        }
                    }
                    else if (morphType == MorphologyType.Erosion)
                    {
                        for (int filterY = -filterOffset;
                            filterY <= filterOffset; filterY++)
                        {
                            for (int filterX = -filterOffset;
                                filterX <= filterOffset; filterX++)
                            {
                                calcOffset = byteOffset + (filterX * 4) + (filterY * stride);

                                if (pixelBuffer[calcOffset] < blue)
                                {
                                    blue = pixelBuffer[calcOffset];
                                }

                                if (pixelBuffer[calcOffset + 1] < green)
                                {
                                    green = pixelBuffer[calcOffset + 1];
                                }

                                if (pixelBuffer[calcOffset + 2] < red)
                                {
                                    red = pixelBuffer[calcOffset + 2];
                                }
                            }
                        }
                    }


                    if (applyBlue == false)
                    {
                        blue = pixelBuffer[byteOffset];
                    }


                    if (applyGreen == false)
                    {
                        green = pixelBuffer[byteOffset + 1];
                    }


                    if (applyRed == false)
                    {
                        red = pixelBuffer[byteOffset + 2];
                    }


                    resultBuffer[byteOffset] = blue;
                    resultBuffer[byteOffset + 1] = green;
                    resultBuffer[byteOffset + 2] = red;
                    resultBuffer[byteOffset + 3] = 255;
                }
            }


            var resultBitmap = sourceBitmap.CreateCopy();

            resultBitmap.FromByteArray(resultBuffer);

            return resultBitmap;
        }
    }
}
