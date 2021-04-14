using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MusicArt.Resources
{
    public class ArtistAlbumGroupStyleSelector : StyleSelector
    {
        public Style ArtistTemplate { get; set; }
        public Style AlbumTemplate { get; set; }

        public override Style SelectStyle(object item, DependencyObject container) =>
            item is CollectionViewGroup group
                ? group.IsBottomLevel
                    ? AlbumTemplate
                    : ArtistTemplate
                : base.SelectStyle(item, container);
    }
}
