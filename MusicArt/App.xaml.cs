using iTunesLib;
using MusicArt.Views;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace MusicArt
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;
        internal iTunesApp iApp;


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

        private static readonly DependencyObject dependencyObject = new();
        internal static bool GetIsInDesignMode() => DesignerProperties.GetIsInDesignMode(dependencyObject);

        // Process unhandled exception
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) =>
            e.Handled = true; // Prevent default unhandled exception processing
    }
}
