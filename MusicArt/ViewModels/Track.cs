using iTunesLib;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MusicArt.ViewModels
{
    public class Track : BindableModelBase
    {
        // Fields
        private string title;
        private string artist;
        private string album;
        private int duration;
        private string location;
        private string lyrics;
        private ImageSource coverArtImageSource;
        private int coverArtMaxWidth;
        private double coverArtWidth = double.NaN;

        // Constructors
        public Track() { }
        public Track(IITTrack track)
        {
            Title = track.Name;
            Artist = track.Artist;
            Album = track.Album;
            Duration = track.Duration;
            if (track is IITFileOrCDTrack t)
            {
                Location = t.Location;
                SetCoverArt(t);
                Lyrics = t.Lyrics;
            }
        }

        public Track(ImageSource coverArt, int maxWidth)
        {
            CoverArtImageSource = coverArt;
            CoverArtMaxWidth = maxWidth;
            CoverArtWidth = double.NaN;
            ResizeCoverArt();
        }

        // Properties
        public string Title { get => title; set => SetProperty(ref title, value); }
        public string Artist { get => artist; set => SetProperty(ref artist, value); }
        public string Album { get => album; set => SetProperty(ref album, value); }
        public int Duration { get => duration; set => SetProperty(ref duration, value); }
        public string Location { get => location; set => SetProperty(ref location, value); }
        public string Lyrics { get => lyrics; set => SetProperty(ref lyrics, value); }
        public ImageSource CoverArtImageSource { get => coverArtImageSource; set => SetProperty(ref coverArtImageSource, value); }
        public int CoverArtMaxWidth { get => coverArtMaxWidth; set => SetProperty(ref coverArtMaxWidth, value); }
        public double CoverArtWidth { get => coverArtWidth; set => SetProperty(ref coverArtWidth, value); }

        // Readonly Properties
        public string ArtistAlbum => Artist + (string.IsNullOrEmpty(Album) ? null : " — " + Album);
        public string TitleArtist => string.IsNullOrEmpty(Title) && string.IsNullOrEmpty(Artist) 
            ? "MusicArt | No Track" 
            : Title + " by " + Artist;

        // Public Methods
        public static ImageSource ImageSourceFromPath(string path)
        {
            BitmapImage bmp = new();
            bmp.BeginInit();
            // Caches image into memory, so file isn't locked.
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            // Loads image without using an existing image cache. In case image was changed, new image will be loaded.
            bmp.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bmp.UriSource = new Uri(path);
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }

        // Private Methods
        private void SetCoverArt(IITFileOrCDTrack track)
        {
            string coverPath = null;
            string trackDirectory = Path.GetDirectoryName(track.Location);
            string fileName = Path.GetFileNameWithoutExtension(track.Location);
            FileInfo fi = new DirectoryInfo(trackDirectory).GetFiles("*.*")
                            .Where(fileInfo => fileInfo.Extension == ".jpg" || fileInfo.Extension == ".png")
                            .Where(fileInfo =>
                                Path.GetFileNameWithoutExtension(fileInfo.Name) == fileName ||
                                Path.GetFileNameWithoutExtension(fileInfo.Name) == "folder" ||
                                Path.GetFileNameWithoutExtension(fileInfo.Name) == "cover")
                            .FirstOrDefault();
            if (fi != null) coverPath = fi.FullName;

            if (coverPath == null && track.Artwork.Count > 0)
            {
                try
                {
                    TagLib.File tagLibFile = TagLib.File.Create(track.Location);
                    TagLib.IPicture firstPicture = tagLibFile.Tag.Pictures.FirstOrDefault();
                    if (firstPicture != null)
                    {
                        CoverArtImageSource = (ImageSource)new ImageSourceConverter().ConvertFrom(firstPicture.Data.Data);
                        CoverArtMaxWidth = (int)CoverArtImageSource.Width;
                        // Hack to set Taskbar Thumbnail
                        ResizeCoverArt();
                        return;
                    }
                }
                catch (Exception) { }

                //try
                //{
                //    track.Artwork[1].SaveArtworkToFile(coverPath);
                //    coverPath = Path.Combine(Path.GetTempPath(), "MusicArt.img");
                //}
                //catch (Exception) { }
            }
            if (coverPath != null)
            {
                CoverArtImageSource = ImageSourceFromPath(coverPath);
                CoverArtMaxWidth = (int)CoverArtImageSource.Width;
            }
            else
            {
                CoverArtImageSource = (ImageSource)Application.Current.FindResource("Notes");
                CoverArtMaxWidth = 200;
            }
            // Hack to set Taskbar Thumbnail
            ResizeCoverArt();
        }

        // Hack to set Taskbar Thumbnail
        private void ResizeCoverArt()
        {
            CoverArtWidth = 100d;
            CoverArtWidth = double.NaN;
        }

    }
}
