using DeadDog.Audio;
using DeadDog.Audio.Libraries;
using DeadDog.Audio.Playback;
using DeadDog.Audio.Playlist;
using PropertyChanged;
using System;
using System.Windows.Input;

namespace SimpleAudio.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel
    {
        private readonly Library _library;
        private readonly Player<Track> _player;
        private readonly IPlaylist<Track> _playlist;
        private readonly QueuePlaylist<Track> _queue;

        public StatusViewModel Status { get; }

        public ICommand PlayCommand { get; set; }
        public ICommand PauseCommand { get; set; }
        public ICommand PlayPauseCommand { get; set; }
        public ICommand StopCommand { get; set; }

        public ICommand PreviousCommand { get; set; }
        public ICommand NextCommand { get; set; }

        public ICommand SeekBackwardsCommand { get; set; }
        public ICommand SeekForwardsCommand { get; set; }

        public LibraryCollection<Track> Tracks { get; }

        public ICommand PlayTrack { get; }
        public ICommand QueueTrack { get; }

        public MainViewModel(StatusViewModel status, Library library, Player<Track> player, IPlaylist<Track> playlist, QueuePlaylist<Track> queue)
        {
            _library = library;
            _player = player;
            _playlist = playlist;
            _queue = queue;

            Status = status;

            PlayCommand = new Command(() => _player.Play());
            PauseCommand = new Command(() => _player.Pause());
            PlayPauseCommand = new Command(() => { if (player.Status == PlayerStatus.Playing) player.Pause(); else player.Play(); });
            StopCommand = new Command(() => _player.Stop());

            PreviousCommand = new Command(() => _playlist.MovePrevious());
            NextCommand = new Command(() => _playlist.MoveNext());

            SeekBackwardsCommand = new Command(() => player.Seek(TimeSpan.FromSeconds(-5)));
            SeekForwardsCommand = new Command(() => player.Seek(TimeSpan.FromSeconds(5)));

            Tracks = library.Tracks;

            PlayTrack = new Command<Track>(t =>
            {
                _playlist.MoveToEntry(t);
                if (_player.Status == PlayerStatus.Stopped || _player.Status == PlayerStatus.Paused)
                    _player.Play();
            });
            QueueTrack = new Command<Track>(t => _queue.Enqueue(t));
        }
    }
}
