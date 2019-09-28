using PropertyChanged;

namespace SimpleAudio.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class OldStatusViewModel
    {
        public OldPlayerViewModel Player { get; }

        public OldStatusViewModel(OldPlayerViewModel player)
        {
            Player = player;
        }
    }
}
