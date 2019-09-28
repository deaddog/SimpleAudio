using DeadDog.Audio.Libraries;
using Mvvm;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace SimpleAudio.ViewModels
{
    public class MediaSearchViewModel : ObservableObject
    {
        private readonly Library _library;

        public MediaSearchViewModel(Library library)
        {
            _library = library ?? throw new System.ArgumentNullException(nameof(library));
            Text = string.Empty;
        }

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                if (Set(ref _text, value ?? throw new ArgumentNullException(nameof(value))))
                    Results = _library.Tracks.Where(t => DeadDog.Audio.Searching.Match(t, DeadDog.Audio.SearchMethods.ContainsAll, value)).ToImmutableList();
            }
        }

        private IImmutableList<Track> _results;
        public IImmutableList<Track> Results
        {
            get => _results;
            private set => Set(ref _results, value);
        }
    }
}
