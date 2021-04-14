using MusicArt.ViewModels;
using System.Reflection;
using System.Windows;

namespace MusicArt.Views
{
    /// <summary>
    /// Interaction logic for LibraryInfoWindow.xaml
    /// </summary>
    public partial class LibraryInfoWindow : Window
    {
        private static LibraryInfoWindow instance;
        public static LibraryInfoWindow Instance
        {
            get
            {
                if (instance == null || (bool)typeof(Window).GetProperty("IsDisposed",
                    BindingFlags.NonPublic | BindingFlags.Instance).GetValue(instance))
                    return instance = new();
                if (instance.IsLoaded)
                {
                    instance.Activate();
                }
                return instance;
            }
        }

        public LibraryInfoWindow() => InitializeComponent();

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ((TracksViewModelBase)NoArtTracksView.DataContext).Dispose();
            ((TracksViewModelBase)DeadTracksView.DataContext).Dispose();
        }
    }
}
