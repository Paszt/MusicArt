using iTunesLib;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MusicArt.ViewModels
{
    public class Track
    {
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

        public Track(ImageSource coverArt)
        {
            CoverArtImageSource = coverArt;
            CoverArtMaxWidth = 400;
        }

        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public int Duration { get; set; }
        public string Location { get; set; }
        public string Lyrics { get; set; }
        public ImageSource CoverArtImageSource { get; set; }
        public int CoverArtMaxWidth { get; set; }

        public string ArtistAlbum => Artist + (string.IsNullOrEmpty(Album) ? null : " — " + Album);
        public string TitleArtist => Title + " by " + Artist;

        private void SetCoverArt(IITFileOrCDTrack track)
        {
            string coverPath = null;
            string trackDirectory = Path.GetDirectoryName(track.Location);
            string fileName = Path.GetFileNameWithoutExtension(track.Location);
            FileInfo fi = new DirectoryInfo(trackDirectory).GetFiles("*.*")
                            .Where(fileInfo => fileInfo.Extension == ".jpg" || fileInfo.Extension == ".png")
                            .Where(fileInfo =>
                                Path.GetFileNameWithoutExtension(fileInfo.Name) == fileName ||
                                Path.GetFileNameWithoutExtension(fileInfo.Name) == "cover" ||
                                Path.GetFileNameWithoutExtension(fileInfo.Name) == "folder")
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
        }

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
    }
}
