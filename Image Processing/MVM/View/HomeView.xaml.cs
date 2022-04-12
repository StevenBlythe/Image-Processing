using Image_Processing.PDE;
using System.Drawing;
using System.Windows.Controls;
using System.Drawing.Imaging;
using System.Windows.Input;

namespace Image_Processing.MVM.View
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
        }

        private void window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }
    }
}
