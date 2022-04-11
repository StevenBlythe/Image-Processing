using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Image_Processing.PDE;
using System.IO;


namespace Image_Processing.MVM.View
{
    /// <summary>
    /// Interaction logic for GrayprocessView.xaml
    /// </summary>
    public partial class GrayprocessView : System.Windows.Controls.UserControl
    {
        private Bitmap grayImagePre;
        private string grayImagePreName;
        private Bitmap grayImagePost;

        public GrayprocessView()
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

            // Obtain Name of Image
            // Process to Gray
            // Update Gray Image
            await ProcessHeatEquationAsync(selectedFile); // Selected File assumed to be gray

            watch.Stop();

        }




        private async Task ProcessHeatEquationAsync(string selectedFile)
        {
            // Process the image in Bitmap
            grayImagePre = new Bitmap(selectedFile); // Organize name
            grayImagePreName = Path.GetFileNameWithoutExtension(selectedFile); // Obtain name of image
            grayImagePost = await Task.Run(() => PDEFormulas.HeatEquation(grayImagePre)); // Apply Heat Equation

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
