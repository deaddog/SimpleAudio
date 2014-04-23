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
        private Player<Track> player;

        public MainWindow()
        {
            InitializeComponent();

            library = new Library();
            playlist = new LibraryPlaylist(library);
            player = new Player<Track>(playlist, new AudioControl<Track>(rt => rt.FilePath));

            scanner = new ScannerBackgroundWorker(
                new AudioScanner(new MediaParser(), @"C:\Users\Mikkel\Music\"));
            scanner.FileParsed += scanner_FileParsed;
            scanner.RunAync();

            hotkeys = new Hotkeys.HotKeyManager(this);
            hotkeys.AddHotKey(Key.J, ModifierKeys.Control | ModifierKeys.Alt, () => { this.Show(); textbox.Focus(); textbox.Text = ""; });
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

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    this.Hide();
                    break;

                case Key.Down:
                case Key.Up:
                    if (listbox.IsFocused)
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
