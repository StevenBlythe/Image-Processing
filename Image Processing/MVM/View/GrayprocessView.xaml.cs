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
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Allow users to open an image to convert to grayscale
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Image Files(*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                // <Image/> in XAML takes BitmapImages
                gray_original.Source = new BitmapImage(new Uri(openFile.FileName));

                // Process the image in Bitmap
                grayImagePre = new Bitmap(openFile.FileName);
                grayImagePreName = Path.GetFileNameWithoutExtension(openFile.FileName);
                grayImagePost = PDEFormulas.ProcessImage(grayImagePre);

                // Convert image to BitmapImages for <Image/> compatability
                gray_post.Source = PDEFormulas.BitmapToImageSource(grayImagePost);

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (grayImagePre != null)
            {
                PDEFormulas.SaveImage(grayImagePreName, grayImagePost);
            }
        }

    }
}
