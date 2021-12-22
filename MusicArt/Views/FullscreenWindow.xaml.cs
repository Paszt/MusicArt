using MusicArt.Resources;
using MusicArt.ViewModels;
using Paszt.WPF.Windows;
using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MusicArt.Views
{
    /// <summary>
    /// Interaction logic for FullscreenWindow.xaml
    /// </summary>
    public partial class FullscreenWindow : Window
    {
        private iTunesViewModel ViewModel => (iTunesViewModel)DataContext;

        public FullscreenWindow() => InitializeComponent();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Top = SystemParameters.WorkArea.Top;
            Left = SystemParameters.WorkArea.Left;
            Width = SystemParameters.WorkArea.Width;
            Height = SystemParameters.WorkArea.Height;
            if (My.Settings.LeftColumnWidth == -1) My.Settings.LeftColumnWidth = LeftColumn.ActualWidth;
            LeftColumn.Width = My.Settings.IsLeftColumnCollapsed ? (new(0)) : (new(My.Settings.LeftColumnWidth));
            if (My.Settings.IsProgressBarVisible) ToggleProgressOpacity();
            UpdateLeftColumnProps();
            CoverArtImage.UpdateLayout();
            UpdateTaskbarThumbnail();
            ViewModel.ToggleFullscreen += ToggleFullscreen;
            ViewModel.UpdateTaskbarThumbnail += UpdateTaskbarThumbnail;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => ViewModel.DisconnectFromItunes();

        private void CoverArtImage_SizeChanged(object sender, SizeChangedEventArgs e) =>
            App.Current.Dispatcher.Invoke(() => UpdateTaskbarThumbnail());

        private void UpdateTaskbarThumbnail()
        {

            Point wpfPoint = CoverArtImage.TransformToAncestor(this).Transform(new Point(0, 0));
            ViewModel.ThumbnailClipMargin = new(wpfPoint.X, wpfPoint.Y,
                ActualWidth - CoverArtImage.ActualWidth - wpfPoint.X,
                ActualHeight - CoverArtImage.ActualHeight - wpfPoint.Y);
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
            ControlButtonFadeOutStoryboard.Begin();
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e) => ToggleFullscreen();

        private void ToggleFullscreen()
        {
            if (SystemParameters.WorkArea.Height != SystemParameters.PrimaryScreenHeight)
            {
                // Taskbar is Top or Bottom
                if (Height == SystemParameters.WorkArea.Height)
                {
                    Height = SystemParameters.PrimaryScreenHeight;
                    Top = 0;
                    ViewModel.ToggleFullscreenImageSource = (ImageSource)Application.Current.FindResource("ExitFullscreen");
                    ViewModel.ToggleFullScreenImage.Source = ViewModel.ToggleFullscreenImageSource;
                    //RestoreImage.Source = (ImageSource)Application.Current.FindResource("ExitFullscreen");
                    //RestoreButton.ToolTip = "Exit Fullscreen (Esc or F11)";
                    ViewModel.ToggleFullscreenText = "Exit Fullscreen (Esc or F11)";
                }
                else
                {
                    Height = SystemParameters.WorkArea.Height;
                    Top = SystemParameters.WorkArea.Top;
                    ViewModel.ToggleFullscreenImageSource = (ImageSource)Application.Current.FindResource("Fullscreen");
                    ViewModel.ToggleFullScreenImage.Source = ViewModel.ToggleFullscreenImageSource;
                    //RestoreImage.Source = (ImageSource)Application.Current.FindResource("Fullscreen");
                    //RestoreButton.ToolTip = "Fullscreen (Esc or F11)";
                    ViewModel.ToggleFullscreenText = "Fullscreen (Esc or F11)";
                }
            }
            else
            {
                // Taskbar is Left or Right
                if (Width == SystemParameters.WorkArea.Width)
                {
                    Width = SystemParameters.PrimaryScreenWidth;
                    Left = 0;
                    RestoreImage.Source = (ImageSource)Application.Current.FindResource("ExitFullscreen");
                    RestoreButton.ToolTip = "Exit Fullscreen (Esc or F11)";
                }
                else
                {
                    Width = SystemParameters.WorkArea.Width;
                    Left = SystemParameters.WorkArea.Left;
                    RestoreImage.Source = (ImageSource)Application.Current.FindResource("Fullscreen");
                    RestoreButton.ToolTip = "Fullscreen (Esc or F11)";
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat) return;
            switch (e.Key)
            {
                case Key.Escape:
                case Key.F11:
                    ToggleFullscreen();
                    break;
                case Key.I:
                    TrackInfoGrid.BeginAnimation(OpacityProperty,
                        new DoubleAnimation(0.7, TimeSpan.FromSeconds(0.2)));
                    break;
                case Key.L:
                    LibraryInfoWindow.Instance.Show();
                    break;
                case Key.M:
                    WindowState = WindowState.Minimized;
                    break;
                case Key.P:
                    ToggleProgressOpacity();
                    break;
                case Key.OemQuestion:
                    ShortcutGuideGrid.Visibility = Visibility.Visible;
                    ShortcutGuideGrid.BeginAnimation(OpacityProperty,
                        new DoubleAnimation(1, TimeSpan.FromSeconds(0.2)));
                    break;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.C:
                case Key.R:
                    DoubleAnimationUsingKeyFrames flashAnimation = new()
                    {
                        KeyFrames = new()
                        {
                            new LinearDoubleKeyFrame(0.2, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.1))),
                            new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.1)))
                        }
                    };
                    flashAnimation.Completed += (s, e) => OverlayGrid.Visibility = Visibility.Hidden;
                    OverlayGrid.Visibility = Visibility.Visible;
                    OverlayGrid.BeginAnimation(OpacityProperty, flashAnimation);
                    break;
                case Key.I:
                    TrackInfoGrid.BeginAnimation(OpacityProperty, new DoubleAnimation(0, TimeSpan.FromSeconds(0.2)));
                    break;
                case Key.Right:
                case Key.Left:
                case Key.F12:
                    AnimateLeftColumn();
                    break;
                case Key.OemQuestion:
                    DoubleAnimation hideGridAnimation = new(0, TimeSpan.FromSeconds(0.2));
                    hideGridAnimation.Completed += (object sender, EventArgs e) => ShortcutGuideGrid.Visibility = Visibility.Collapsed;
                    ShortcutGuideGrid.BeginAnimation(OpacityProperty, hideGridAnimation);
                    break;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        private void GridSplitter_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (LeftColumn.ActualWidth < 220) LeftColumn.Width = new(0);
            UpdateLeftColumnProps();
        }

        private void CollapseExpandLeftColButton_Click(object sender, RoutedEventArgs e) => AnimateLeftColumn();

        private void AnimateLeftColumn()
        {
            ExpandCollapseState resultState = LeftColumn.Width.Value <= 20
                ? ExpandCollapseState.Expanded
                : ExpandCollapseState.Collapsed;
            Storyboard storyBoard = new();
            double resultWidth = (resultState == ExpandCollapseState.Expanded) ? My.Settings.LeftColumnWidth : 0d;
            GridLengthAnimation animation = new(resultWidth, TimeSpan.FromSeconds(0.1));
            animation.Completed += delegate
            {
                // Set the animation to null on completion. This allows the grid to be resized manually.
                LeftColumn.BeginAnimation(ColumnDefinition.WidthProperty, null);
                // Set the final value manually.
                LeftColumn.Width = new GridLength(resultWidth);
                // Update Settings and button properties to reflect new state.
                UpdateLeftColumnProps();
                // Update the taskbar thumbnail
                UpdateTaskbarThumbnail();
            };
            storyBoard.Children.Add(animation);
            Storyboard.SetTarget(animation, LeftColumn);
            Storyboard.SetTargetProperty(animation, new PropertyPath(ColumnDefinition.WidthProperty));
            storyBoard.Begin();
        }

        private void ToggleProgressOpacity()
        {
            double toOpacityValue = TrackProgress.Opacity > 0 ? 0d : 1d;
            DoubleAnimation progressOpacityAnimation = new(toOpacityValue, TimeSpan.FromSeconds(0.2));
            progressOpacityAnimation.Completed += (s, e) =>
            {
                My.Settings.IsProgressBarVisible = TrackProgress.Opacity == 1;
                My.Settings.Save();
                if (TrackProgress.Opacity == 0) TrackProgress.Visibility = Visibility.Collapsed;
            };
            if (TrackProgress.Opacity == 0) TrackProgress.Visibility = Visibility.Visible;
            TrackProgress.BeginAnimation(OpacityProperty, progressOpacityAnimation);
        }

        private void UpdateLeftColumnProps()
        {
            double newAngle;
            if (LeftColumn.Width.Value <= 20)
            {
                My.Settings.IsLeftColumnCollapsed = true;
                newAngle = 180;
            }
            else
            {
                My.Settings.LeftColumnWidth = LeftColumn.Width.Value;
                My.Settings.IsLeftColumnCollapsed = false;
                newAngle = 0;
            }
            My.Settings.Save();

            if (CollapseColImg.RenderTransform is RotateTransform rotateTransform)
                rotateTransform.BeginAnimation(RotateTransform.AngleProperty,
                    new DoubleAnimation(newAngle, new(TimeSpan.FromSeconds(.2))));
        }

        private void TrackProgress_Clicked(object sender, RoutedEventArgs e)
        {
            int duration = ViewModel.Track.Duration;
            int newPlayerPosition = (int)(duration * ((ClickedWidthEventArgs)e).WidthPercent);
            App.Current.iApp.PlayerPosition = newPlayerPosition;
        }
    }
}
