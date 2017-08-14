using DeadDog.Audio.Libraries;
using DeadDog.Audio.Playback;
using PropertyChanged;
using System;

namespace SimpleAudio.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class PlayerViewModel
    {
        private readonly Player<Track> _player;

        public PlayerViewModel(Player<Track> player)
        {
            _player = player;

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

        public PlayerStatus Status { get; private set; }

        public TimeSpan Position { get; private set; }
        public TimeSpan Length { get; private set; }
        public double Progress => Position.TotalSeconds / Length.TotalSeconds;

        public Track CurrentTrack { get; private set; }
    }
}
