using DeadDog.Audio.Libraries;
using DeadDog.Audio.Playback;
using System;

namespace SimpleAudio.Tests.Ui.PlayerUi.Mocks
{
    public class PlaybackMock : IPlayback<Track>
    {
        public PlaybackMock()
        {
            Status = PlayerStatus.NoFileOpen;
        }

        public PlayerStatus Status { get; private set; }

        public uint Position { get; private set; }
        public uint Length { get; private set; }

        public event EventHandler StatusChanged;
        public event PositionChangedEventHandler PositionChanged;

        public bool CanOpen(Track element)
        {
            return true;
        }
        public bool Open(Track element)
        {
            Status = PlayerStatus.Stopped;
            return true;
        }
        public bool Close()
        {
            Status = PlayerStatus.NoFileOpen;
            return true;
        }

        public bool Pause()
        {
            Status = PlayerStatus.Paused;
            return true;
        }
        public bool Play()
        {
            Status = PlayerStatus.Playing;
            return true;
        }
        public bool Stop()
        {
            Status = PlayerStatus.Stopped;
            return true;
        }

        public bool Seek(PlayerSeekOrigin origin, uint offset)
        {
            Position = offset;
            return true;
        }

        public void GetVolume(out double left, out double right)
        {
            left = 0;right = 0;
        }
        public void SetVolume(double left, double right)
        {
        }

        public void Dispose()
        {
        }
    }
}
