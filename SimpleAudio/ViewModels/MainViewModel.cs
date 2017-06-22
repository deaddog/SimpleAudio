using DeadDog.Audio;
using DeadDog.Audio.Libraries;
using DeadDog.Audio.Playback;
using PropertyChanged;
using System.Windows.Input;

namespace SimpleAudio.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel
    {
        private readonly Player<Track> _player;
        private readonly IPlaylist<Track> _playlist;

        public SearchViewModel Searching { get; }

        public ICommand PlayCommand { get; set; }
        public ICommand PauseCommand { get; set; }
        public ICommand PlayPauseCommand { get; set; }
        public ICommand StopCommand { get; set; }

        public ICommand PreviousCommand { get; set; }
        public ICommand NextCommand { get; set; }

        public ICommand SeekBackwardsCommand { get; set; }
        public ICommand SeekForwardsCommand { get; set; }

        public MainViewModel(SearchViewModel searching, Player<Track> player, IPlaylist<Track> playlist)
        {
            _player = player;
            _playlist = playlist;
            Searching = searching;

            PlayCommand = new Command(() => _player.Play());
            PauseCommand = new Command(() => _player.Pause());
            PlayPauseCommand = new Command(() => { if (player.Status == PlayerStatus.Playing) player.Pause(); else player.Play(); });
            StopCommand = new Command(() => _player.Stop());

            PreviousCommand = new Command(() => _playlist.MovePrevious());
            NextCommand = new Command(() => _playlist.MoveNext());

            SeekBackwardsCommand = new Command(() => player.Seek(PlayerSeekOrigin.CurrentBackwards, 5000));
            SeekForwardsCommand = new Command(() => player.Seek(PlayerSeekOrigin.CurrentForwards, 5000));
        }
    }
}
