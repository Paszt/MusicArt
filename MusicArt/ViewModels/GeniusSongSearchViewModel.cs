using MusicArt.Commands;
using MusicArt.ViewModels.Genius;
using System;
using System.Collections.ObjectModel;

namespace MusicArt.ViewModels
{
    public class GeniusSongSearchViewModel : BindableModelBase
    {
        public static string TestProp => "Shit";

        private bool searchHasStarted;
        private bool isWaiting;
        private bool noSongsFound;
        private ObservableCollection<GeniusSearchResult> searchResults;

        public bool SearchHasStarted
        {
            get => searchHasStarted;
            set { if (SetProperty(ref searchHasStarted, value)) NotifyPropertyChanged(nameof(SearchHasNotStarted)); }
        }
        public bool SearchHasNotStarted => !SearchHasStarted;
        public bool IsWaiting { get => isWaiting; set => SetProperty(ref isWaiting, value); }
        public bool NoSongsFound { get => noSongsFound; set => SetProperty(ref noSongsFound, value); }
        public ObservableCollection<GeniusSearchResult> SearchResults { get => searchResults; set => SetProperty(ref searchResults, value); }

        public void Reset()
        {
            if (!SearchHasStarted) return;
            IsWaiting = false;
            SearchHasStarted = false;
            NoSongsFound = false;
            App.Current?.Dispatcher?.BeginInvoke(new Action(() => SearchResults?.Clear()));
        }

        private RelayCommand<string> geniusSearchCommand;
        public RelayCommand<string> GeniusSearchCommand => geniusSearchCommand ??= new(async (string query) =>
        {
            IsWaiting = true;
            SearchHasStarted = true;
            using Services.GeniusService genius = new();
            SearchResults = await genius.SongSearchAsync(query);
            NoSongsFound = SearchResults.Count == 0;
            IsWaiting = false;
        });

    }
}
