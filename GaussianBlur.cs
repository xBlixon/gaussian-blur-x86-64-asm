using System.Drawing;

namespace gaussian_blur_x86_64_asm
{
    abstract internal class GaussianBlur
    {
        protected int _radius;
        protected byte _threadsCount;
        protected readonly Bitmap _image;
        protected readonly double[] _kernel;

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
                    Console.Write(_kernel[row] + "\t");
                }
                Console.WriteLine();
            }
        }

        protected abstract double[] GenerateKernel(int radius);

        protected abstract void BlurHorizontal(byte[] src,float [] temp, int width, int height, int stride, ParallelOptions options);
        protected abstract void BlurVertical(float[] temp, byte[] dst, int width, int height, int stride, ParallelOptions options);

        public abstract Bitmap GetBlurred();
    }
}
