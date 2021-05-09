using iTunesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicArt.ViewModels
{
    public class Playlist
    {
        private readonly IITPlaylist iITPlaylist;

        public Playlist(IITPlaylist iITPlaylist) => this.iITPlaylist = iITPlaylist;

        public string Name => iITPlaylist.Name;
        public IITTrackCollection Tracks => iITPlaylist.Tracks;
    }
}
