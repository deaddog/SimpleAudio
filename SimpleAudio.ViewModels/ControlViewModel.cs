using DeadDog.Audio.Libraries;
using DeadDog.Audio.Playback;
using DeadDog.Audio.Playlist;
using Mvvm;
using System;
using System.Windows.Input;

namespace SimpleAudio.ViewModels
{
    public class ControlViewModel : ObservableObject
    {
        private readonly Player<Track> _player;
        private readonly IPlaylist<Track> _playlist;
        private readonly QueuePlaylist<Track> _queue;

        public ControlViewModel(Player<Track> player, IPlaylist<Track> playlist, QueuePlaylist<Track> queue)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _playlist = playlist ?? throw new ArgumentNullException(nameof(playlist));
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
            PlayCommand = new RelayCommand(() => _player.Play());
            PauseCommand = new RelayCommand(() => _player.Pause());
            PlayPauseCommand = new RelayCommand(() => { if (player.Status == PlayerStatus.Playing) player.Pause(); else player.Play(); });
            StopCommand = new RelayCommand(() => _player.Stop());

            PreviousCommand = new RelayCommand(() => _playlist.MovePrevious());
            NextCommand = new RelayCommand(() => _playlist.MoveNext());

            SeekBackwardsCommand = new RelayCommand(() => player.Seek(PlayerSeekOrigin.CurrentBackwards, 5000));
            SeekForwardsCommand = new RelayCommand(() => player.Seek(PlayerSeekOrigin.CurrentForwards, 5000));

            PlayTrack = new RelayCommand<Track>(t =>
            {
                _playlist.MoveToEntry(t);
                if (_player.Status == PlayerStatus.Stopped || _player.Status == PlayerStatus.Paused)
                    _player.Play();
            });
            QueueTrack = new RelayCommand<Track>(t => _queue.Enqueue(t));
        }

        public ICommand PlayCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand PlayPauseCommand { get; }
        public ICommand StopCommand { get; }

        public ICommand PreviousCommand { get; }
        public ICommand NextCommand { get; }

        public ICommand SeekBackwardsCommand { get; }
        public ICommand SeekForwardsCommand { get; }

        public ICommand PlayTrack { get; }
        public ICommand QueueTrack { get; }
    }
}
