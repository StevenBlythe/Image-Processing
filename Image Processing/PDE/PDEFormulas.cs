using System;
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
            // Obtain original image
            Bitmap previousImage = (Bitmap)originalImage.Clone();

            // Image to manipulate
            Bitmap saveImage = (Bitmap)originalImage.Clone();

            // Directory, File Name //
            imagePath += "Heat Equation delta " + delta + "\\";
            string pathFile = imagePath + imageName + " - ";
            if (!Directory.Exists(imagePath))
                _ = Directory.CreateDirectory(imagePath);

            // Colors, Pixels, Dictionaries for Calculations
            double utRed;
            double utGreen;
            double utBlue;

            double[,,] palette = new double[originalImage.Width, originalImage.Height, 3];
            palette = UtilityMethods.GetFullRGBValues(palette, originalImage);
            double[,,] paletteAugmented = new double[originalImage.Width, originalImage.Height, 3];
            paletteAugmented = UtilityMethods.GetFullRGBValues(paletteAugmented, originalImage);

            IDictionary<string, double> pixelRed = new Dictionary<string, double>();
            IDictionary<string, double> pixelGreen = new Dictionary<string, double>();
            IDictionary<string, double> pixelBlue = new Dictionary<string, double>();

            for (int t = 0; t < loops; t++)
            {
                if (saveIncrements > 0 && t != -1 && t % saveIncrements == 0)
                    UtilityMethods.SaveImageInterval(saveImage, palette, pathFile, t);
                for (int i = 1; i < previousImage.Width - 1; i++)
                {
                    for (int j = 1; j < previousImage.Height - 1; j++)
                    {
                        UtilityMethods.GetRGBValues(pixelRed, pixelGreen, pixelBlue, palette, originalImage, i, j);

                        // Support methods, 
                        if (paletteChoice == "Color")
                        {
                            utRed   = HeatEquationCalculation(pixelRed, delta);
                            utGreen = HeatEquationCalculation(pixelGreen, delta);
                            utBlue  = HeatEquationCalculation(pixelBlue, delta);
                            paletteAugmented[i, j, 0] = utRed;
                            paletteAugmented[i, j, 1] = utGreen;
                            paletteAugmented[i, j, 2] = utBlue;
                        }
                        else
                        {
                            utRed = HeatEquationCalculation(pixelRed, delta);
                            paletteAugmented[i, j, 0] = utRed;
                            paletteAugmented[i, j, 1] = utRed;
                            paletteAugmented[i, j, 2] = utRed;
                        }
                    }
                }
                // Swap arrays, pixelAugmentedValues becomes u^(n + 1), swap in O(1) time
                (palette, paletteAugmented) = (paletteAugmented, palette);
            }

            UtilityMethods.SaveImageInterval(saveImage, palette, pathFile, loops);
            return saveImage;
        }

        private static double HeatEquationCalculation(IDictionary<string, double> pixel, double delta)
        {
            return (pixel["R"] + pixel["L"] - 4 * pixel["C"] + pixel["U"] + pixel["D"]) * delta + pixel["C"];
        }

        public static Bitmap LevelSet(Bitmap originalImage, int loops, double delta, string paletteChoice, int saveIncrements, string imageName, string imagePath)
        {
            // Obtain original image
            Bitmap previousImage = (Bitmap)originalImage.Clone();

            // Image to manipulate
            Bitmap saveImage = (Bitmap)originalImage.Clone();

            // Directory, File Name //
            imagePath += "Level Set delta " + delta + "\\";
            string pathFile = imagePath + imageName + " - ";
            if (!Directory.Exists(imagePath))
                _ = Directory.CreateDirectory(imagePath);

            // Colors, Pixels, Dictionaries for Calculations
            double utRed;
            double utGreen;
            double utBlue;

            double[,,] palette = new double[originalImage.Width, originalImage.Height, 3];
            palette = UtilityMethods.GetFullRGBValues(palette, originalImage);
            double[,,] paletteAugmented = new double[originalImage.Width, originalImage.Height, 3];
            paletteAugmented = UtilityMethods.GetFullRGBValues(paletteAugmented, originalImage);

            IDictionary<string, double> pixelRed = new Dictionary<string, double>();
            IDictionary<string, double> pixelGreen = new Dictionary<string, double>();
            IDictionary<string, double> pixelBlue = new Dictionary<string, double>();

            for (int t = 0; t < loops; t++)
            {
                if (saveIncrements > 0 && t != -1 && t % saveIncrements == 0)
                    UtilityMethods.SaveImageInterval(saveImage, palette, pathFile, t);
                for (int i = 1; i < previousImage.Width - 1; i++)
                {
                    for (int j = 1; j < previousImage.Height - 1; j++) 
                    {
                        UtilityMethods.GetRGBValues(pixelRed, pixelGreen, pixelBlue, palette, originalImage, i, j);

                        // Support methods, 
                        if (paletteChoice == "Color")
                        {
                            utRed = LevelSetCalculation(pixelRed, delta);
                            utGreen = LevelSetCalculation(pixelGreen, delta);
                            utBlue = LevelSetCalculation(pixelBlue, delta);
                            paletteAugmented[i, j, 0] = utRed;
                            paletteAugmented[i, j, 1] = utGreen;
                            paletteAugmented[i, j, 2] = utBlue;
                        }
                        else
                        {
                            utRed = LevelSetCalculation(pixelRed, delta);
                            paletteAugmented[i, j, 0] = utRed;
                            paletteAugmented[i, j, 1] = utRed;
                            paletteAugmented[i, j, 2] = utRed;
                        }
                    }
                }
                // Swap arrays, pixelAugmentedValues becomes u^(n + 1), swap in O(1) time
                (palette, paletteAugmented) = (paletteAugmented, palette);
            }

            UtilityMethods.SaveImageInterval(saveImage, palette, pathFile, loops);
            return saveImage;
        }

        // Level set equation
        // TODO add multithread calculations
        // Currently no/limited workaround, consult to find way to pass property
        private static double LevelSetCalculation(IDictionary<string, double> pixel, double delta)
        {
            double ux, uy, uxy, uyy, uxx, top, bottom, pixelValue;

            ux  = (double)(pixel["R"] - pixel["L"]) / 2;
            uy  = (double)(pixel["U"] - pixel["D"]) / 2;
            uxy = (double)(pixel["UR"] - pixel["UL"] - pixel["DR"] + pixel["DL"])/4;
            uyy = (double)(pixel["U"] - 2 * pixel["C"] + pixel["D"]);
            uxx = (double)(pixel["R"] - 2 * pixel["C"] + pixel["L"]);

            top = (ux * ux * uyy) - (2 * ux * uy * uxy) + (uy * uy * uxx);
            bottom = (ux * ux) + (uy * uy) + 0.0000001;

            pixelValue = (top / bottom)*delta + pixel["C"]; // top/bottom * delta time + u^n_j,k

            //pixelValue = (int)Math.Round(value, 0);

            if (pixelValue < 0) return 0;
            if (pixelValue > 255) return 255;
            else return pixelValue;
        }

        // Modified Level Set
        public static Bitmap ModifiedLevelSet(Bitmap originalImage, int loops, double alpha, double delta, string paletteChoice, int saveIncrements, string imageName, string imagePath)
        {
            // Obtain original image
            Bitmap previousImage = (Bitmap)originalImage.Clone();

            // Image to manipulate
            Bitmap saveImage = (Bitmap)originalImage.Clone();

            // Directory, File Name //
            imagePath += "Modified Level Set delta " + delta + " alpha " + alpha + "\\";
            string pathFile = imagePath + imageName + " - ";
            if (!Directory.Exists(imagePath))
                _ = Directory.CreateDirectory(imagePath);

            // Colors, Pixels, Dictionaries for Calculations
            double utRed;
            double utGreen;
            double utBlue;

            double[,,] palette = new double[originalImage.Width, originalImage.Height, 3];
            palette = UtilityMethods.GetFullRGBValues(palette, originalImage);
            double[,,] paletteAugmented = new double[originalImage.Width, originalImage.Height, 3];
            paletteAugmented = UtilityMethods.GetFullRGBValues(paletteAugmented, originalImage);

            IDictionary<string, double> pixelRed = new Dictionary<string, double>();
            IDictionary<string, double> pixelGreen = new Dictionary<string, double>();
            IDictionary<string, double> pixelBlue = new Dictionary<string, double>();

            for (int t = 0; t < loops; t++)
            {
                if (saveIncrements > 0 && t != -1 && t % saveIncrements == 0)
                    UtilityMethods.SaveImageInterval(saveImage, palette, pathFile, t);
                for (int i = 1; i < previousImage.Width - 1; i++)
                {
                    for (int j = 1; j < previousImage.Height - 1; j++)
                    {
                        UtilityMethods.GetRGBValues(pixelRed, pixelGreen, pixelBlue, palette, originalImage, i, j);

                        // Support methods, 
                        if (paletteChoice == "Color")
                        {
                            utRed = ModifiedLevelSetCalculation(pixelRed, delta, alpha);
                            utGreen = ModifiedLevelSetCalculation(pixelGreen, delta, alpha);
                            utBlue = ModifiedLevelSetCalculation(pixelBlue, delta, alpha);
                            paletteAugmented[i, j, 0] = utRed;
                            paletteAugmented[i, j, 1] = utGreen;
                            paletteAugmented[i, j, 2] = utBlue;
                        }
                        else
                        {
                            utRed = ModifiedLevelSetCalculation(pixelRed, delta, alpha);
                            paletteAugmented[i, j, 0] = utRed;
                            paletteAugmented[i, j, 1] = utRed;
                            paletteAugmented[i, j, 2] = utRed;
                        }
                    }
                }
                // Swap arrays, pixelAugmentedValues becomes u^(n + 1), swap in O(1) time
                (palette, paletteAugmented) = (paletteAugmented, palette);
            }

            UtilityMethods.SaveImageInterval(saveImage, palette, pathFile, loops);
            return saveImage;
        }

        private static double ModifiedLevelSetCalculation(IDictionary<string, double> pixel, double delta, double alpha)
        {
            double modified = alpha * (pixel["C"] - pixel["OC"]);
            double ux, uy, uxy, uyy, uxx, top, bottom, pixelValue;

            ux = (double)(pixel["R"] - pixel["L"]) / 2;
            uy = (double)(pixel["U"] - pixel["D"]) / 2;
            uxy = (double)(pixel["UR"] - pixel["UL"] - pixel["DR"] + pixel["DL"]) / 4;
            uyy = (double)(pixel["U"] - 2 * pixel["C"] + pixel["D"]);
            uxx = (double)(pixel["R"] - 2 * pixel["C"] + pixel["L"]);

            top = (ux * ux * uyy) - (2 * ux * uy * uxy) + (uy * uy * uxx);
            bottom = (ux * ux) + (uy * uy) + 0.0000001;

            pixelValue = (top / bottom - modified) * delta + pixel["C"]; // top/bottom * delta time + u^n_j,k

            if (pixelValue < 0) return 0;
            if (pixelValue > 255) return 255;
            else return pixelValue;
        }


        // Shock Filter
        // Level Set
        public static Bitmap ShockFilter(Bitmap originalImage, int loops, double delta, string paletteChoice, int saveIncrements, string imageName, string imagePath)
        {
            // Obtain original image
            Bitmap previousImage = (Bitmap)originalImage.Clone();

            // Image to manipulate
            Bitmap saveImage = (Bitmap)originalImage.Clone();

            // Directory, File Name //
            imagePath += "Shock Filter delta " + delta + "\\";
            string pathFile = imagePath + imageName + " - ";
            if (!Directory.Exists(imagePath))
                _ = Directory.CreateDirectory(imagePath);

            // Colors, Pixels, Dictionaries for Calculations
            double utRed;
            double utGreen;
            double utBlue;

            double[,,] palette = new double[originalImage.Width, originalImage.Height, 3];
            palette = UtilityMethods.GetFullRGBValues(palette, originalImage);
            double[,,] paletteAugmented = new double[originalImage.Width, originalImage.Height, 3];
            paletteAugmented = UtilityMethods.GetFullRGBValues(paletteAugmented, originalImage);

            IDictionary<string, double> pixelRed = new Dictionary<string, double>();
            IDictionary<string, double> pixelGreen = new Dictionary<string, double>();
            IDictionary<string, double> pixelBlue = new Dictionary<string, double>();

            for (int t = 0; t < loops; t++)
            {
                if (saveIncrements > 0 && t != -1 && t % saveIncrements == 0)
                    UtilityMethods.SaveImageInterval(saveImage, palette, pathFile, t);
                for (int i = 1; i < previousImage.Width - 1; i++)
                {
                    for (int j = 1; j < previousImage.Height - 1; j++)
                    {
                        UtilityMethods.GetRGBValues(pixelRed, pixelGreen, pixelBlue, palette, originalImage, i, j);

                        // Support methods, 
                        if (paletteChoice == "Color")
                        {
                            utRed   = ShockFilterCalculation(pixelRed, delta);
                            utGreen = ShockFilterCalculation(pixelGreen, delta);
                            utBlue  = ShockFilterCalculation(pixelBlue, delta);
                            paletteAugmented[i, j, 0] = utRed;
                            paletteAugmented[i, j, 1] = utGreen;
                            paletteAugmented[i, j, 2] = utBlue;
                        }
                        else
                        {
                            utRed = ShockFilterCalculation(pixelRed, delta);
                            paletteAugmented[i, j, 0] = utRed;
                            paletteAugmented[i, j, 1] = utRed;
                            paletteAugmented[i, j, 2] = utRed;
                        }
                    }
                }
                // Swap arrays, pixelAugmentedValues becomes u^(n + 1), swap in O(1) time
                (palette, paletteAugmented) = (paletteAugmented, palette);
            }

            UtilityMethods.SaveImageInterval(saveImage, palette, pathFile, loops);
            return saveImage;
        }

        private static double ShockFilterCalculation(IDictionary<string, double> pixel, double delta)
        {
            double ux, uy, uyy, uxx, top, value;

            // Use Given Equations
            uyy = (double)(pixel["U"] - 2 * pixel["C"] + pixel["D"]);
            uxx = (double)(pixel["R"] - 2 * pixel["C"] + pixel["L"]);

            // If Cases
            double uxF, uxB, uyF, uyB;
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

            value = (top * delta + pixel["C"]);

            if (value < 0) return 0;
            if (value > 255) return 255;
            else return value;
        }
    }
}
