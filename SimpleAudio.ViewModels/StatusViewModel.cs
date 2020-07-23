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

            _player.StatusChanged += PlayerStatusChanged;
            _player.PositionChanged += PlayerPositionChanged;
            _player.TrackChanged += PlayerTrackChanged;
        }
        ~StatusViewModel()
        {
            _player.StatusChanged -= PlayerStatusChanged;
            _player.PositionChanged -= PlayerPositionChanged;
            _player.TrackChanged -= PlayerTrackChanged;
        }

        private void PlayerStatusChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(Status));
        }
        private void PlayerPositionChanged(object sender, PositionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Position));
            RaisePropertyChanged(nameof(Progress));
        }
        private void PlayerTrackChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(CurrentTrack));
            RaisePropertyChanged(nameof(Length));
            RaisePropertyChanged(nameof(Progress));
        }

        public PlayerStatus Status => _player.Status;
        public Track CurrentTrack => _player.Track;

        public TimeSpan Length => _player.Length;
        public TimeSpan Position => _player.Position;

        public double Progress => Position.TotalSeconds / Length.TotalSeconds;
    }
}
