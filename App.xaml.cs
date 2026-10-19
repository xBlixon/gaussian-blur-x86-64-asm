using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;

namespace gaussian_blur_x86_64_asm
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
#if DEBUG
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();
#endif

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

#if DEBUG
            AllocConsole();
            Console.WriteLine("DEBUG MODE");
#endif
        }
    }

}
