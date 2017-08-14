using PropertyChanged;

namespace SimpleAudio.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class StatusViewModel
    {
        public PlayerViewModel Player { get; }

        public StatusViewModel(PlayerViewModel player)
        {
            Player = player;
        }
    }
}
