namespace SimpleAudio.ViewModels
{
    public class StatusViewModel
    {
        public PlayerViewModel Player { get; }

        public StatusViewModel(PlayerViewModel player)
        {
            Player = player;
        }
    }
}
