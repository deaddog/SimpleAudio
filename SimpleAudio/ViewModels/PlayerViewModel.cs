using DeadDog.Audio.Libraries;
using DeadDog.Audio.Playback;
using Mvvm;
using System;

namespace SimpleAudio.ViewModels
{
    public class PlayerViewModel : ObservableObject
    {
        private readonly Player<Track> _player;

        public PlayerViewModel(Player<Track> player)
        {
            _player = player;

            _player.StatusChanged += (s, e) => Status = _player.Status;
            _player.PositionChanged += (s, e) => Position = _player.Position;

            _player.TrackChanged += (s, e) =>
            {
                CurrentTrack = _player.Track;
                Length = _player.Length;
                Position = TimeSpan.Zero;
            };

            Status = _player.Status;
            Position = _player.Position;
            CurrentTrack = _player.Track;
            Length = _player.Length;
        }

        private PlayerStatus _status;
        public PlayerStatus Status
        {
            get => _status;
            set => Set(ref _status, value);
        }

        private TimeSpan _position;
        private TimeSpan _length;
        public TimeSpan Position
        {
            get => _position;
            set
            {
                if (Set(ref _position, value))
                    RaisePropertyChanged(nameof(Progress));
            }
        }
        public TimeSpan Length
        {
            get => _length;
            set
            {
                if (Set(ref _length, value))
                    RaisePropertyChanged(nameof(Progress));
            }
        }
        public double Progress => Position.TotalSeconds / Length.TotalSeconds;

        private Track _currentTrack;
        public Track CurrentTrack
        {
            get => _currentTrack;
            set => Set(ref _currentTrack, value);
        }
    }
}
