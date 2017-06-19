using SimpleAudio.ViewModels;

namespace SimpleAudio
{
    public class ViewModelLocator
    {
        public SearchViewModel SearchViewModel
        {
            get { return new SearchViewModel(); }
        }

        public StatusViewModel StatusViewModel
        {
            get { return new StatusViewModel(); }
        }
    }
}
