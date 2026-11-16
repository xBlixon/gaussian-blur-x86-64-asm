using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace gaussian_blur_x86_64_asm
{
    internal class AsmGaussianBlur : GaussianBlur
    {
        public AsmGaussianBlur(int radius, ref Bitmap image, byte threadsCount) : base(radius, ref image, threadsCount)
        {
            PrintKernel();
        }

        public override Bitmap GetBlurred()
        {
            int width = _image.Width;
            int height = _image.Height;

            // New image to be blurred
            Bitmap return_bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            //Locking buffers
            BitmapData srcData = _image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData dstData = return_bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            try
            {
                int stride = Math.Abs(srcData.Stride);
                int bufferSize = Math.Abs(stride) * height;

                byte[] srcBuffer = new byte[bufferSize];
                float[] tempBuffer = new float[bufferSize];
                byte[] dstBuffer = new byte[bufferSize];

                Marshal.Copy(srcData.Scan0, srcBuffer, 0, bufferSize);


                var options = new ParallelOptions
                {
                    MaxDegreeOfParallelism = _threadsCount
                };

                BlurHorizontal(srcBuffer, tempBuffer, width, height, stride, options);
                BlurVertical(tempBuffer, dstBuffer, width, height, stride, options);

                Marshal.Copy(dstBuffer, 0, dstData.Scan0, bufferSize);
            }
            finally
            {
                _image.UnlockBits(srcData);
                return_bmp.UnlockBits(dstData);
            }
            return return_bmp;
        }

        protected override void BlurHorizontal(byte[] src, float[] temp, int width, int height, int stride, ParallelOptions options)
        {
            Parallel.For(0, height, options, y =>
            {
                int rowOffset = y * stride;

                for (int x = 0; x < width; x++)
                {
                    double bSum = 0, gSum = 0, rSum = 0, aSum = 0;
                    for (int i = -_radius; i <= _radius; i++)
                    {
                        int sampleX = Math.Clamp(x + i, 0, width - 1);
                        int sampleOffset = rowOffset + (sampleX * 4);
                        double scale = _kernel[_radius + i];

                        bSum += src[sampleOffset + 0] * scale;
                        gSum += src[sampleOffset + 1] * scale;
                        rSum += src[sampleOffset + 2] * scale;
                        aSum += src[sampleOffset + 3] * scale;
                    }

                    int pixelOffset = rowOffset + (x * 4);
                    temp[pixelOffset + 0] = (float)bSum;
                    temp[pixelOffset + 1] = (float)gSum;
                    temp[pixelOffset + 2] = (float)rSum;
                    temp[pixelOffset + 3] = (float)aSum;
                }
            });
        }

        protected override void BlurVertical(float[] temp, byte[] dst, int width, int height, int stride, ParallelOptions options)
        {
            Parallel.For(0, height, options, y =>
            {
                int rowOffset = y * stride;

                for (int x = 0; x < width; x++)
                {
                    double bSum = 0, gSum = 0, rSum = 0, aSum = 0;
                    int colOffset = x * 4;
                    for (int j = -_radius; j <= _radius; j++)
                    {
                        int sampleY = Math.Clamp(y + j, 0, height - 1);
                        int sampleOffset = (sampleY * stride) + colOffset;
                        double scale = _kernel[_radius + j];

                        bSum += temp[sampleOffset + 0] * scale;
                        gSum += temp[sampleOffset + 1] * scale;
                        rSum += temp[sampleOffset + 2] * scale;
                        aSum += temp[sampleOffset + 3] * scale;
                    }

                    int pixelOffset = rowOffset + colOffset;
                    dst[pixelOffset + 0] = (byte)Math.Clamp((int)Math.Round(bSum), 0, 255);
                    dst[pixelOffset + 1] = (byte)Math.Clamp((int)Math.Round(gSum), 0, 255);
                    dst[pixelOffset + 2] = (byte)Math.Clamp((int)Math.Round(rSum), 0, 255);
                    dst[pixelOffset + 3] = (byte)Math.Clamp((int)Math.Round(aSum), 0, 255);
                }
            });
        }

        protected override double[] GenerateKernel(int radius)
        {
            int side = radius * 2 + 1;
            double[] kernel = new double[side];
            double sigma = radius / 3.0; // Standard deviation for Gaussian function
            double twoSigmaSquare = 2.0 * sigma * sigma;

            double sum = 0.0;
            for (int i = 0; i < side; i++)
            {
                int x = i - radius;
                kernel[i] = Math.Exp(-(x * x) / twoSigmaSquare);
                sum += kernel[i];
            }

            // Kernel normalization
            for (int i = 0; i < side; i++)
            {
                    kernel[i] /= sum;
            }

            return kernel;
        }
    }
}
