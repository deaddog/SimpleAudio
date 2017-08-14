using DeadDog.Audio.Libraries;
using DeadDog.Audio.Scan;
using PropertyChanged;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SimpleAudio.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MediaSourceViewModel
    {
        private readonly Library _library;
        private readonly MediaSource _mediaSource;

        private readonly IProgress<ScanProgress> _libraryProgress;

        private readonly MD5 _md5 = MD5.Create();
        private string SourceCacheFilepath
        {
            get
            {
                var directory = Path.Combine(App.ApplicationDataPath, "sources");

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                return Path.Combine(directory, _md5.GetHashString(_mediaSource.Path) + ".cache");
            }
        }

        public delegate MediaSourceViewModel Factory(MediaSource mediaSource);

        public MediaSourceViewModel(Library library, MediaSource mediaSource)
        {
            _library = library;
            _mediaSource = mediaSource;

            _libraryProgress = new LibraryUpdateProgress(_library);
        }

        public void LoadCachedTracks()
        {
            var scanSettings = new ScannerSettings(_mediaSource.Path, SearchOption.AllDirectories, SourceCacheFilepath)
            {
                IncludeFileUpdates = false,
                IncludeNewFiles = false,
                RemoveMissingFiles = false
            };

            var res = AudioScanner.ScanDirectory(scanSettings).Result;

            foreach (var file in res.Where(x => x.Action == FileActions.Skipped))
                _library.AddTrack(file.MediaInfo);
        }
        public async Task ReloadTracks()
        {
            var scanSettings = new ScannerSettings(_mediaSource.Path, SearchOption.AllDirectories, SourceCacheFilepath)
            {
                IncludeFileUpdates = true,
                IncludeNewFiles = true,
                RemoveMissingFiles = true
            };

            await AudioScanner.ScanDirectory(scanSettings, _libraryProgress).ConfigureAwait(false);
        }
    }
}
