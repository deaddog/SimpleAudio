using DeadDog.Audio.Libraries;
using DeadDog.Audio.Playback;
using Mvvm;
using System;

namespace SimpleAudio.ViewModels
{
    public class StatusViewModel : ObservableObject
    {
        private readonly Player<Track> _player;

        public StatusViewModel(Player<Track> player)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));

            _player.StatusChanged += (s, e) => Status = _player.Status;
            _player.PositionChanged += (s, e) => Position = TimeSpan.FromMilliseconds(_player.Position);

            _player.TrackChanged += (s, e) =>
            {
                CurrentTrack = _player.Track;
                Length = TimeSpan.FromMilliseconds(_player.Length);
            };

            Status = _player.Status;
            Position = TimeSpan.FromMilliseconds(_player.Position);
            CurrentTrack = _player.Track;
            Length = TimeSpan.FromMilliseconds(_player.Length);
        }

        private PlayerStatus _status;
        public PlayerStatus Status
        {
            get => _status;
            private set => Set(ref _status, value);
        }

        private TimeSpan _position;
        public TimeSpan Position
        {
            get => _position;
            private set { if (Set(ref _position, value)) RaisePropertyChanged(nameof(Progress)); }
        }
        private TimeSpan _length;
        public TimeSpan Length
        {
            get => _length;
            private set { if (Set(ref _length, value)) RaisePropertyChanged(nameof(Progress)); }
        }
        public double Progress => Position.TotalSeconds / Length.TotalSeconds;

        private Track _currentTrack;
        public Track CurrentTrack
        {
            get => _currentTrack;
            private set => Set(ref _currentTrack, value);
        }
    }
}
