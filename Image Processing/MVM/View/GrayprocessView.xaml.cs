using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Image_Processing.PDE;


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
            // Select File
            string selectedFile = UtilityMethods.UserSelectFile();

            if (selectedFile == null)
                return;

            // Speed Benchmark
            var watch = System.Diagnostics.Stopwatch.StartNew();

            // Update Image
            UtilityMethods.UpdateSourceImage(selectedFile, gray_original);

            // Obtain Name of Image
            // Process to Gray
            ComboBoxItem cbi = (ComboBoxItem)gray_process.SelectedItem;
            string selectedMethod = cbi.Content.ToString();

            if (selectedMethod == "Heat Equation")
                await ProcessHeatEquationAsync(selectedFile); // Selected File assumed to be gray
            else if (selectedMethod == "Level Set")
                return;
            else if (selectedMethod == "Modified Level Set")
                return;
            else if (selectedMethod == "Shock")
                return;
            else
                return;

            watch.Stop();
        }




        private async Task ProcessHeatEquationAsync(string selectedFile)
        {
            // Process the image in Bitmap
            grayImagePre = new Bitmap(selectedFile); // Organize name
            grayImagePreName = Path.GetFileNameWithoutExtension(selectedFile); // Obtain name of image

            // Checks for format
            if (grayImagePre.PixelFormat == PixelFormat.Format8bppIndexed)
                grayImagePre = UtilityMethods.ConvertIndexedToNonIndexed(grayImagePre);

            Bitmap grayTempImg = (Bitmap)grayImagePre.Clone();

            // Create Progress Bar
            BackgroundWorker worker = new BackgroundWorker();

            int time = Int32.Parse(time_gray.Text);
            grayImagePost = await Task.Run(() => PDEFormulas.HeatEquation(grayTempImg, time)); // Apply Heat Equation
            grayTempImg = (Bitmap)grayImagePre.Clone();
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
