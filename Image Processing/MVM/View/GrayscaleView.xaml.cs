using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Image_Processing.PDE;

namespace Image_Processing.MVM.View
{
    /// <summary>
    /// Interaction logic for GrayscaleView.xaml
    /// </summary>
    public partial class GrayscaleView : System.Windows.Controls.UserControl
    {
        private Bitmap grayImagePre;
        private string grayImagePreName;
        private Bitmap grayImagePost;

        public GrayscaleView()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            // Speed Benchmark
            var watch = System.Diagnostics.Stopwatch.StartNew();

            string selectedFile = UtilityMethods.UserSelectFile();

            // Check Image
            Bitmap img = new Bitmap(selectedFile);
            string _text = img.PixelFormat.ToString();

            // Update Image
            UtilityMethods.UpdateSourceImage(selectedFile, gray_original);

            // Obtain Name of Image
            // Process to Gray
            // Update Gray Image
            await ProcessColorToGrayImageAsync(selectedFile);


            watch.Stop();

        }

        private async Task ProcessColorToGrayImageAsync(string selectedFile)
        {
            // Process the image in Bitmap
            grayImagePre = new Bitmap(selectedFile); // Organize name

            // Provided images work in an 8bpp format, SetPixel does not work with this format.
            // Convert to appropriate format.
            // TODO, send to utility.
            if (grayImagePre.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                // Create a blank bitmap with the same dimensions
                Bitmap tempBitmap = new Bitmap(grayImagePre.Width, grayImagePre.Height);

                // From this bitmap, the graphics can be obtained, because it has the right PixelFormat
                using (Graphics g = Graphics.FromImage(tempBitmap))
                {
                    // Draw the original bitmap onto the graphics of the new bitmap
                    g.DrawImage(grayImagePre, 0, 0);
                }
                grayImagePre = tempBitmap;
            }
            grayImagePreName = Path.GetFileNameWithoutExtension(selectedFile); // Obtain name of image
            grayImagePost = await Task.Run(() => UtilityMethods.ProcessImage(grayImagePre)); // Process Color to Gray

            // Convert image to BitmapImages for <Image/> compatability
            gray_post.Source = UtilityMethods.BitmapToImageSource(grayImagePost); // Update Gray Image

            //PDEFormulas.GetBitmapColorMatrix(grayImagePost);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (grayImagePre != null)
            {
                UtilityMethods.SaveImage(grayImagePreName, grayImagePost);
            }
        }
    }
}
