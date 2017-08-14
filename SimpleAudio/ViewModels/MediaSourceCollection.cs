using PropertyChanged;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleAudio.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MediaSourceCollection
    {
        private readonly MediaSourceViewModel[] _sources;
        private readonly Timer _timer;

        public MediaSourceCollection(MediaSourceViewModel.Factory mediaSourceFactory, Settings settings)
        {
            _sources = settings.MediaSources.Select(x => mediaSourceFactory(x)).ToArray();
            _timer = new Timer(_ => ReloadTracks().Wait(), null, TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(30));
        }

        public void LoadCachedTracks()
        {
            foreach (var s in _sources)
                s.LoadCachedTracks();
        }
        private async Task ReloadTracks()
        {
            await Task.WhenAll(_sources.Select(x => x.ReloadTracks()).ToArray()).ConfigureAwait(false);
        }
    }
}
