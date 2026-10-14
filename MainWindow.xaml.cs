using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading.Tasks;

namespace gaussian_blur_x86_64_asm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            
            InitializeComponent();
        }

        private async void Image_Select_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Implement image selection logic here
            statusText.Text = "PhotoClicked";
            await Task.Delay(1000);
            statusText.Text = "Ready";
        }

        private async void Run_Gaussian_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Implement Gaussian Blur call here
            statusText.Text = "Running Gaussian Blur...";
            await Task.Delay(1000);
            statusText.Text = "Ready";
            
        }

        private async void Save_Image_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Implement image saving logic here
            statusText.Text = "Image Saved";
            await Task.Delay(1000);
            statusText.Text = "Ready";
        }
    }
}