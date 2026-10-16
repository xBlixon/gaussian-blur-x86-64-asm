using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace gaussian_blur_x86_64_asm
{
    internal class ImageBlurrer
    {
        private Image? _source;
        private Image? _blurred;

        public ImageSource? Source
        {
            get { return _source != null ? ToImageSource(_source) : null; }
        }

        public ImageSource? Blurred
        {
            get { return _blurred != null ? ToImageSource(_blurred) : null; }
        }

        public bool loadSourceImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                _source = Image.FromFile(openFileDialog.FileName);
                return true;
            }
            return false;
        }

        public void ApplyWithCSharp()
        {
            // TODO: Implement Gaussian Blur
        }

        private ImageSource ToImageSource(Image image)
        {
            return ToBitmapImage(image);
        }

        private BitmapImage ToBitmapImage(Image image)
        {
            var memory = new MemoryStream();
            image.Save(memory, ImageFormat.Png);
            memory.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();

            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;

            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }

    }
}
