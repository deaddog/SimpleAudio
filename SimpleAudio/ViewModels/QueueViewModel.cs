using DeadDog.Audio.Libraries;
using DeadDog.Audio.Playlist;
using PropertyChanged;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace SimpleAudio.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class QueueViewModel : IEnumerable<Track>, INotifyCollectionChanged
    {
        private readonly QueuePlaylist<Track> _queue;
        private int _queueSize = 0;

        public QueueViewModel(QueuePlaylist<Track> queue)
        {
            _queue = queue;

            _queue.Enqueued += TrackEnqueued;
            _queue.Dequeued += TrackDequeued;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void TrackEnqueued(Track item)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, _queueSize++));
        }
        private void TrackDequeued(Track item)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, 0));
        }

        public IEnumerator<Track> GetEnumerator()
        {
            foreach (var t in _queue.ToArray())
                yield return t;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _queue.ToArray().GetEnumerator();
        }
    }
}
