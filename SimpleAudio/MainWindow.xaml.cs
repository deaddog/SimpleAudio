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

        public MainWindow()
        {
            InitializeComponent();
            scanner = new ScannerBackgroundWorker(
                new DeadDog.Audio.Scan.AudioScanner(new DeadDog.Audio.MediaParser(), @"C:\Users\Mikkel\Music\"));
            scanner.FileParsed += scanner_FileParsed;
            scanner.RunAync();
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
                this.Close();
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
