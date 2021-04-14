using iTunesLib;

namespace MusicArt.ViewModels
{
    public class DeadTracksViewModel : TracksViewModelBase
    {
        protected override bool ShouldAddTrack(IITTrack track) => 
            track is IITFileOrCDTrack iitf && string.IsNullOrEmpty(iitf.Location);
        protected override bool ShouldStopTrackIteration() => false;
        protected override bool TrackIsFixed(TrackViewModel track) => !string.IsNullOrEmpty(track.Location);

        //protected override void GetTracksThunk()
        //{
        //    //IsBusy = true;
        //    //IsTracksListVisible = true;
        //    //if (Tracks == null) InitializeTracks();
        //    //else ClearTracks();
        //    IITTrackCollection tracks = iApp.LibraryPlaylist.Tracks;
        //    TotalLibraryTracks = tracks.Count;
        //    int TrackCounter = 1;
        //    while (TrackCounter <= TotalLibraryTracks)
        //    {
        //        StatusText = $"{TrackCounter:N0} / {TotalLibraryTracks:N0}";
        //        TracksProgressValue = (double)(TrackCounter / (decimal)TotalLibraryTracks) * 100;
        //        if (tracks[TrackCounter] is IITFileOrCDTrack iitf && string.IsNullOrEmpty(iitf.Location))
        //            AddTrack(iitf);
        //        TrackCounter += 1;
        //    }
        //    //WereNoTracksFound = tracks.Count == 0;
        //    //StatusText = string.Empty;
        //    //IsBusy = false;
        //}

    }
}
