﻿using iTunesLib;
using MusicArt.Commands;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;

namespace MusicArt.ViewModels
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles",
        Justification = "That's how it's spelled.")]
    public class iTunesViewModel : ViewModelBase
    {
        private iTunesApp iApp;
        private bool isItunesClosed = true;
        private bool isNextEnabled;
        private bool isPauseEnabled;
        private bool isPlayEnabled;
        private bool isPreviousEnabled;
        private bool isStopEnabled;
        private Track track;
        private TaskbarItemProgressState progressState = TaskbarItemProgressState.None;
        private double progressValue;
        private Thickness thumbnailClipMargin;
        private Timer updateProgressTimer;

        public iTunesViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                Track = new()
                {
                    Artist = "Bob Dylan",
                    Title = "Visions of Johanna",
                    Album = "Blonde on Blonde",
                    CoverArtImageSource = new BitmapImage(new(@"https://lastfm.freetls.fastly.net/i/u/8dbd0e1e45eb4affb6416ec329bdf49b.jpg")),
                    Lyrics = "Ain’t it just like the night to play tricks when you’re tryin' to be so quiet?\nWe sit here stranded, though we’re all doin’ our best to deny it\nAnd Louise holds a handful of rain, temptin’ you to defy it\nLights flicker from the opposite loft\nIn this room the heat pipes just cough\nThe country music station plays soft\nBut there’s nothing, really nothing to turn off\nJust Louise and her lover so entwined\nAnd these visions of Johanna that conquer my mind\n\nIn the empty lot where the ladies play blindman’s bluff with the key chain\nAnd the all-night girls they whisper of escapades out on the “D” train\nWe can hear the night watchman click his flashlight\nAsk himself if it’s him or them that’s really insane\nLouise, she’s all right, she’s just near\nShe’s delicate and seems like the mirror\nBut she just makes it all too concise and too clear\nThat Johanna’s not here\nThe ghost of ’lectricity howls in the bones of her face\nWhere these visions of Johanna have now taken my place\n\nNow, little boy lost, he takes himself so seriously\nHe brags of his misery, he likes to live dangerously\nAnd when bringing her name up\nHe speaks of a farewell kiss to me\nHe’s sure got a lotta gall to be so useless and all\nMuttering small talk at the wall while I’m in the hall\nHow can I explain?\nIt's so hard to get on\nAnd these visions of Johanna, they kept me up past the dawn\n\nInside the museums, Infinity goes up on trial\nVoices echo this is what salvation must be like after a while\nBut Mona Lisa musta had the highway blues\nYou can tell by the way she smiles\nSee the primitive wallflower freeze\nWhen the jelly-faced women all sneeze\nHear the one with the mustache say, “Jeeze\nI can’t find my knees”\nOh, jewels and binoculars hang from the head of the mule\nBut these visions of Johanna, they make it all seem so cruel\n\nThe peddler now speaks to the countess who’s pretending to care for him\nSayin’, “Name me someone that’s not a parasite and I’ll go out and say a prayer for him”\nBut like Louise always says\n“Ya can’t look at much, can ya man?”\nAs she, herself, prepares for him\nAnd Madonna, she still has not showed\nWe see this empty cage now corrode\nWhere her cape of the stage once had flowed\nThe fiddler, he now steps to the road\nHe writes ev’rything’s been returned which was owed\nOn the back of the fish truck that loads\nWhile my conscience explodes\nThe harmonicas play the skeleton keys and the rain\nAnd these visions of Johanna are now all that remain",
                };
            }
            else ConnectToItunes();
        }

        public Track Track { get => track; set { if (SetProperty(ref track, value)) TrackChanged(); } }

        public bool TrackHasLyrics => string.IsNullOrWhiteSpace(Track.Lyrics);
        public bool TrackDoesntHaveLyrics => !TrackHasLyrics;
        public bool IsNextEnabled { get => isNextEnabled; set => SetProperty(ref isNextEnabled, value); }
        public bool IsPauseEnabled { get => isPauseEnabled; set => SetProperty(ref isPauseEnabled, value); }
        public bool IsPlayEnabled { get => isPlayEnabled; set => SetProperty(ref isPlayEnabled, value); }
        public bool IsPreviousEnabled { get => isPreviousEnabled; set => SetProperty(ref isPreviousEnabled, value); }
        public bool IsStopEnabled { get => isStopEnabled; set => SetProperty(ref isStopEnabled, value); }
        public bool IsItunesClosed { get => isItunesClosed; set => SetProperty(ref isItunesClosed, value); }
        public double ProgressValue { get => progressValue; set => SetProperty(ref progressValue, value); }
        public TaskbarItemProgressState ProgressState { get => progressState; set => SetProperty(ref progressState, value); }
        public Thickness ThumbnailClipMargin { get => thumbnailClipMargin; set => SetProperty(ref thumbnailClipMargin, value); }


        public void GetPlayerButtonsState(out bool previousEnabled, out ITPlayButtonState playPauseStopState, out bool nextEnabled) =>
            iApp.GetPlayerButtonsState(out previousEnabled, out playPauseStopState, out nextEnabled);

        public void DisconnectFromItunes()
        {
            updateProgressTimer.Change(Timeout.Infinite, Timeout.Infinite);
            updateProgressTimer?.Dispose();
            iApp.OnPlayerPlayEvent -= iApp_PlayerPlaying;
            iApp.OnPlayerStopEvent -= iApp_PlayerStopped;
            iApp.OnPlayerPlayingTrackChangedEvent -= iApp_PlayingTrackInfoChanged;
            iApp.OnQuittingEvent -= iApp_iTunesQuitting;
            iApp.OnAboutToPromptUserToQuitEvent -= iApp_AboutToPromptUserToQuit;
            iApp.OnSoundVolumeChangedEvent -= iApp_SoundVolumeChanged;
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(iApp);
            iApp = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            IsItunesClosed = true;
        }

        private void iApp_AboutToPromptUserToQuit() => DisconnectFromItunes();

        private void iApp_PlayingTrackInfoChanged(object iTrack) => UpdateTrackAndPlayerState();

        private void iApp_PlayerPlaying(object iTrack) => UpdateTrackAndPlayerState();

        private void iApp_PlayerStopped(object iTrack) => UpdateTrackAndPlayerState();

        private void UpdateTrackAndPlayerState()
        {
            Track = iApp.CurrentTrack != null 
                ? new(iApp.CurrentTrack) 
                : new((ImageSource)Application.Current.FindResource("Cassette"));
            GetPlayerButtonsState(out bool previousEnabled, out _, out bool nextEnabled);
            IsPreviousEnabled = previousEnabled;
            IsNextEnabled = nextEnabled;
            IsPauseEnabled = iApp.PlayerState == ITPlayerState.ITPlayerStatePlaying;
            IsPlayEnabled = !IsPauseEnabled;
            IsStopEnabled = IsPauseEnabled;
        }

        private void TrackChanged()
        {
            NotifyPropertyChanged(nameof(TrackHasLyrics));
            NotifyPropertyChanged(nameof(TrackDoesntHaveLyrics));
        }

        private void iApp_iTunesQuitting() => DisconnectFromItunes();
        private void iApp_SoundVolumeChanged(int newVolume) { }

        private void ConnectToItunes()
        {
            iApp = new();
            IsItunesClosed = false;
            iApp.OnPlayerPlayEvent += iApp_PlayerPlaying;
            iApp.OnPlayerStopEvent += iApp_PlayerStopped;
            iApp.OnQuittingEvent += iApp_iTunesQuitting;
            iApp.OnAboutToPromptUserToQuitEvent += iApp_AboutToPromptUserToQuit;
            iApp.OnSoundVolumeChangedEvent += iApp_SoundVolumeChanged;
            UpdateTrackAndPlayerState();
            updateProgressTimer = new((e) => UpdateProgress(), null, TimeSpan.Zero, TimeSpan.FromSeconds(0.5));
        }

        private void UpdateProgress()
        {
            if (iApp.CurrentTrack == null || Track == null || Track.Duration == 0) ProgressState = TaskbarItemProgressState.None;
            else
            {
                ProgressValue = (double)(iApp.PlayerPosition / (decimal)Track.Duration);
                switch (iApp?.PlayerState)
                {
                    case ITPlayerState.ITPlayerStatePlaying:
                        ProgressState = TaskbarItemProgressState.Normal;
                        break;
                    case ITPlayerState.ITPlayerStateStopped:
                        ProgressState = TaskbarItemProgressState.Paused;
                        break;
                }
            }
        }

        private void ShowTrackInItunes()
        {
            if (iApp.CurrentTrack is IITFileOrCDTrack iitf) iitf.Reveal();
        }

        private void OpenTrackFolder() => Process.Start("explorer.exe",
            string.Format(@"""{0}""", System.IO.Path.GetDirectoryName(Track.Location)));

        #region Commands

        private RelayCommand copyArtistAlbumCommand;
        public RelayCommand CopyArtistAlbumCommand => copyArtistAlbumCommand ??= new(() =>
            Clipboard.SetText(Track?.Artist + " " + Track?.Album));

        private RelayCommand nextTrackCommand;
        public RelayCommand NextTrackCommand => nextTrackCommand ??= new(() => iApp.NextTrack());

        private RelayCommand playPauseCommand;
        public RelayCommand PlayPauseCommand => playPauseCommand ??= new(() => iApp.PlayPause());

        private RelayCommand previousTrackCommand;
        public RelayCommand PreviousTrackCommand => previousTrackCommand ??= new(() => iApp.PreviousTrack());

        private RelayCommand startItunesCommand;
        public RelayCommand StartItunesCommand => startItunesCommand ??= new(() => ConnectToItunes());

        private RelayCommand showItunesCommand;
        public RelayCommand ShowItunesCommand => showItunesCommand ??= new(() => ShowTrackInItunes());

        private RelayCommand openTrackFolderCommand;
        public RelayCommand OpenTrackFolderCommand => openTrackFolderCommand ??= new(() => OpenTrackFolder());

        private RelayCommand refreshTrackInfoCommand;
        public RelayCommand RefreshTrackInfoCommand => refreshTrackInfoCommand ??= new(() => UpdateTrackAndPlayerState());

        private RelayCommand showItunesOpenFolderCommand;
        public RelayCommand ShowItunesOpenFolderCommand => showItunesOpenFolderCommand ??= new(() =>
        {
            ShowTrackInItunes();
            OpenTrackFolder();
        });

        #endregion

    }
}