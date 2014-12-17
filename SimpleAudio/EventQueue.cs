using DeadDog.Audio;
using DeadDog.Audio.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAudio
{
    public class EventQueue : IQueue<Track>
    {
        private Queue<Track> myqueue;
        public EventQueue()
        {
            this.myqueue = new Queue<Track>();
        }

        public int Count
        {
            get { return myqueue.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public event EventHandler<QueuedEventArgs> Enqueued;
        public event EventHandler<QueuedEventArgs> Dequeued;

        public void Enqueue(Track entry)
        {
            myqueue.Enqueue(entry);
            if (Enqueued != null)
                Enqueued(this, new QueuedEventArgs(entry));
        }
        public Track Dequeue()
        {
            var entry = myqueue.Dequeue();
            if (Dequeued != null)
                Dequeued(this, new QueuedEventArgs(entry));
            return entry;
        }

        public void Clear()
        {
            if (Dequeued != null)
                foreach (var c in myqueue)
                    Dequeued(this, new QueuedEventArgs(c));
            myqueue.Clear();
        }

        public bool Contains(Track item)
        {
            return myqueue.Contains(item);
        }

        public void CopyTo(Track[] array, int arrayIndex)
        {
            myqueue.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Track> GetEnumerator()
        {
            return myqueue.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return myqueue.GetEnumerator();
        }

        public class QueuedEventArgs : EventArgs
        {
            private Track track;

            public QueuedEventArgs(Track track)
            {
                this.track = track;
            }

            public Track Track
            {
                get { return track; }
            }
        }
    }
}
