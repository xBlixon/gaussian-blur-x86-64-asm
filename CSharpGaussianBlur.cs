using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;


namespace gaussian_blur_x86_64_asm
{
    internal class CSharpGaussianBlur
    {
        private int _radius;
        private readonly Bitmap _image;

        private readonly double[,] _kernel;

        public CSharpGaussianBlur(int radius, ref Bitmap image)
        {
            _radius = radius;
            _image = image;
            _kernel = generateKernel(radius);
            printKernel();
        }

        public Bitmap GetBlurred()
        {
            // New image to be blurred
            Bitmap bmp = new Bitmap(_image.Width, _image.Height);
            for (int X=0; X<bmp.Width; X++)
            {
                for(int Y=0; Y<bmp.Height; Y++)
                {
                    bmp.SetPixel(X, Y, GetBlurredPixel(X, Y));
                }
            }
            return bmp;
        }

        private Color GetBlurredPixel(int X, int Y)
        {
            double A = 0;
            double R = 0;
            double G = 0;
            double B = 0;
            for (int rX = -_radius; rX <= _radius; rX++)
            {
                for (int rY = -_radius; rY <= _radius; rY++)
                {
                    Color pxl = _image.GetPixel(Math.Clamp(X + rX, 0, _image.Width-1), Math.Clamp(Y + rY, 0, _image.Height-1));
                    var scale = _kernel[_radius + rX, _radius + rY];
                    A += pxl.A * scale;
                    R += pxl.R * scale;
                    G += pxl.G * scale;
                    B += pxl.B * scale;

                }
            }
            return Color.FromArgb((int)Math.Round(A), (int)Math.Round(R), (int)Math.Round(G), (int)Math.Round(B));
        }

        private double[,] generateKernel(int radius)
        {
            int side = radius * 2 + 1;
            double[,] kernel = new double[side, side];
            double sigma = radius / 2.0; // Standard deviation for Gaussian function
            double twoSigmaSquare = 2.0 * sigma * sigma;

            //TODO: add parallel execution to those loops
            for (int row=0; row<side; row++)
            {
                for(int col=0; col<side; col++)
                {
                    // Calculating weights
                    int radiusX = (radius - row);
                    int radiusY = (radius - col);
                    kernel[row, col] = Math.Exp(-(radiusX * radiusX + radiusY * radiusY) / twoSigmaSquare);
                }
            }

            double sum = 0.0;
            foreach (var value in kernel)
            {
                sum += value;
            }

            // Kernel normalization
            //TODO: add parallel execution to those loops
            for (int row = 0; row < side; row++)
            {
                for (int col = 0; col < side; col++)
                {
                    kernel[row, col] /= sum;
                }
            }

            return kernel;
        }

        private void printKernel()
        {
            int side = _kernel.GetLength(0);
            for (int row = 0; row < side; row++)
            {
                for (int col = 0; col < side; col++)
                {
                    Console.Write(_kernel[row, col] + "\t");
                }
                Console.WriteLine();
            }
        }
    }
}
