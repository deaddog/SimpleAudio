using Mvvm;

namespace SimpleAudio.ViewModels
{
    public class PlayerViewModel : ObservableObject
    {
        public PlayerViewModel(StatusViewModel status, QueueViewModel queue)
        {
            Status = status ?? throw new System.ArgumentNullException(nameof(status));
            Queue = queue ?? throw new System.ArgumentNullException(nameof(queue));
        }

        public StatusViewModel Status { get; }
        public QueueViewModel Queue { get; }
    }
}
