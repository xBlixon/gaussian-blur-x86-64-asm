using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace gaussian_blur_x86_64_asm
{
    internal class ImageBlurrer
    {
        private Bitmap? _source;
        private Bitmap? _blurred;

        public ImageSource? Source
        {
            get { return _source != null ? ToBitmapImage(_source) : null; }
        }

        public ImageSource? Blurred
        {
            get { return _blurred != null ? ToBitmapImage(_blurred) : null; }
        }

        public bool loadSourceImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    _source = new Bitmap(openFileDialog.FileName);
                }
                catch (Exception)
                {
                    return false;
                }

                return true;
            }
            return false;
        }

        public bool IsSourceLoaded()
        {
            return _source != null;
        }

        public Task ApplyWithCSharp(int radius)
        {
            if (_source == null) return Task.CompletedTask;

                return Task.Run(() =>
            {
                var sourceCopy = _source;
                CSharpGaussianBlur blur = new CSharpGaussianBlur(radius, ref sourceCopy);
                _blurred = blur.GetBlurred(); 
            });
        }

        private BitmapImage? ToBitmapImage(Bitmap bitmap)
        {
            if (bitmap == null) return null;

            using var memory = new MemoryStream();
            bitmap.Save(memory, ImageFormat.Bmp);
            memory.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }

        internal void SaveBlurredImage()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    _blurred?.Save(saveFileDialog.FileName, ImageFormat.Bmp);
                }
                catch (Exception)
                {
                    throw new Exception("Failed to save image");
                }
            }
        }

        //TODO: Implement MASM solution
        internal async Task ApplyWithAsm(int value)
        {
            throw new NotImplementedException();
        }
    }
}
