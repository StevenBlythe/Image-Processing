using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Image_Processing.PDE
{
    public static class UtilityMethods
    {
        public static void GetRGBValues(IDictionary<string, int> pixelRed, IDictionary<string, int> pixelGreen, IDictionary<string, int> pixelBlue, Bitmap previousImage, Bitmap originalImage, int i, int j)
        {
            // Center Pixel
            pixelRed["C"] = previousImage.GetPixel(i, j).R;
            pixelRed["OC"] = originalImage.GetPixel(i, j).R;
            // x-Axis
            pixelRed["L"] = previousImage.GetPixel(i - 1, j).R;
            pixelRed["R"] = previousImage.GetPixel(i + 1, j).R;

            // y-Axis
            pixelRed["D"] = previousImage.GetPixel(i, j - 1).R;
            pixelRed["U"] = previousImage.GetPixel(i, j + 1).R;

            // Corner UR UL DR DL
            pixelRed["UR"] = previousImage.GetPixel(i + 1, j + 1).R;
            pixelRed["UL"] = previousImage.GetPixel(i - 1, j + 1).R;
            pixelRed["DR"] = previousImage.GetPixel(i + 1, j - 1).R;
            pixelRed["DL"] = previousImage.GetPixel(i - 1, j - 1).R;

            // Center Pixel
            pixelGreen["C"] = previousImage.GetPixel(i, j).G;
            pixelGreen["OC"] = originalImage.GetPixel(i, j).G;
            // x-Axis
            pixelGreen["L"] = previousImage.GetPixel(i - 1, j).G;
            pixelGreen["R"] = previousImage.GetPixel(i + 1, j).G;

            // y-Axis
            pixelGreen["D"] = previousImage.GetPixel(i, j - 1).G;
            pixelGreen["U"] = previousImage.GetPixel(i, j + 1).G;

            // Corner UR UL DR DL
            pixelGreen["UR"] = previousImage.GetPixel(i + 1, j + 1).G;
            pixelGreen["UL"] = previousImage.GetPixel(i - 1, j + 1).G;
            pixelGreen["DR"] = previousImage.GetPixel(i + 1, j - 1).G;
            pixelGreen["DL"] = previousImage.GetPixel(i - 1, j - 1).G;

            // Center Pixel
            pixelBlue["C"] = previousImage.GetPixel(i, j).B;
            pixelBlue["OC"] = originalImage.GetPixel(i, j).B;
            // x-Axis
            pixelBlue["L"] = previousImage.GetPixel(i - 1, j).B;
            pixelBlue["R"] = previousImage.GetPixel(i + 1, j).B;

            // y-Axis
            pixelBlue["D"] = previousImage.GetPixel(i, j - 1).B;
            pixelBlue["U"] = previousImage.GetPixel(i, j + 1).B;

            // Corner UR UL DR DL
            pixelBlue["UR"] = previousImage.GetPixel(i + 1, j + 1).B;
            pixelBlue["UL"] = previousImage.GetPixel(i - 1, j + 1).B;
            pixelBlue["DR"] = previousImage.GetPixel(i + 1, j - 1).B;
            pixelBlue["DL"] = previousImage.GetPixel(i - 1, j - 1).B;
        }

        public static void SaveImage(string grayImagePreName, Bitmap grayImagePost)
        {
            // Displays a SaveFileDialog so the user can save the Image
            // assigned to Button2.
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "PNG Image|*.png|JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.FileName = grayImagePreName + "_grayscale";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.
                System.IO.FileStream fs =
                    (System.IO.FileStream)saveFileDialog1.OpenFile();
                // Saves the Image in the appropriate ImageFormat based upon the
                // File type selected in the dialog box.
                // NOTE that the FilterIndex property is one-based.
                switch (saveFileDialog1.FilterIndex)
                {
                    case 1:
                        grayImagePost.Save(fs,
                          System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;

                    case 2:
                        grayImagePost.Save(fs,
                          System.Drawing.Imaging.ImageFormat.Bmp);
                        break;

                    case 3:
                        grayImagePost.Save(fs,
                          System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                    case 4:
                        grayImagePost.Save(fs,
                          System.Drawing.Imaging.ImageFormat.Png);
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
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
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
    }
}
