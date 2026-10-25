using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace gaussian_blur_x86_64_asm
{
    abstract internal class GaussianBlur
    {
        protected int _radius;
        protected byte _threadsCount;
        protected readonly Bitmap _image;
        protected readonly double[,] _kernel;

        public GaussianBlur(int radius, ref Bitmap image, byte threadsCount)
        {
            _radius = radius;
            _image = image;
            _kernel = GenerateKernel(radius);
            _threadsCount = threadsCount;
        }

        protected void PrintKernel()
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

        protected abstract double[,] GenerateKernel(int radius);

        protected abstract void GetBlurredPixel(byte[] src, int width, int height, int stride, int x, int y,
            out byte outB, out byte outG, out byte outR, out byte outA);

        public abstract Bitmap GetBlurred();
    }
}
