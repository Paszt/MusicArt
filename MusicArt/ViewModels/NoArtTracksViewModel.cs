using iTunesLib;

namespace MusicArt.ViewModels
{
    public class NoArtTracksViewModel : TracksViewModelBase
    {
        protected override bool ShouldAddTrack(IITTrack track) => TrackViewModel.IITrackHasFolderArt(track) == false;
        protected override bool ShouldStopTrackIteration() => FoundTracks.Count > 200;
        protected override bool TrackIsFixed(TrackViewModel track) => track.HasFolderArt == true;

        //protected override void GetTracks()
        //{
        //    IsBusy = true;
        //    IsTracksListVisible = true;
        //    if (Tracks == null) InitializeTracks();
        //    else ClearTracks();
        //    IITTrackCollection tracks = iApp.LibraryPlaylist.Tracks;
        //    TotalLibraryTracks = tracks.Count;
        //    int TrackCounter = 1;
        //    while (TrackCounter <= TotalLibraryTracks)
        //    {
        //        StatusText = $"{TrackCounter:N0} / {TotalLibraryTracks:N0}";
        //        TracksProgressValue = (double)(TrackCounter / (decimal)TotalLibraryTracks) * 100;
        //        if (TrackViewModel.IITrackHasFolderArt(tracks[TrackCounter]) == false)
        //            AddTrack(tracks[TrackCounter]);

        //        TrackCounter += 1;
        //        if (Tracks.Count > 200) break;
        //    }
        //    WereNoTracksFound = tracks.Count == 0;
        //    StatusText = string.Empty;
        //    IsBusy = false;
        //}
    }
}
