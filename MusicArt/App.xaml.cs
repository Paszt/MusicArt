using MusicArt.Views;
using System.Windows;
using System.Windows.Threading;

namespace MusicArt
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            if (My.Settings.StartFullscreen)
            {
                FullscreenWindow fullscreenWindow = new();
                fullscreenWindow.Show();
            }
            else
            {
                //TODO: Add miniplayer
                Current.Shutdown();
            }
        }


        // Process unhandled exception
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) =>
            e.Handled = true; // Prevent default unhandled exception processing
    }
}
