using iTunesLib;
using MusicArt.Commands;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Data;

namespace MusicArt.ViewModels
{
    public abstract class TracksViewModelBase : BindableModelBase
    {
        // Fields
        protected iTunesApp iApp;
        private bool isBusy;
        private bool isTracksListVisible = false;
        private bool removeTrackWhenFixed;
        private string statusText;
        private BindingList<TrackViewModel> tracks;
        private double tracksProgressValue;
        private int totalLibraryTracks;
        private bool wereNoTracksFound = false;

        //Constructors
        public TracksViewModelBase()
        {
            Tracks = new();
            AddTracksGroupings();
            if (App.GetIsInDesignMode())
            {
                tracksProgressValue = 45;
                StatusText = "467 / 23,733";
                IsBusy = true;
                AddSampleTracks();
                IsTracksListVisible = true;
            }
            else
            {
                iApp = App.Current.iApp;
                iApp.OnDatabaseChangedEvent += IApp_DatabaseChanged;
            }
        }

        // Protected Methods
        protected virtual void IApp_DatabaseChanged(object deletedObjectIDs, object changedObjectIDs)
        {
            if (Tracks.Count == 0) return;
            // deleted items
            object[,] deletedList = (object[,])deletedObjectIDs;
            for (int i = 0; i < deletedList.GetLength(0) - 1; i++)
            {
                int trackId = (int)deletedList[i, 2];
                if (trackId != 0)
                {
                    TrackViewModel track = Tracks.Where(t => t.IITTrackReference?.trackID == trackId).FirstOrDefault();
                    if (track != null)
                        RemoveTrack(track);
                }
            }

            // changed items
            object[,] changedList = (object[,])changedObjectIDs;
            for (int i = 0; i < changedList.GetLength(0) - 1; i++)
            {
                int trackId = (int)changedList[i, 2];
                if (trackId != 0)
                {
                    TrackViewModel track = Tracks.Where(t => t.IITTrackReference?.trackID == trackId).FirstOrDefault();
                    if (track != null)
                    {
                        track.UpdateFromIITTrackReference();
                        if (RemoveTrackWhenFixed && TrackIsFixed(track))
                            RemoveTrack(track);
                    }
                    //Tracks.Where(t => t.IITTrackReference?.trackID == trackId).FirstOrDefault()?.UpdateFromIITTrackReference();
                }
            }
        }

        protected virtual bool TrackIsFixed(TrackViewModel track) => false;

        protected void GetTracks()
        {
            IsBusy = true;
            IsTracksListVisible = true;
            if (Tracks == null) InitializeTracks();
            else ClearTracks();
            IITTrackCollection tracks = iApp.LibraryPlaylist.Tracks;
            TotalLibraryTracks = tracks.Count;
            int TrackCounter = 1;
            while (TrackCounter <= TotalLibraryTracks)
            {
                StatusText = $"{TrackCounter:N0} / {TotalLibraryTracks:N0}";
                TracksProgressValue = (double)(TrackCounter / (decimal)TotalLibraryTracks) * 100;
                if (ShouldAddTrack(tracks[TrackCounter]))
                    AddTrack(tracks[TrackCounter]);

                TrackCounter += 1;
                if (ShouldStopTrackIteration()) break;
            }
            WereNoTracksFound = Tracks.Count == 0;
            StatusText = string.Empty;
            IsBusy = false;
        }

        protected abstract bool ShouldAddTrack(IITTrack track);
        protected abstract bool ShouldStopTrackIteration();

        // If there are bindings on the BindingList, not calling methods on UI thread will throw errros.
        public void AddTrack(IITTrack itTrack) =>
            App.Current?.Dispatcher.Invoke((Action<TrackViewModel>)Tracks.Add, new TrackViewModel(itTrack));
        public void ClearTracks() => App.Current?.Dispatcher.Invoke(Tracks.Clear);
        public void RemoveTrack(TrackViewModel track)
        {
            track.RemoveItunesReference();
            App.Current?.Dispatcher.Invoke(new Action(() => Tracks.Remove(track)));
        }

