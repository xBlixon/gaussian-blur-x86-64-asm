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
        private bool ready = false;
        ImageBlurrer _blurrer;

        public MainWindow()
        {
            InitializeComponent();
            _blurrer = new ImageBlurrer();
            ready = true;
        }

        private async void Image_Select_Click(object sender, RoutedEventArgs e)
        {
            if(ready == false) return;
            ready = false;
            statusText.Text = "Loading Image...";
            bool loaded = _blurrer.loadSourceImage();
            
            if(loaded)
            {
                statusText.Text = "Image loaded";
                var imageControl = selectImageButton.Template.FindName("image", selectImageButton) as System.Windows.Controls.Image;

                if (imageControl == null) return;
                
                imageControl.Source = _blurrer.Source;
            } else
            {
                statusText.Text = "Failed to load image";
            }

            await Task.Delay(1000);
            ready = true;
            statusText.Text = "Ready";
        }

        private async void Show_Original_Mouse_Down(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (_blurrer?.Source == null) return;
            statusText.Text = "Showing Original Image...";
            var imageControl = selectImageButton.Template.FindName("image", selectImageButton) as System.Windows.Controls.Image;
            imageControl.Source = _blurrer.Source;


            // Catching releases outside the button
            if(sender is UIElement element)
            {
                Mouse.Capture(element);
            }


            
        }
        private async void Show_Original_Mouse_Up(object sender, RoutedEventArgs e)
        {
            if (sender is UIElement element && Mouse.Captured == element)
            {
                Mouse.Capture(null);
            }
            
            if (_blurrer.Blurred != null)
            {
                var imageControl = selectImageButton.Template.FindName("image", selectImageButton) as System.Windows.Controls.Image;
                imageControl.Source = _blurrer.Blurred;
            }
        }  

        private async void Run_Gaussian_Click(object sender, RoutedEventArgs e)
        {
            if (ready == false) return;
            statusText.Text = "Running Gaussian Blur...";
            ready = false;
            await _blurrer.ApplyWithCSharp((int)radiusSlider.Value);

            var imageControl = selectImageButton.Template.FindName("image", selectImageButton) as System.Windows.Controls.Image;
            if (imageControl == null) return;

            imageControl.Source = _blurrer.Blurred;
        
            ready = true;
            statusText.Text = "Ready";
            
        }

        private async void Save_Image_Click(object sender, RoutedEventArgs e)
        {
            if (ready == false) return;
            ready = false;
            //TODO: Implement image saving logic here
            statusText.Text = "Image Saved";
            await Task.Delay(1000);
            ready = true;
            statusText.Text = "Ready";
        }
    }
}