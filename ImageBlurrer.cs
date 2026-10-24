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
                    using (Bitmap loadedBitmap = new Bitmap(openFileDialog.FileName))
                    {
                        _source?.Dispose();
                        _source = new Bitmap(loadedBitmap.Width, loadedBitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                        using (Graphics g = Graphics.FromImage(_source))
                        {
                            g.DrawImage(loadedBitmap, new Rectangle(0, 0, _source.Width, _source.Height));
                        }
                    }
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

        public Task ApplyWithCSharp(int radius,byte threadsCount)
        {
            if (_source == null) return Task.CompletedTask;
            
                return Task.Run(() =>
            {
                _blurred?.Dispose();
                var sourceCopy = _source;
                CSharpGaussianBlur blur = new CSharpGaussianBlur(radius, ref sourceCopy, threadsCount);
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
