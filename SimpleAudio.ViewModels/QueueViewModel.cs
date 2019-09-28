using DeadDog.Audio.Libraries;
using DeadDog.Audio.Playlist;
using Mvvm;
using System;
using System.Collections.Immutable;

namespace SimpleAudio.ViewModels
{
    public class QueueViewModel : ObservableObject
    {
        private readonly QueuePlaylist<Track> _queue;

        public QueueViewModel(QueuePlaylist<Track> queue)
        {
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));

            _queue.QueueChanged += newQueue => _tracks = newQueue.ToImmutableList();
        }

        private IImmutableList<Track> _tracks;
        public IImmutableList<Track> Tracks
        {
            get => _tracks;
            private set => Set(ref _tracks, value);
        }
    }
}
