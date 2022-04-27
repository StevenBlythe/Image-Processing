using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Image_Processing.PDE
{
    public static class UtilityMethods
    {

        public static void GetRGBValues(IDictionary<string, double> pixelRed, IDictionary<string, double> pixelGreen, IDictionary<string, double> pixelBlue, double[,,] palette, Bitmap originalImage, int i, int j)
        {
            // Red //
            // Center Pixel
            pixelRed["C"] = palette[i, j, 0];
            pixelRed["OC"] = originalImage.GetPixel(i, j).R;
            // x-Axis
            pixelRed["L"] = palette[i - 1, j, 0];
            pixelRed["R"] = palette[i + 1, j, 0];

            // y-Axis
            pixelRed["D"] = palette[i, j + 1, 0];
            pixelRed["U"] = palette[i, j - 1, 0];

            // Corner UR UL DR DL
            pixelRed["UR"] = palette[i + 1, j - 1, 0];
            pixelRed["UL"] = palette[i - 1, j - 1, 0];
            pixelRed["DR"] = palette[i + 1, j + 1, 0];
            pixelRed["DL"] = palette[i - 1, j + 1, 0];

            // Green //
            // Center Pixel
            pixelGreen["C"] = palette[i, j, 1];
            pixelGreen["OC"] = originalImage.GetPixel(i, j).G;
            // x-Axis
            pixelGreen["L"] = palette[i - 1, j, 1];
            pixelGreen["R"] = palette[i + 1, j, 1];

            // y-Axis
            pixelGreen["D"] = palette[i, j + 1, 1];
            pixelGreen["U"] = palette[i, j - 1, 1];

            // Corner UR UL DR DL
            pixelGreen["UR"] = palette[i + 1, j - 1, 1];
            pixelGreen["UL"] = palette[i - 1, j - 1, 1];
            pixelGreen["DR"] = palette[i + 1, j + 1, 1];
            pixelGreen["DL"] = palette[i - 1, j + 1, 1];

            // Blue //
            // Center Pixel
            pixelBlue["C"] = palette[i, j, 2];
            pixelBlue["OC"] = originalImage.GetPixel(i, j).B;
            // x-Axis
            pixelBlue["L"] = palette[i - 1, j, 2];
            pixelBlue["R"] = palette[i + 1, j, 2];

            // y-Axis
            pixelBlue["D"] = palette[i, j + 1, 2];
            pixelBlue["U"] = palette[i, j - 1, 2];

            // Corner UR UL DR DL
            pixelBlue["UR"] = palette[i + 1, j - 1, 2];
            pixelBlue["UL"] = palette[i - 1, j - 1, 2];
            pixelBlue["DR"] = palette[i + 1, j + 1, 2];
            pixelBlue["DL"] = palette[i - 1, j + 1, 2];
        }

        public static void SaveImage(string grayImagePreName, Bitmap grayImagePost)
        {
            if (grayImagePost == null)
                return;

            // Displays a SaveFileDialog so the user can save the Image
            // assigned to Button2.
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "PNG Image|*.png|JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.FileName = grayImagePreName;
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "" )
            {
                // Saves the Image via a FileStream created by the OpenFile method.
                FileStream fs =
                    (FileStream)saveFileDialog1.OpenFile();
                // Saves the Image in the appropriate ImageFormat based upon the
                // File type selected in the dialog box.
                // NOTE that the FilterIndex property is one-based.
                switch (saveFileDialog1.FilterIndex)
                {
                    case 1:
                        grayImagePost.Save(fs,
                          ImageFormat.Jpeg);
                        break;

                    case 2:
                        grayImagePost.Save(fs,
                          ImageFormat.Bmp);
                        break;

                    case 3:
                        grayImagePost.Save(fs,
                          ImageFormat.Gif);
                        break;
                    case 4:
                        grayImagePost.Save(fs,
                          ImageFormat.Png);
                        break;
                }

                fs.Close();
            }
        }

        public static Bitmap ProcessImage(Bitmap bmp)
        {
                for (int i = 0; i < bmp.Width; i++)
                {
                    for (int j = 0; j < bmp.Height; j++)
                    {
                        Color bmpColor = bmp.GetPixel(i, j);
                        int red = bmpColor.R;
                        int green = bmpColor.G;
                        int blue = bmpColor.B;
                        // Maple's conversion formula
                        int gray = (byte)(.3 * red + .59 * green + .11 * blue);
                        red = gray;
                        green = gray;
                        blue = gray;

                        bmp.SetPixel(i, j, Color.FromArgb(red, green, blue));
                    }
                }
                return bmp;
        }

        internal static double[,,] GetFullRGBValues(double[,,] pixelValues, Bitmap originalImage)
        {
            Color pixel;
            for (int i = 0; i < originalImage.Width; i++)
            {
                for (int j = 0; j < originalImage.Height; j++)
                {
                    pixel = originalImage.GetPixel(i, j);
                    pixelValues[i, j, 0] = pixel.R;
                    pixelValues[i, j, 1] = pixel.G;
                    pixelValues[i, j, 2] = pixel.B;
                }
            }
            return pixelValues;
        }

        public static Bitmap ConvertIndexedToNonIndexed(Bitmap bmp)
        {
            Bitmap bp = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format24bppRgb);
            using (Graphics gr = Graphics.FromImage(bp))
                gr.DrawImage(bmp, new Rectangle(0, 0, bp.Width, bp.Height));
            return bp;
        }

        // Bitmap to image
        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }


        public static void UpdateSourceImage(string selectedFile, System.Windows.Controls.Image originalImage)
            => originalImage.Source = new BitmapImage(new Uri(selectedFile));

        public static string UserSelectFile()
        {
            // Allow users to open an image to convert to grayscale
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Image Files(*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                return openFile.FileName;
            }
            return null;
        }

        internal static void SaveImageInterval(Bitmap saveImage, double[,,] pixelValues, string pathFile, int loops)
        {
            string fileName = pathFile + loops + ".png";

            // pixelValues to bitmap
            for (int i = 0; i < pixelValues.GetLength(0); i++)
                for (int j = 0; j < pixelValues.GetLength(1); j++)
                    saveImage.SetPixel(i, j, Color.FromArgb(
                            (int)Math.Round(pixelValues[i, j, 0], 0), 
                            (int)Math.Round(pixelValues[i, j, 1], 0), 
                            (int)Math.Round(pixelValues[i, j, 2], 0)));

            // Save bitmap
            saveImage.Save(fileName, ImageFormat.Png);
        }
    }
}
