﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections;

namespace Image_Processing.PDE
{
    public static class PDEFormulas
    {
        // Heat Equation
        public static Bitmap HeatEquation(Bitmap originalImage, int loops, double delta, string paletteChoice, int saveIncrements, string imageName, string imagePath)
        {
            // Clone Image -- 
            Bitmap previousImage = (Bitmap) originalImage.Clone();

            // Modified Image (n + 1)
            Bitmap augmentedImage = (Bitmap)originalImage.Clone();

            // Name for autosave
            string fileName;
            imagePath += "Heat Equation delta " + delta +"\\";

            if (!Directory.Exists(imagePath) && saveIncrements > 0)
                _ = Directory.CreateDirectory(imagePath);

            // Heat Equation:
            // ut = uxx + uyy
            // \Delta x and \Delta y are both 1, since we are measuring in units of one pixel.
            // Refer to 11.7, Heat Equation

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
                if (saveIncrements > 0 && t != -1 && t % saveIncrements == 0)
                {
                    fileName = imagePath + imageName + " - " + t + ".png";
                    previousImage.Save(fileName, ImageFormat.Png);
                }
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
                        red = (int)(top * delta + (double)(pixelCenter.R));

                        if (paletteChoice == "Color") 
                        { 
                            top = (double)(pixelRight.G + pixelLeft.G - 4 * pixelCenter.G + pixelUp.G + pixelDown.G);
                            green = (int)(top * delta + (double)(pixelCenter.G));

                            top = (double)(pixelRight.B + pixelLeft.B - 4 * pixelCenter.B + pixelUp.B + pixelDown.B);
                            blue = (int)(top * delta + (double)(pixelCenter.B));
                            augmentedImage.SetPixel(i, j, Color.FromArgb(red, green, blue));
                        } 
                        else
                            augmentedImage.SetPixel(i, j, Color.FromArgb(red, red, red));

                    }
                }
                previousImage = (Bitmap)augmentedImage.Clone();
            }

            if (saveIncrements > 0)
                previousImage.Save(imagePath + imageName + " - " + loops + ".png", ImageFormat.Png);

            return augmentedImage;
        }

        // Level Set
        public static Bitmap LevelSet(Bitmap originalImage, int loops, double delta, string paletteChoice, int saveIncrements, string imageName, string imagePath)
        {
            // Clone Image -- 
            Bitmap previousImage = (Bitmap)originalImage.Clone();

            // Modified Image (n + 1)
            Bitmap augmentedImage = (Bitmap)originalImage.Clone();

            string fileName;
            imagePath += "Level Set delta " + delta + "\\";

            if (!Directory.Exists(imagePath) && saveIncrements > 0)
                _ = Directory.CreateDirectory(imagePath);

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
                if (saveIncrements > 0 && t != -1 && t % saveIncrements == 0)
                {
                    fileName = imagePath + imageName + " - " + t + ".png";
                    previousImage.Save(fileName, ImageFormat.Png);
                }
                for (int i = 1; i < previousImage.Width - 1; i++)
                {
                    for (int j = 1; j < previousImage.Height - 1; j++)
                    {
                        // Format: U R D L UR DR DL UL for pixelRed, pixelGreen, pixelBlue + Original Center for original image
                        UtilityMethods.GetRGBValues(pixelRed, pixelGreen, pixelBlue, previousImage, originalImage, i, j);

                        // Support methods, 
                        if (paletteChoice == "Color") {
                        utRed =   LevelSetCalculation(pixelRed, delta);
                        utGreen = LevelSetCalculation(pixelGreen, delta);
                        utBlue =  LevelSetCalculation(pixelBlue, delta);
                        augmentedImage.SetPixel(i, j, Color.FromArgb(utRed, utGreen, utBlue));
                        } else
                        {
                            utRed = LevelSetCalculation(pixelRed, delta);
                            augmentedImage.SetPixel(i, j, Color.FromArgb(utRed, utRed, utRed));

                        }

                    }
                }
                previousImage = (Bitmap)augmentedImage.Clone();
            }
            if (saveIncrements > 0)
                previousImage.Save(imagePath + imageName + " - " + loops + ".png", ImageFormat.Png);

            return augmentedImage;
        }

        // Level set equation
        // TODO add multithread calculations
        // Currently no/limited workaround, consult to find way to pass property
        private static int LevelSetCalculation(IDictionary<string, int> pixel, double delta)
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

            value = (double)((top / bottom)*delta + pixel["C"]);

            pixelValue = (int)Math.Round(value, 0);

            if (value < 0) return 0;
            if (value > 255) return 255;
            else return pixelValue;
        }


        // Modified Level Set
        public static Bitmap ModifiedLevelSet(Bitmap originalImage, int loops, double alpha, double delta, string paletteChoice, int saveIncrements, string imageName, string imagePath)
        {
            // Clone Image -- 
            Bitmap previousImage = (Bitmap)originalImage.Clone();

            // Modified Image (n + 1)
            Bitmap augmentedImage = (Bitmap)originalImage.Clone();

            string fileName;
            imagePath += "Modified Level Set delta " + delta + " alpha " + alpha + "\\";

            if (!Directory.Exists(imagePath) && saveIncrements > 0)
                _ = Directory.CreateDirectory(imagePath);

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
                if (saveIncrements > 0 && t != -1 && t % saveIncrements == 0)
                {
                    fileName = imagePath + imageName + " - " + t + ".png";
                    previousImage.Save(fileName, ImageFormat.Png);
                }
                for (int i = 1; i < previousImage.Width - 1; i++)
                {
                    for (int j = 1; j < previousImage.Height - 1; j++)
                    {
                        // Format: U R D L UR DR DL UL for pixelRed, pixelGreen, pixelBlue + Original Center for original image
                        UtilityMethods.GetRGBValues(pixelRed, pixelGreen, pixelBlue, previousImage, originalImage, i, j);

                        // Support methods, 
                        if (paletteChoice == "Color") {
                            utRed = ModifiedLevelSetCalculation(pixelRed, delta, alpha);
                            utGreen = ModifiedLevelSetCalculation(pixelGreen, delta, alpha);
                            utBlue = ModifiedLevelSetCalculation(pixelBlue, delta, alpha);
                            augmentedImage.SetPixel(i, j, Color.FromArgb(utRed, utGreen, utBlue));
                        }
                        else {
                            utRed = ModifiedLevelSetCalculation(pixelRed, delta, alpha);
                            augmentedImage.SetPixel(i, j, Color.FromArgb(utRed, utRed, utRed));
                        }
                    }
                }
                previousImage = (Bitmap)augmentedImage.Clone();
            }
            if (saveIncrements > 0)
                previousImage.Save(imagePath + imageName + " - " + loops + ".png", ImageFormat.Png);

            return augmentedImage;
        }

        private static int ModifiedLevelSetCalculation(IDictionary<string, int> pixel, double delta, double alpha)
        {
            // Modified Level Set term
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

            value = (double)((top / bottom - modified) * delta + pixel["C"]);

            pixelValue = (int)Math.Round(value, 0);

            if (value < 0) return 0;
            if (value > 255) return 255;
            else return pixelValue;
        }


        // Shock Filter
        // Level Set
        public static Bitmap ShockFilter(Bitmap originalImage, int loops, double delta, string paletteChoice, int saveIncrements, string imageName, string imagePath)
        {
            // Clone Image -- 
            Bitmap previousImage = (Bitmap)originalImage.Clone();

            // Modified Image (n + 1)
            Bitmap augmentedImage = (Bitmap)originalImage.Clone();

            string fileName;

            imagePath += "Shock Filter delta " + delta + "\\";

            if (!Directory.Exists(imagePath) && saveIncrements > 0)
                _ = Directory.CreateDirectory(imagePath);

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
                if (saveIncrements > 0 && t != -1 && t % saveIncrements == 0)
                {
                    fileName = imagePath + imageName + " - " + t + ".png";
                    previousImage.Save(fileName, ImageFormat.Png);
                }
                for (int i = 1; i < previousImage.Width - 1; i++)
                {
                    for (int j = 1; j < previousImage.Height - 1; j++)
                    {
                        // Format: U R D L UR DR DL UL for pixelRed, pixelGreen, pixelBlue + Original Center for original image
                        UtilityMethods.GetRGBValues(pixelRed, pixelGreen, pixelBlue, previousImage, originalImage, i, j);

                        if (paletteChoice == "Color") { 
                            utRed   = ShockFilterCalculation(pixelRed, delta);
                            utGreen = ShockFilterCalculation(pixelGreen, delta);
                            utBlue  = ShockFilterCalculation(pixelBlue, delta);
                        } 
                        else {
                            utRed = ShockFilterCalculation(pixelRed, delta);
                            utGreen = utRed;
                            utBlue = utRed;
                        }

                        augmentedImage.SetPixel(i, j, Color.FromArgb(utRed, utGreen, utBlue));
                    }
                }
                previousImage = (Bitmap)augmentedImage.Clone();
            }
            if (saveIncrements > 0)
                previousImage.Save(imagePath + imageName + " - " + loops + ".png", ImageFormat.Png);

            return augmentedImage;
        }

        // Level set equation
        // TODO add multithread calculations
        // Currently no/limited workaround, consult to find way to pass property
        private static int ShockFilterCalculation(IDictionary<string, int> pixel, double delta)
        {
            double ux, uy, uyy, uxx, top, value;
            int pixelValue;

            // Use Given Equations
            uyy = (double)(pixel["U"] - 2 * pixel["C"] + pixel["D"]);
            uxx = (double)(pixel["R"] - 2 * pixel["C"] + pixel["L"]);

            // If Cases
            int uxF, uxB, uyF, uyB;
            uxF = pixel["R"] - pixel["C"];
            uxB = pixel["C"] - pixel["L"];
            
            if (uxF * uxB <= 0) { ux = 0; }
            else if (uxF > 0) { ux = Math.Min(Math.Abs(uxF), Math.Abs(uxB)); }
            else { ux = - Math.Min(Math.Abs(uxF), Math.Abs(uxB)); }

            uyF = pixel["U"] - pixel["C"];
            uyB = pixel["C"] - pixel["D"];

            if (uyF * uyB <= 0) { uy = 0; }
            else if (uyF > 0) { uy = Math.Min(Math.Abs(uyF), Math.Abs(uyB)); }
            else { uy = -Math.Min(Math.Abs(uyF), Math.Abs(uyB)); }


            top = -Math.Sqrt((ux * ux) + (uy * uy)) * (uxx + uyy);

            value = (double)(top * delta + pixel["C"]);

            pixelValue = (int)Math.Round(value, 0);

            if (value < 0) return 0;
            if (value > 255) return 255;
            else return pixelValue;
        }
    }
}
