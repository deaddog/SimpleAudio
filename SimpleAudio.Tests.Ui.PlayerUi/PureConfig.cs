using DeadDog.Audio.Libraries;
using DeadDog.Audio.Playback;
using DeadDog.Audio.Playlist;
using SimpleAudio.ViewModels;

namespace SimpleAudio.Tests.Ui.PlayerUi
{
    public static class PureConfig
    {
        public static PlayerViewModel CreateViewModel()
        {
            var library = new Library();
            var track = library.Add(new DeadDog.Audio.RawTrack("C:\\file.flac", "Good God", "Life is Peachy", 6, "KoRn", 1996));

            var playlist = new Playlist<Track>();
            playlist.Add(track);

            var queueplaylist = new QueuePlaylist<Track>(playlist);

            var player = new Player<Track>(queueplaylist, new Mocks.PlaybackMock());

            return new PlayerViewModel
            (
                new StatusViewModel
                (
                    player
                ),
                new QueueViewModel
                (
                    queueplaylist
                )
            );
        }
    }
}
