using DeadDog.Audio.Libraries;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleAudio.Controls
{
    /// <summary>
    /// Interaction logic for TrackSearchControl.xaml
    /// </summary>
    public partial class TrackSearchControl : UserControl
    {
        private bool _shiftDown = false;

        public TrackSearchControl()
        {
            InitializeComponent();
        }

        private static readonly DependencyProperty PlayTrackCommandProperty = DependencyProperty.Register(nameof(PlayTrackCommand), typeof(ICommand), typeof(TrackSearchControl));
        private static readonly DependencyProperty QueueTrackCommandProperty = DependencyProperty.Register(nameof(QueueTrackCommand), typeof(ICommand), typeof(TrackSearchControl));
        private static readonly DependencyProperty TracksProperty = DependencyProperty.Register(nameof(Tracks), typeof(LibraryCollection<Track>), typeof(TrackSearchControl));

        public ICommand PlayTrackCommand
        {
            get => GetValue(PlayTrackCommandProperty) as ICommand;
            set => SetValue(PlayTrackCommandProperty, value);
        }
        public ICommand QueueTrackCommand
        {
            get => GetValue(QueueTrackCommandProperty) as ICommand;
            set => SetValue(QueueTrackCommandProperty, value);
        }
        public LibraryCollection<Track> Tracks
        {
            get => GetValue(TracksProperty) as LibraryCollection<Track>;
            set => SetValue(TracksProperty, value);
        }

        public void ResetSearch()
        {
            textbox.Focus();
            textbox.Text = "";
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = (sender as TextBox).Text.Trim();

            listbox.Items.Filter =
                track => DeadDog.Audio.Searching.Match((Track)track, DeadDog.Audio.SearchMethods.ContainsAll, text.ToLower());
        }
        
        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            base.OnPreviewKeyUp(e);
            switch (e.Key)
            {
                case Key.LeftShift:
                case Key.RightShift:
                    _shiftDown = false;
                    break;
            }
        }
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                case Key.Up:
                    if (!listbox.IsFocused)
                    {
                        int i = listbox.SelectedIndex;
                        int c = listbox.Items.Count;
                        if (e.Key == Key.Down)
                            listbox.SelectedIndex = i < c - 1 ? i + 1 : c - 1;
                        else
                            listbox.SelectedIndex = i > 0 ? i - 1 : 0;
                        listbox.ScrollIntoView(listbox.SelectedItem);
                    }
                    break;

                case Key.Enter:
                    if (listbox.SelectedItem != null)
                        if (_shiftDown)
                            PlayTrackCommand?.Execute(listbox.SelectedItem);
                        else
                            QueueTrackCommand?.Execute(listbox.SelectedItem);
                    break;

                case Key.LeftShift:
                case Key.RightShift:
                    _shiftDown = true;
                    break;

                default:
                    break;
            }

            base.OnPreviewKeyDown(e);
        }
    }
}
