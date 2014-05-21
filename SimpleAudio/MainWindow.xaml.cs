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

namespace SimpleAudio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ScannerBackgroundWorker scanner;
        private Hotkeys.HotKeyManager hotkeys;
        private TaskbarIcon icon;

        private Library library;
        private LibraryPlaylist playlist;
        private Queue<Track> queue;
        private QueuePlaylist<Track> queuePlaylist;
        private Player<Track> player;

        public MainWindow()
        {
            InitializeComponent();

            library = new Library();
            playlist = new LibraryPlaylist(library);

            queue = new Queue<Track>();
            queuePlaylist = new QueuePlaylist<Track>(queue, playlist);

            player = new Player<Track>(queuePlaylist, new AudioControl<Track>(rt => rt.FilePath));
            player.StatusChanged += player_StatusChanged;

            Settings s;
            FileInfo file = new FileInfo(Properties.Settings.Default.settingsfile);
            if (file.Exists)
                s = Settings.LoadSettings(file.FullName);
            else
                s = new Settings();

            foreach (var path in s.Mediapaths)
            {
                scanner = new ScannerBackgroundWorker(
                new AudioScanner(new MediaParser(), path));

                scanner.FileParsed += scanner_FileParsed;
                scanner.RunAync();
            }

            icon = new TaskbarIcon();
            icon.Icon = Properties.Resources.headset;

            var ctal = ModifierKeys.Control | ModifierKeys.Alt;

            hotkeys = new Hotkeys.HotKeyManager(this);
            hotkeys.AddHotKey(Key.J, ctal, () => { this.Show(); textbox.Focus(); textbox.Text = ""; });
            hotkeys.AddHotKey(Key.Q, ctal, () => this.Close());
            hotkeys.AddHotKey(Key.Insert, ctal, () => player.Play());
            hotkeys.AddHotKey(Key.Home, ctal, () => player.Pause());
            hotkeys.AddHotKey(Key.End, ctal, () => player.Stop());
            hotkeys.AddHotKey(Key.PageUp, ctal, () => playlist.MovePrevious());
            hotkeys.AddHotKey(Key.PageDown, ctal, () => queuePlaylist.MoveNext());
            hotkeys.AddHotKey(Key.Right, ctal, () => player.Seek(PlayerSeekOrigin.CurrentForwards, 5000));
            hotkeys.AddHotKey(Key.Left, ctal, () => player.Seek(PlayerSeekOrigin.CurrentBackwards, 5000));

            textbox.Focus();
        }

        private void player_StatusChanged(object sender, EventArgs e)
        {
            switch (player.Status)
            {
                case PlayerStatus.Playing:
                    icon.Icon = Properties.Resources.headset_play;
                    break;
                case PlayerStatus.Paused:
                    icon.Icon = Properties.Resources.headset_pause;
                    break;
                case PlayerStatus.Stopped:
                    icon.Icon = Properties.Resources.headset_stop;
                    break;
                case PlayerStatus.NoFileOpen:
                default:
                    icon.Icon = Properties.Resources.headset;
                    break;
            }
        }

        void scanner_FileParsed(object sender, ScanFileEventArgs e)
        {
            listbox.Items.Add(library.AddTrack(e.Track));
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
                            if (player.Status == PlayerStatus.Stopped)
                                player.Play();
                        }
                        else
                            queue.Enqueue(listbox.SelectedItem as Track);
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
