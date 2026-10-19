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
using System.Diagnostics;

namespace gaussian_blur_x86_64_asm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _readyForRequest = false;
        private readonly ImageBlurrer _blurrer;

        public MainWindow()
        {
            InitializeComponent();
            _blurrer = new ImageBlurrer();
            _readyForRequest = true;
        }

        private async void Image_Select_Click(object sender, RoutedEventArgs e)
        {
            if(_readyForRequest == false) return;
            _readyForRequest = false;
            statusText.Text = "Loading Image...";
            bool loaded = _blurrer.loadSourceImage();
            
            if(loaded)
            {
                statusText.Text = "Image loaded";
                if (selectImageButton.Template.FindName("image", selectImageButton) is not System.Windows.Controls.Image imageControl) return;

                imageControl.Source = _blurrer.Source;
            } else
            {
                statusText.Text = "Failed to load image";
            }

            await Task.Delay(1000);
            _readyForRequest = true;
            statusText.Text = "Ready";
        }

        private async void Show_Original_Mouse_Down(object sender, MouseButtonEventArgs e)
        {
            if (_readyForRequest == false) return;
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (_blurrer?.Source == null) return;
            statusText.Text = "Showing Original Image...";
            if (selectImageButton.Template.FindName("image", selectImageButton) is not System.Windows.Controls.Image imageControl) return;
            imageControl.Source = _blurrer.Source;


            // Catching releases outside the button
            if(sender is UIElement element)
            {
                Mouse.Capture(element);
            }


            
        }
        private async void Show_Original_Mouse_Up(object sender, RoutedEventArgs e)
        {
            if (_readyForRequest == false) return;
            if (sender is UIElement element && Mouse.Captured == element)
            {
                Mouse.Capture(null);
            }
            
            if (_blurrer.Blurred != null)
            {
                if (selectImageButton.Template.FindName("image", selectImageButton) is not System.Windows.Controls.Image imageControl) return;
                imageControl.Source = _blurrer.Blurred;
            }
        }  

        private async void Run_Gaussian_Click(object sender, RoutedEventArgs e)
        {
            executionTimeText.Text = "Last Run Execution Time: -- ms";
            if (_readyForRequest == false) return;
            statusText.Text = "Running Gaussian Blur...";
            _readyForRequest = false;
            if(_blurrer.IsSourceLoaded() == false)
            {
                statusText.Text = "No image loaded";
                _readyForRequest = true;
                return;
            }

            Stopwatch blurMeasureStopwatch = Stopwatch.StartNew(); 
            
            //Checking if the value is of radio button is true and if it's value is not null
            //I've added the comment because it looks ugly for me
            if (radioASM.IsChecked.HasValue && radioASM.IsChecked.Value)
            {
                await _blurrer.ApplyWithAsm((int)radiusSlider.Value);
            }
            else if (radioCSharp.IsChecked.HasValue && radioCSharp.IsChecked.Value)
            {
                await _blurrer.ApplyWithCSharp((int)radiusSlider.Value);
            }
            blurMeasureStopwatch.Stop(); 

            if (selectImageButton.Template.FindName("image", selectImageButton) is not System.Windows.Controls.Image imageControl) return;

            imageControl.Source = _blurrer.Blurred;
        
            executionTimeText.Text = $"Last Run Execution Time: {blurMeasureStopwatch.ElapsedMilliseconds} ms";
            _readyForRequest = true;
            statusText.Text = "Ready";
            
        }

        private async void Save_Image_Click(object sender, RoutedEventArgs e)
        {
            if (_readyForRequest == false) return;
            _readyForRequest = false;
            statusText.Text = "Saving Image";
            if (_blurrer.Blurred != null)
            {
                try
                {
                    _blurrer.SaveBlurredImage();

                }
                catch (Exception ex)
                {
                    statusText.Text = $"Error saving image: {ex.Message}";
                }
                statusText.Text = "Image Saved";

            }
            else
            {
                statusText.Text = "No blurred image to save";
                await Task.Delay(1000);
            }

            _readyForRequest = true;
            await Task.Delay(1000);
            statusText.Text = "Ready";
        }
    }
}