using MusicArt.Resources;
using MusicArt.ViewModels;
using System;
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
        public FullscreenWindow() => InitializeComponent();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Top = SystemParameters.WorkArea.Top;
            Left = SystemParameters.WorkArea.Left;
            Width = SystemParameters.WorkArea.Width;
            Height = SystemParameters.WorkArea.Height;
            UpdateTaskbarThumbnail();
            if (My.Settings.LeftColumnWidth == -1) My.Settings.LeftColumnWidth = LeftColumn.ActualWidth;
            LeftColumn.Width = My.Settings.IsLeftColumnCollapsed ? (new(0)) : (new(My.Settings.LeftColumnWidth));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext != null && DataContext is iTunesViewModel vm) vm.DisconnectFromItunes();
        }

        private void CoverArtImage_SizeChanged(object sender, SizeChangedEventArgs e) => UpdateTaskbarThumbnail();

        private void UpdateTaskbarThumbnail()
        {
            if (DataContext != null && DataContext is iTunesViewModel vm)
            {
                Point wpfPoint = CoverArtImage.TransformToAncestor(this).Transform(new Point(0, 0));
                vm.ThumbnailClipMargin = new(wpfPoint.X, wpfPoint.Y,
                    ActualWidth - CoverArtImage.ActualWidth - wpfPoint.X,
                    ActualHeight - CoverArtImage.ActualHeight - wpfPoint.Y);
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void RestoreButton_Click(object sender, RoutedEventArgs e) => PerformRestore();

        private void PerformRestore()
        {
            if (SystemParameters.WorkArea.Height != SystemParameters.PrimaryScreenHeight)
            {
                // Taskbar is Top or Bottom
                if (Height == SystemParameters.WorkArea.Height)
                {
                    Height = SystemParameters.PrimaryScreenHeight;
                    Top = 0;
                    RestoreImage.Source = (ImageSource)Application.Current.FindResource("RestoreDown");
                }
                else
                {
                    Height = SystemParameters.WorkArea.Height;
                    Top = SystemParameters.WorkArea.Top;
                    RestoreImage.Source = (ImageSource)Application.Current.FindResource("RestoreUp");
                }
            }
            else
            {
                // Taskbar is Left or Right
                if (Width == SystemParameters.WorkArea.Width)
                {
                    Width = SystemParameters.PrimaryScreenWidth;
                    Left = 0;
                    RestoreImage.Source = (ImageSource)Application.Current.FindResource("RestoreDown");
                }
                else
                {
                    Width = SystemParameters.WorkArea.Width;
                    Left = SystemParameters.WorkArea.Left;
                    RestoreImage.Source = (ImageSource)Application.Current.FindResource("RestoreUp");
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.I:
                    TrackInfoGrid.BeginAnimation(OpacityProperty,
                        new DoubleAnimation(0.7, TimeSpan.FromSeconds(0.2)));
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
                case Key.Escape:
                    PerformRestore();
                    break;
                case Key.I:
                    TrackInfoGrid.BeginAnimation(OpacityProperty, new DoubleAnimation(0, TimeSpan.FromSeconds(0.2)));
                    break;
                case Key.Right:
                    if (LeftColumn.Width.Value != My.Settings.LeftColumnWidth) AnimateLeftColumn(ExpandCollapseState.Expanded);
                    break;
                case Key.Left:
                    if (LeftColumn.Width.Value > 20) AnimateLeftColumn(ExpandCollapseState.Collapsed);
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

        private void CollapseExpandLeftColButton_Click(object sender, RoutedEventArgs e)
        {
            ExpandCollapseState resultState = LeftColumn.Width.Value <= 20 ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
            AnimateLeftColumn(resultState);
        }

        private void AnimateLeftColumn(ExpandCollapseState resultState)
        {
            Storyboard storyBoard = new Storyboard();
            double resultWidth = (resultState == ExpandCollapseState.Expanded) ? My.Settings.LeftColumnWidth : 0d;
            GridLengthAnimation animation = new(resultWidth, TimeSpan.FromSeconds(0.2));
            animation.Completed += delegate
            {
                // Set the animation to null on completion. This allows the grid to be resized manually.
                LeftColumn.BeginAnimation(ColumnDefinition.WidthProperty, null);
                // Set the final value manually.
                LeftColumn.Width = new GridLength(resultWidth);
                // Update Settings and button properties to reflect new state.
                UpdateLeftColumnProps();
            };
            storyBoard.Children.Add(animation);
            Storyboard.SetTarget(animation, LeftColumn);
            Storyboard.SetTargetProperty(animation, new PropertyPath(ColumnDefinition.WidthProperty));
            storyBoard.Begin();
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
    }
}
