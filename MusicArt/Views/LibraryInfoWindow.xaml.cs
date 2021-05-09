using MusicArt.ViewModels;
using System.Reflection;
using System.Windows;
using Paszt.WPF.Extensions;

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

        private void Window_Loaded(object sender, RoutedEventArgs e) => this.SetPlacement(My.Settings.ReportsWindowPlacement);

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            My.Settings.ReportsWindowPlacement = this.GetPlacement();
            My.Settings.Save();
            ((TracksViewModelBase)NoArtTracksView.DataContext).Dispose();
            ((TracksViewModelBase)DeadTracksView.DataContext).Dispose();
        }

    }
}
