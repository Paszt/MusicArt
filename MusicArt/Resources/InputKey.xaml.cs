using System.Windows;
using System.Windows.Controls;

namespace MusicArt.Resources
{
    /// <summary>
    /// Interaction logic for InputKey.xaml
    /// </summary>
    public partial class InputKey : UserControl
    {
        public InputKey() => InitializeComponent();

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register
            ("Text", typeof(string), typeof(InputKey), new FrameworkPropertyMetadata("Esc"));

        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
    }
}
