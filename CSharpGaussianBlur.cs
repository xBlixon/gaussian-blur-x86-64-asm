using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;


namespace gaussian_blur_x86_64_asm
{
    internal class CSharpGaussianBlur
    {
        private int _radius;
        private byte _threadsCount;
        private readonly Bitmap _image;

        private readonly double[,] _kernel;

        public CSharpGaussianBlur(int radius, ref Bitmap image, byte threadsCount)
        {
            _radius = radius;
            _image = image;
            _kernel = generateKernel(radius);
            _threadsCount = threadsCount;
            printKernel();
        }

        public Bitmap GetBlurred()
        {
            int width  = _image.Width;
            int height = _image.Height;
            
            // New image to be blurred
            Bitmap return_bmp = new Bitmap(width,height,PixelFormat.Format32bppArgb);
            
            //Locking buffers
            BitmapData srcData = _image.LockBits(new Rectangle(0,0,width,height),ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData dstData = return_bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,PixelFormat.Format32bppArgb);

            try
            {
                int stride = srcData.Stride;
                int bufferSize = Math.Abs(stride) * height;

                byte[] srcBuffer = new byte[bufferSize];
                byte[] dstBuffer = new byte[bufferSize];

                Marshal.Copy(srcData.Scan0, srcBuffer, 0, bufferSize);


                var options = new ParallelOptions
                {
                    MaxDegreeOfParallelism = _threadsCount
                };
                Parallel.For(0, height, options, y =>
                {
                    int rowOffset = (int)y * stride;

                    for (int x = 0; x < width; x++)
                    {
                        int pixelOffset = rowOffset + (x * 4);
                        GetBlurredPixel(srcBuffer, width, height, stride, x, y, out byte b, out byte g,out byte r,out byte a);

                        //Format32bppArgb [B,G,R,A]
                        dstBuffer[pixelOffset + 0] = b;
                        dstBuffer[pixelOffset + 1] = g;
                        dstBuffer[pixelOffset + 2] = r;
                        dstBuffer[pixelOffset + 3] = a;

                    }
                });
                Marshal.Copy(dstBuffer, 0,dstData.Scan0, bufferSize);
            }
            finally
            {
                _image.UnlockBits(srcData);
                return_bmp.UnlockBits(dstData);
            }
            return return_bmp;
        }

        private void GetBlurredPixel(byte[]src,int width,int height, int stride, int x, int y,
            out byte outB, out byte outG, out byte outR, out byte outA)
        {
            double aSum = 0;
            double rSum = 0;
            double gSum = 0;
            double bSum = 0;
            for (int rY = -_radius; rY <= _radius; rY++)
            {
                int sampleY = Math.Clamp(y + rY, 0, height-1);
                int sampleRowOffset = sampleY * stride;

                for (int rX = -_radius; rX <= _radius; rX++)
                {
                    int sampleX = Math.Clamp(x +rX,0,width-1);
                    int sampleOffset = sampleRowOffset + (sampleX * 4);
                    double scale = _kernel[_radius + rX, _radius + rY];

                    //using Format32bppArgb we have 0:blue, 1:green, 2:red, 3:alpha
                    bSum += src[sampleOffset + 0] * scale;
                    gSum += src[sampleOffset + 1] * scale;
                    rSum += src[sampleOffset + 2] * scale;
                    aSum += src[sampleOffset + 3] * scale;
                }
            }

            outB = (byte)Math.Clamp((int)Math.Round(bSum),0, 255);
            outG = (byte)Math.Clamp((int)Math.Round(gSum),0, 255);
            outR = (byte)Math.Clamp((int)Math.Round(rSum),0, 255);
            outA = (byte)Math.Clamp((int)Math.Round(aSum),0, 255);
                
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
