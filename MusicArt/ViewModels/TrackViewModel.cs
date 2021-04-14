using iTunesLib;
using System.IO;
using System.Linq;

namespace MusicArt.ViewModels
{
    public class TrackViewModel : BindableModelBase
    {
        private string name;
        private string artist;
        private string album;
        private string location;
        private long trackNumber;
        private bool? hasFolderArt;
        private IITTrack iITTrackReference;

        public TrackViewModel() { }

        public TrackViewModel(IITTrack itTrack)
        {
            iITTrackReference = itTrack;
            UpdateFromIITTrackReference();
        }

        public TrackViewModel(string artist, string album, int trackNumber, string name, bool hasFolderArt = false)
        {
            Artist = artist;
            Album = album;
            TrackNumber = trackNumber;
            Name = name;
            HasFolderArt = hasFolderArt;
        }

        public string Name { get => name; set => SetProperty(ref name, value); }
        public string Artist { get => artist; set => SetProperty(ref artist, value); }
        public string Album { get => album; set => SetProperty(ref album, value); }
        public string Location { get => location; set { if (SetProperty(ref location, value)) UpdateHasFolderArt(); } }
        public long TrackNumber { get => trackNumber; set => SetProperty(ref trackNumber, value); }
        public bool? HasFolderArt { get => hasFolderArt; set => SetProperty(ref hasFolderArt, value); }
        public IITTrack IITTrackReference => iITTrackReference;

        public void Reveal()
        {
            if (IITTrackReference is IITFileOrCDTrack itFile)
                itFile.Reveal();
        }

        public void UpdateFromIITTrackReference()
        {
            name = IITTrackReference.Name;
            artist = IITTrackReference.Artist;
            album = IITTrackReference.Album;
            trackNumber = IITTrackReference.TrackNumber;
            UpdateHasFolderArt();
            if (IITTrackReference is IITFileOrCDTrack itFile)
                Location = itFile.Location;
        }

        public void RemoveItunesReference() => iITTrackReference = null;

        private void UpdateHasFolderArt() => HasFolderArt = TrackPathHasFolderArt(Location);

        public static bool? IITrackHasFolderArt(IITTrack itTrack) =>
            itTrack is IITFileOrCDTrack iitf
                ? TrackPathHasFolderArt(iitf.Location)
                : null;

        private static bool? TrackPathHasFolderArt(string location)
        {
            if (string.IsNullOrEmpty(location) || !File.Exists(location))
                return null;

            string trackDirectory = Path.GetDirectoryName(location);
            string fileName = Path.GetFileNameWithoutExtension(location);
            FileInfo fi = new DirectoryInfo(trackDirectory).GetFiles("*.*")
                                        .Where(fileInfo => fileInfo.Extension == ".jpg" || fileInfo.Extension == ".png")
                                        .Where(fileInfo =>
                                            Path.GetFileNameWithoutExtension(fileInfo.Name) == fileName ||
                                            Path.GetFileNameWithoutExtension(fileInfo.Name) == "cover" ||
                                            Path.GetFileNameWithoutExtension(fileInfo.Name) == "folder")
                                        .FirstOrDefault();
            return fi != null;
        }

    }
}
