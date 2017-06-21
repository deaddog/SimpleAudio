using PropertyChanged;

namespace SimpleAudio.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel
    {
        public SearchViewModel Searching { get; }

        public MainViewModel(SearchViewModel searching)
        {
            Searching = searching;
        }
    }
}
