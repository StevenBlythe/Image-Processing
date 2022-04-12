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

            // Update Image
            UtilityMethods.UpdateSourceImage(selectedFile, gray_original);

            // Obtain Name of Image | Process to Gray | Update Gray Image
            await ProcessColorToGrayImageAsync(selectedFile);


            watch.Stop();

        }

        private async Task ProcessColorToGrayImageAsync(string selectedFile)
        {
            grayImagePre = new Bitmap(selectedFile); // Organize name

            
            if (grayImagePre.PixelFormat == PixelFormat.Format8bppIndexed)                   // Provided images work in an 8bpp format, SetPixel does not work with this format.
                grayImagePre = UtilityMethods.ConvertIndexedToNonIndexed(grayImagePre);      // Convert to appropriate format.

            grayImagePreName = Path.GetFileNameWithoutExtension(selectedFile);               // Obtain name of image
            grayImagePost = await Task.Run(() => UtilityMethods.ProcessImage(grayImagePre)); // Process Color to Gray

            
            gray_post.Source = UtilityMethods.BitmapToImageSource(grayImagePost);            // Convert image to BitmapImages for <Image/> compatability, update Post image
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (grayImagePre != null)
                UtilityMethods.SaveImage(grayImagePreName, grayImagePost);
        }
    }
}