        internal void InitializeTracks()
        {
            App.Current?.Dispatcher.Invoke(new Action(() => Tracks = new()));
            AddTracksGroupings();
        }

        public void Dispose()
        {
            try
            {
                if (iApp != null)
                    iApp.OnDatabaseChangedEvent -= IApp_DatabaseChanged;
            }
            catch (Exception) { }
        }

        // Properties
        public bool IsBusy { get => isBusy; set => SetProperty(ref isBusy, value); }
        public bool RemoveTrackWhenFixed { get => removeTrackWhenFixed; set => SetProperty(ref removeTrackWhenFixed, value); }
        public string StatusText { get => statusText; set => SetProperty(ref statusText, value); }
        public BindingList<TrackViewModel> Tracks { get => tracks; set => SetProperty(ref tracks, value); }
        public double TracksProgressValue { get => tracksProgressValue; set => SetProperty(ref tracksProgressValue, value); }
        public int TotalLibraryTracks { get => totalLibraryTracks; set => SetProperty(ref totalLibraryTracks, value); }
        public bool WereNoTracksFound { get => wereNoTracksFound; set => SetProperty(ref wereNoTracksFound, value); }
        public bool IsTracksListVisible { get => isTracksListVisible; set => SetProperty(ref isTracksListVisible, value); }

        // Commands
        private RelayCommand getTracksCommand;
        public RelayCommand GetTracksCommand => getTracksCommand ??= new(() =>
        {
            // Setting thread to background causes the thread to be automatically aborted when window is closed.
            Thread thread = new(GetTracks) { IsBackground = true };
            thread.Start();
        }, () => App.GetIsInDesignMode() || App.Current?.iApp != null && !IsBusy);


        private void AddSampleTracks()
        {
            Tracks = new()
            {
                new("Artist 1", "Album 1", 1, "Song 1") { Location = "C:/Music/1.flac" },
                new("Artist 1", "Album 1", 2, "Song 2", true),
                new("Artist 1", "Album 1", 3, "Song 3"),
                new("Artist 1", "Album 1", 4, "Song 4"),
                new("Artist 1", "Album 1", 5, "Song 5", true),
                new("Artist 1", "Album 1", 6, "Song 6") { Location = "C:/Music/6.flac" },
                new("Artist 1", "Album 1", 7, "Song 7"),
                new("Artist 1", "Album 1", 8, "Song 8"),
                new("Artist 1", "Album 1", 9, "Song 9"),
                new("Artist 1", "Album 1", 10, "Song 10"),

                new("Artist 1", "Album 2", 1, "Song 1"),
                new("Artist 1", "Album 2", 2, "Song 2"),
                new("Artist 1", "Album 2", 3, "Song 3"),
                new("Artist 1", "Album 2", 4, "Song 4"),
                new("Artist 1", "Album 2", 5, "Song 5"),
                new("Artist 1", "Album 2", 6, "Song 6"),


                new("Artist 2", "Album 1", 1, "Song 1"),
                new("Artist 2", "Album 1", 2, "Song 2"),
                new("Artist 2", "Album 1", 3, "Song 3"),
                new("Artist 2", "Album 1", 4, "Song 4"),
                new("Artist 2", "Album 1", 5, "Song 5"),
                new("Artist 2", "Album 1", 6, "Song 6"),
                new("Artist 2", "Album 1", 7, "Song 7"),
                new("Artist 2", "Album 1", 8, "Song 8"),
                new("Artist 2", "Album 1", 9, "Song 9"),
                new("Artist 2", "Album 1", 10, "Song 10"),
            };
            AddTracksGroupings();
        }

        private void AddTracksGroupings()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(Tracks);
            view.GroupDescriptions.Add(new PropertyGroupDescription("Artist"));
            view.GroupDescriptions.Add(new PropertyGroupDescription("Album"));
        }
    }
}
