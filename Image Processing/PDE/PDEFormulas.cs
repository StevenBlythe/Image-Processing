using System;
using System.Drawing;
using System.Windows.Controls;

namespace Image_Processing.PDE
{
    public static class PDEFormulas
    {
        // Heat Equation
        public static Bitmap HeatEquation(Bitmap originalImage, int loops, ProgressBar progressBar)
        {
            // Clone Image -- 
            Bitmap previousImage = (Bitmap) originalImage.Clone();

            // Modified Image (n + 1)
            Bitmap augmentedImage = (Bitmap)originalImage.Clone();

            // Heat Equation:
            // ut = uxx + uyy
            // \Delta x and \Delta y are both 1, since we are measuring in units of one pixel.
            // Refer to 11.7, Heat Equation

            double deltaTime = 0.1;
            double top;


            // Initialize Red/Green/Blue pixel values
            int red;
            int green;
            int blue;

            // We do not consider the boundaries
            // Remove border of image from our loop, [0, Width] -> (0, Width)
            double totalCalculations = loops * previousImage.Width;
            double incrementCalculations = 1 / totalCalculations;
            int progressValue = 0;

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
                progressValue = (int)(Math.Round(progressValue + incrementCalculations, 0));
                progressBar.Value = progressValue;
            }

            return augmentedImage;
        }

        // Shock Filter

        // Level set equation

        // Modified Level Set



    // Heat Equation



    // Add noise to image?
}
}
