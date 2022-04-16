using Image_Processing.PDE;
using System.Drawing;
using System.Windows.Controls;
using System.Drawing.Imaging;
using System.Windows.Input;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Image_Processing.MVM.View
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        private Bitmap ImagePre;
        private string ImagePreName;
        private Bitmap ImagePost;

        public HomeView()
        {
            InitializeComponent();
        }

        private void window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
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
            UtilityMethods.UpdateSourceImage(selectedFile, original_image);

            await ProcessImageAsync(selectedFile); // Selected File assumed to be gray

            watch.Stop();
        }

        private async Task ProcessImageAsync(string selectedFile)
        {
            // Process the image in Bitmap
            ImagePre = new Bitmap(selectedFile); // Organize name
            ImagePreName = Path.GetFileNameWithoutExtension(selectedFile); // Obtain name of image

            // Checks for format
            if (ImagePre.PixelFormat == PixelFormat.Format8bppIndexed)
                ImagePre = UtilityMethods.ConvertIndexedToNonIndexed(ImagePre);

            Bitmap TempImg = (Bitmap)ImagePre.Clone();

            // Create Progress Bar
            BackgroundWorker worker = new BackgroundWorker();

            int time = Int32.Parse(user_time.Text);

            ComboBoxItem cbi = (ComboBoxItem)process.SelectedItem;
            string selectedMethod = cbi.Content.ToString();

            if (selectedMethod == "Heat Equation")
                ImagePost = await Task.Run(() => PDEFormulas.HeatEquation(TempImg, time));
            else if (selectedMethod == "Level Set")
                ImagePost = await Task.Run(() => PDEFormulas.LevelSet(TempImg, time));
            else if (selectedMethod == "Modified Level Set")
                ImagePost = await Task.Run(() => PDEFormulas.ModifiedLevelSet(TempImg, time));
            else if (selectedMethod == "Shock Filter")
                ImagePost = await Task.Run(() => PDEFormulas.ShockFilter(TempImg, time));
            else
                return;

            // Convert image to BitmapImages for <Image/> compatability
            post.Source = UtilityMethods.BitmapToImageSource(ImagePost); // Update Gray Image

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (ImagePre != null)
            {
                ComboBoxItem cbi = (ComboBoxItem)process.SelectedItem;
                string selectedMethod = cbi.Content.ToString();
                UtilityMethods.SaveImage(ImagePreName + " - " + selectedMethod, ImagePost);
            }
        }
    }
}
