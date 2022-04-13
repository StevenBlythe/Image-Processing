using System;
using System.Collections.Generic;
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
                previousImage = (Bitmap)augmentedImage.Clone();
            }

            return augmentedImage;
        }

        // Level Set
        public static Bitmap LevelSet(Bitmap originalImage, int loops)
        {
            // Clone Image -- 
            Bitmap previousImage = (Bitmap)originalImage.Clone();

            // Modified Image (n + 1)
            Bitmap augmentedImage = (Bitmap)originalImage.Clone();

            // Level Set:
            // Refer to pg 76


            int utRed;
            int utGreen;
            int utBlue;



            IDictionary<string, int> pixelRed = new Dictionary<string, int>();
            IDictionary<string, int> pixelGreen = new Dictionary<string, int>();
            IDictionary<string, int> pixelBlue = new Dictionary<string, int>();
            for (int t = 0; t < loops; t++)
            {
                for (int i = 1; i < previousImage.Width - 1; i++)
                {
                    for (int j = 1; j < previousImage.Height - 1; j++)
                    {
                        // Format: U R D L UR DR DL UL for pixelRed, pixelGreen, pixelBlue + Original Center for original image
                        UtilityMethods.GetRGBValues(pixelRed, pixelGreen, pixelBlue, previousImage, originalImage, i, j);

                        // Support methods, 
                        // 
                        utRed =   LevelSetCalculation(pixelRed);
                        utGreen = LevelSetCalculation(pixelGreen);
                        utBlue =  LevelSetCalculation(pixelBlue);

                        augmentedImage.SetPixel(i, j, Color.FromArgb(utRed, utGreen, utBlue));
                    }
                }
                previousImage = (Bitmap)augmentedImage.Clone();
            }
            return augmentedImage;
        }

        // Level set equation
        // TODO add multithread calculations
        // Currently no/limited workaround, consult to find way to pass property
        private static int LevelSetCalculation(IDictionary<string, int> pixel)
        {
            double ux, uy, uxy, uyy, uxx, top, bottom, value;
            int pixelValue;

            ux  = (double)(pixel["R"] - pixel["L"]) / 2;
            uy  = (double)(pixel["U"] - pixel["D"]) / 2;
            uxy = (double)(pixel["UR"] - pixel["UL"] - pixel["DR"] + pixel["DL"])/4;
            uyy = (double)(pixel["U"] - 2 * pixel["C"] + pixel["D"]);
            uxx = (double)(pixel["R"] - 2 * pixel["C"] + pixel["L"]);

            top = (ux * ux * uyy) - (2 * ux * uy * uxy) + (uy * uy * uxx);
            bottom = (ux * ux) + (uy * uy) + 0.0001;

            value = (double)((top / bottom)*0.25 + pixel["C"]);

            pixelValue = (int)Math.Round(value, 0);

            if (value < 0) return 0;
            if (value > 255) return 255;
            else return pixelValue;
        }


        // Modified Level Set
        public static Bitmap ModifiedLevelSet(Bitmap originalImage, int loops)
        {
            // Clone Image -- 
            Bitmap previousImage = (Bitmap)originalImage.Clone();

            // Modified Image (n + 1)
            Bitmap augmentedImage = (Bitmap)originalImage.Clone();

            // Level Set:
            // Refer to pg 76


            int utRed;
            int utGreen;
            int utBlue;



            IDictionary<string, int> pixelRed = new Dictionary<string, int>();
            IDictionary<string, int> pixelGreen = new Dictionary<string, int>();
            IDictionary<string, int> pixelBlue = new Dictionary<string, int>();
            for (int t = 0; t < loops; t++)
            {
                for (int i = 1; i < previousImage.Width - 1; i++)
                {
                    for (int j = 1; j < previousImage.Height - 1; j++)
                    {
                        // Format: U R D L UR DR DL UL for pixelRed, pixelGreen, pixelBlue + Original Center for original image
                        UtilityMethods.GetRGBValues(pixelRed, pixelGreen, pixelBlue, previousImage, originalImage, i, j);

                        // Support methods, 
                        // 
                        utRed   = ModifiedLevelSetCalculation(pixelRed);
                        utGreen = ModifiedLevelSetCalculation(pixelGreen);
                        utBlue  = ModifiedLevelSetCalculation(pixelBlue);

                        augmentedImage.SetPixel(i, j, Color.FromArgb(utRed, utGreen, utBlue));
                    }
                }
                previousImage = (Bitmap)augmentedImage.Clone();
            }
            return augmentedImage;
        }

        private static int ModifiedLevelSetCalculation(IDictionary<string, int> pixel)
        {
            // Modified Level Set term
            double alpha = 0.2;
            double modified = alpha * (pixel["C"] - pixel["OC"]);

            double ux, uy, uxy, uyy, uxx, top, bottom, value;
            int pixelValue;

            ux  = (double)(pixel["R"] - pixel["L"]) / 2;
            uy  = (double)(pixel["U"] - pixel["D"]) / 2;
            uxy = (double)(pixel["UR"] - pixel["UL"] - pixel["DR"] + pixel["DL"]) / 4;
            uyy = (double)(pixel["U"] - 2 * pixel["C"] + pixel["D"]);
            uxx = (double)(pixel["R"] - 2 * pixel["C"] + pixel["L"]);

            top = (ux * ux * uyy) - (2 * ux * uy * uxy) + (uy * uy * uxx);
            bottom = (ux * ux) + (uy * uy) + 0.0001;

            value = (double)((top / bottom - modified) * 0.25 + pixel["C"]);

            pixelValue = (int)Math.Round(value, 0);

            if (value < 0) return 0;
            if (value > 255) return 255;
            else return pixelValue;
        }


        // Shock Filter
        // Level Set
        public static Bitmap ShockFilter(Bitmap originalImage, int loops)
        {
            // Clone Image -- 
            Bitmap previousImage = (Bitmap)originalImage.Clone();

            // Modified Image (n + 1)
            Bitmap augmentedImage = (Bitmap)originalImage.Clone();

            // Level Set:
            // Refer to pg 76


            int utRed;
            int utGreen;
            int utBlue;



            IDictionary<string, int> pixelRed = new Dictionary<string, int>();
            IDictionary<string, int> pixelGreen = new Dictionary<string, int>();
            IDictionary<string, int> pixelBlue = new Dictionary<string, int>();
            for (int t = 0; t < loops; t++)
            {
                for (int i = 1; i < previousImage.Width - 1; i++)
                {
                    for (int j = 1; j < previousImage.Height - 1; j++)
                    {
                        // Format: U R D L UR DR DL UL for pixelRed, pixelGreen, pixelBlue + Original Center for original image
                        UtilityMethods.GetRGBValues(pixelRed, pixelGreen, pixelBlue, previousImage, originalImage, i, j);

                        // Support methods, 
                        // 
                        utRed   = ShockFilterCalculation(pixelRed);
                        utGreen = ShockFilterCalculation(pixelGreen);
                        utBlue  = ShockFilterCalculation(pixelBlue);

                        augmentedImage.SetPixel(i, j, Color.FromArgb(utRed, utGreen, utBlue));
                    }
                }
                previousImage = (Bitmap)augmentedImage.Clone();
            }
            return augmentedImage;
        }

        // Level set equation
        // TODO add multithread calculations
        // Currently no/limited workaround, consult to find way to pass property
        private static int ShockFilterCalculation(IDictionary<string, int> pixel)
        {
            double ux, uy, uxy, uyy, uxx, top, bottom, value;
            int pixelValue;

            // Use Given Equations
            //uxy = (double)(pixel["UR"] - pixel["UL"] - pixel["DR"] + pixel["DL"]) / 4;
            uyy = (double)(pixel["U"] - 2 * pixel["C"] + pixel["D"]);
            uxx = (double)(pixel["R"] - 2 * pixel["C"] + pixel["L"]);

            // If Cases
            int uxF, uxB, uyF, uyB;
            uxF = pixel["R"] - pixel["C"];
            uxB = pixel["C"] - pixel["L"];
            
            if (uxF * uxB < 0) { ux = 0; }
            else if (uxF > 0) { ux = Math.Min(Math.Abs(uxF), Math.Abs(uxB)); }
            else { ux = - Math.Min(Math.Abs(uxF), Math.Abs(uxB)); }

            uyF = pixel["U"] - pixel["C"];
            uyB = pixel["C"] - pixel["D"];


            uy = (double)(pixel["U"] - pixel["D"]) / 2;

            top = (ux * ux * uyy) - (2 * ux * uy * uxy) + (uy * uy * uxx);
            bottom = (ux * ux) + (uy * uy) + 0.0001;

            value = (double)((top / bottom) * 0.25 + pixel["C"]);

            pixelValue = (int)Math.Round(value, 0);

            if (value < 0) return 0;
            if (value > 255) return 255;
            else return pixelValue;
        }
    }
}
