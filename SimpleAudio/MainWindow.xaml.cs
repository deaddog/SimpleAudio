using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DeadDog.Audio;
using DeadDog.Audio.Playback;
using DeadDog.Audio.Scan;
using DeadDog.Audio.Libraries;
using Hardcodet.Wpf.TaskbarNotification;
using System.IO;
using DeadDog.Audio.Playlist;

namespace SimpleAudio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ScannerBackgroundWorker scanner;
        private Hotkeys.HotKeyManager hotkeys;

        private Library library;
        private LibraryPlaylist playlist;
        private QueuePlaylist<Track> queuePlaylist;
        private Player<Track> player;

        private bool exiting = false;
        private PopupWindow popup;

        public MainWindow()
        {
            InitializeComponent();

            library = new Library();
            playlist = new LibraryPlaylist(library);

            queuePlaylist = new QueuePlaylist<Track>(playlist);

            player = new Player<Track>(queuePlaylist, new FilePlayback<Track>(new AudioControl(), (rt => rt.FilePath)));

            foreach (var path in App.CurrentApp.Settings.Mediapaths)
            {
                scanner = new ScannerBackgroundWorker(path);

                scanner.FileParsed += scanner_FileParsed;
                scanner.RunAync();
            }

            var ctal = ModifierKeys.Control | ModifierKeys.Alt;

            hotkeys = new Hotkeys.HotKeyManager(this);
            hotkeys.AddHotKey(Key.J, ctal, () => { this.Show(); textbox.Focus(); textbox.Text = ""; });
            hotkeys.AddHotKey(Key.Q, ctal, () => { exiting = true; this.Close(); });

            popup = new PopupWindow(player);
            hotkeys.AddHotKey(Key.Space, ctal, () => popup.ShowPopup());

            textbox.Focus();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (exiting)
            {
                popup.Close();
                base.OnClosing(e);
            }
            else
            {
                e.Cancel = true;
                base.OnClosing(e);
                this.Hide();
            }
        }

        void scanner_FileParsed(object sender, ScanFileEventArgs e)
        {
            switch (e.State)
            {
                case FileState.Added:
                    listbox.Items.Add(library.AddTrack(e.Track));
                    break;
                case FileState.Updated:
                    library.UpdateTrack(e.Track);
                    listbox.Items.Refresh();
                    break;

                case FileState.Error:
                case FileState.AddError:
                case FileState.UpdateError:
                case FileState.Skipped:
                    break;

                case FileState.Removed:
                    break;

                default:
                    break;
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this.DragMove();
            base.OnMouseLeftButtonDown(e);
        }

        private bool shiftDown = false;
        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            base.OnPreviewKeyUp(e);
            switch (e.Key)
            {
                case Key.LeftShift:
                case Key.RightShift:
                    shiftDown = false;
                    break;
            }
        }
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    this.Hide();
                    break;

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
                        if (shiftDown)
                        {
                            playlist.MoveToEntry(listbox.SelectedItem as Track);
                            if (player.Status == PlayerStatus.Stopped || player.Status == PlayerStatus.Paused)
                                player.Play();
                        }
                        else
                            queuePlaylist.Enqueue(listbox.SelectedItem as Track);
                    break;

                case Key.LeftShift:
                case Key.RightShift:
                    shiftDown = true;
                    break;

                default:
                    break;
            }

            base.OnPreviewKeyDown(e);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = (sender as TextBox).Text.Trim();

            listbox.Items.Filter =
                track => DeadDog.Audio.Searching.Match((Track)track, DeadDog.Audio.SearchMethods.ContainsAll, text.ToLower());
        }
    }
}
