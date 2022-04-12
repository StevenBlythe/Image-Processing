using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace Image_Processing.PDE
{
    public static class PDEFormulas
    {
        // Heat Equation
        public static Bitmap HeatEquation(Bitmap originalImage, int loops)
        {
            // Clone Image -- 
            Bitmap previousImage = (Bitmap) originalImage.Clone();

            // Modified Image (n + 1)
            Bitmap augmentedImage = (Bitmap)originalImage.Clone();

            // Heat Equation:
            // ut = uxx + uyy
            // \Delta x and \Delta y are both 1, since we are measuring in units of one pixel.
            // Refer to 11.7, Heat Equation

            double deltaTime = 0.25;
            double top;


            // Initialize Red/Green/Blue pixel values
            int red;
            int green;
            int blue;

            // We do not consider the boundaries
            // Remove border of image from our loop, [0, Width] -> (0, Width)
            double calculations = loops * previousImage.Width;
            for (int t = 0; t < loops; t++)
            { 
                for (int i = 1; i < previousImage.Width-1; i++)
                {
                    for (int j = 1; j < previousImage.Height-1; j++)
                    {
                        // In total, we will call 5 pixels, + shaped.
                        // Center Pixel
                        Color pixelCenter = previousImage.GetPixel(i, j);

                        // x-Axis
                        Color pixelLeft  = previousImage.GetPixel(i-1, j);
                        Color pixelRight = previousImage.GetPixel(i+1, j);

                        // y-Axis
                        Color pixelDown  = previousImage.GetPixel(i, j-1);
                        Color pixelUp    = previousImage.GetPixel(i, j+1);

                        // Colors vs Gray
                        // Color values have three distinguished values for RGB.
                        // Gray values have R = G = B, therefore for we only must do
                        // the function once for gray values.

                        // Red/Gray
                        top = (double)(pixelRight.R + pixelLeft.R - 4 * pixelCenter.R + pixelUp.R + pixelDown.R);
                        red = (int)(top * deltaTime + (double)(pixelCenter.R));

                        top = (double)(pixelRight.G + pixelLeft.G - 4 * pixelCenter.G + pixelUp.G + pixelDown.G);
                        green = (int)(top * deltaTime + (double)(pixelCenter.G));

                        top = (double)(pixelRight.B + pixelLeft.B - 4 * pixelCenter.B + pixelUp.B + pixelDown.B);
                        blue = (int)(top * deltaTime + (double)(pixelCenter.B));

                        augmentedImage.SetPixel(i, j, Color.FromArgb(red, green, blue));
                    }
                }
            }

            return augmentedImage;
        }

        // Shock Filter
        public static Bitmap ShockFilter(Bitmap originalImage, int loops)
        {
            // Clone Image -- 
            Bitmap previousImage = (Bitmap)originalImage.Clone();

            // Modified Image (n + 1)
            Bitmap augmentedImage = (Bitmap)originalImage.Clone();

            // Level Set:
            // Refer to pg 76

            double deltaTime = 0.25;

            int red;
            int green;
            int blue;

            double ux, uy, uxy, uyy, uxx, top, bottom;
            int pixelValue;
            for (int t = 0; t < loops; t++)
            {
                for (int i = 1; i < previousImage.Width - 1; i++)
                {
                    for (int j = 1; j < previousImage.Height - 1; j++)
                    {
                        // Center Pixel
                        Color pixelCenter = previousImage.GetPixel(i, j);

                        // x-Axis
                        Color pixelLeft = previousImage.GetPixel(i - 1, j);
                        Color pixelRight = previousImage.GetPixel(i + 1, j);

                        // y-Axis
                        Color pixelDown = previousImage.GetPixel(i, j - 1);
                        Color pixelUp = previousImage.GetPixel(i, j + 1);

                        ux = 0;
                        uy = 0;
                        uxy = 0;
                        uyy = 0;
                        uxx = 0;
                        top = 0;
                        bottom = 0;

                        pixelValue = (int)Math.Round(top / bottom, 0);
                        // Red/Gray
                        top = (double)(pixelRight.R + pixelLeft.R - 4 * pixelCenter.R + pixelUp.R + pixelDown.R);
                        red = (int)(top * deltaTime + (double)(pixelCenter.R));

                        augmentedImage.SetPixel(i, j, Color.FromArgb(red, red, red));
                    }
                }
            }
            return augmentedImage;
        }

        // Level set equation

        // Modified Level Set



        // Heat Equation



        // Add noise to image?
    }
}
