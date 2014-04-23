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

namespace SimpleAudio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ScannerBackgroundWorker scanner;
        private Hotkeys.HotKeyManager hotkeys;

        public MainWindow()
        {
            InitializeComponent();
            scanner = new ScannerBackgroundWorker(
                new DeadDog.Audio.Scan.AudioScanner(new DeadDog.Audio.MediaParser(), @"C:\Users\Mikkel\Music\"));
            scanner.FileParsed += scanner_FileParsed;
            scanner.RunAync();

            hotkeys = new Hotkeys.HotKeyManager(this);
            hotkeys.AddHotKey(Key.J, ModifierKeys.Control | ModifierKeys.Alt, () => { this.Show(); textbox.Focus(); textbox.Text = ""; });
        }

        void scanner_FileParsed(object sender, DeadDog.Audio.Scan.ScanFileEventArgs e)
        {
            if (e.State == DeadDog.Audio.Scan.FileState.Added)
                listbox.Items.Add(e.Track);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this.DragMove();
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Hide();

            if (!listbox.IsFocused)
            {
                int s = listbox.SelectedIndex;
                int c = listbox.Items.Count;
                if (e.Key == Key.Down)
                    listbox.SelectedIndex = s < c - 1 ? s + 1 : c - 1;
                else if (e.Key == Key.Up)
                    listbox.SelectedIndex = s > 0 ? s - 1 : 0;
                listbox.ScrollIntoView(listbox.SelectedItem);
            }

            base.OnPreviewKeyDown(e);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = (sender as TextBox).Text.Trim();

            listbox.Items.Filter = 
                track => DeadDog.Audio.Searching.Match((DeadDog.Audio.RawTrack)track, DeadDog.Audio.SearchMethods.ContainsAll, text);
        }
    }
}
