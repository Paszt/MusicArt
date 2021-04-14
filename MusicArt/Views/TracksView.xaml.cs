using iTunesLib;
using MusicArt.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace MusicArt.Views
{
    /// <summary>
    /// Interaction logic for DeadTracksView.xaml
    /// </summary>
    public partial class TracksView : UserControl
    {
        public TracksView()
        {
            if (App.GetIsInDesignMode())
            {
                ShowDeadTrackImage = false;
                ShowNoArtTrackImage = true;
            }
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the content to display once the <see cref="TracksView"/> first opens.
        /// </summary>
        public object StartContent
        {
            get => (object)GetValue(StartContentProperty);
            set => SetValue(StartContentProperty, value);
        }
        public static readonly DependencyProperty StartContentProperty =
            DependencyProperty.Register("StartContent", typeof(object), typeof(TracksView),
              new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the content to display once the <see cref="TracksViewModelBase.GetTracks"/> 
        /// method has completed and no tracks were found.
        /// </summary>
        public object NoTracksFoundContent
        {
            get => (object)GetValue(NoTracksFoundContentProperty);
            set => SetValue(NoTracksFoundContentProperty, value);
        }
        public static readonly DependencyProperty NoTracksFoundContentProperty =
            DependencyProperty.Register("NoTracksFoundContent", typeof(object), typeof(TracksView),
              new PropertyMetadata(null));


        public bool ShowDeadTrackImage { get; set; } = false;
        public bool ShowNoArtTrackImage { get; set; } = false;


        private void TrackGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
                if (sender is Grid grid && grid.Tag is TrackViewModel tvm && tvm.IITTrackReference is IITFileOrCDTrack file)
                    file.Reveal();
        }
    }
}
