using DeadDog.Audio;
using DeadDog.Audio.Libraries;
using DeadDog.Audio.Playback;
using DeadDog.Audio.Playlist;
using SimpleAudio.ViewModels;
using System;

namespace SimpleAudio
{
    public class DesignViewModels
    {
        private Library GetLibrary()
        {
            var library = new Library();

            library.AddTrack(new RawTrack(@"C:\song.mp3", "Atlas, Rise!", "Hardwired... to Self-Destruct", 2, "Metallica", 2016));

            return library;
        }

        public MainViewModel MainViewModel => new MainViewModel(StatusViewModel, GetLibrary(), null, null, null);

        public StatusViewModel StatusViewModel
        {
            get
            {
                var library = GetLibrary();
                var playlist = new LibraryPlaylist(library);

                var player = new Player<Track>(playlist, new DesignPlayback());
                var vm = new StatusViewModel(new PlayerViewModel(player));

                playlist.MoveToEntry(library.Tracks[0]);
                player.Play();

                return vm;
            }
        }

        #region Designer classes

        private class DesignPlayback : IPlayback<Track>
        {
            public PlayerStatus Status => PlayerStatus.Playing;

            public uint Position => (1 * 60 + 14) * 1000;
            public uint Length => (5 * 60 + 36) * 1000;

            public event EventHandler StatusChanged;
            public event PositionChangedEventHandler PositionChanged;

            public bool CanOpen(Track element)
            {
                return true;
            }

            public bool Close()
            {
                return true;
            }

            public void Dispose()
            {
            }

            public void GetVolume(out double left, out double right)
            {
                throw new NotImplementedException();
            }
            public void SetVolume(double left, double right)
            {
                throw new NotImplementedException();
            }

            public bool Open(Track element)
            {
                return true;
            }

            public bool Pause()
            {
                return true;
            }

            public bool Play()
            {
                StatusChanged?.Invoke(this, EventArgs.Empty);
                PositionChanged?.Invoke(this, new PositionChangedEventArgs(false));
                return true;
            }

            public bool Seek(PlayerSeekOrigin origin, uint offset)
            {
                return true;
            }

            public bool Stop()
            {
                return true;
            }
        }

        #endregion
    }
}
