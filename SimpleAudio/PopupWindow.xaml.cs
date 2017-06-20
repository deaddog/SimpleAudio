using DeadDog.Audio;
using DeadDog.Audio.Libraries;
using DeadDog.Audio.Playback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SimpleAudio
{
    /// <summary>
    /// Interaction logic for PopupWindow.xaml
    /// </summary>
    public partial class PopupWindow : Window
    {
        private Player<Track> player = null;
        private Timer alphaTimer = null;
        private DateTime countdownStart;
        private const double WAIT_SEC = 3;
        private const double FADE_SEC = 3;

        private CoverLoader coverLoader;

        public PopupWindow()
        {
            InitializeComponent();

            coverLoader = new CoverLoader(new System.Drawing.Size(100, 100));
        }

        public PopupWindow(Player<Track> player)
            : this()
        {
            this.player = player;
            setTrack(player.Track);

            this.player.TrackChanged += player_TrackChanged;

            TaskbarHider.HideMe(this);
            this.Loaded += PopupWindow_Loaded;

            this.alphaTimer = new Timer(20);
            this.alphaTimer.AutoReset = true;
            this.alphaTimer.Elapsed += alphaTimer_Elapsed;

            this.MouseMove += PopupWindow_MouseMove;
            this.MouseLeave += PopupWindow_MouseLeave;
        }

        void PopupWindow_MouseMove(object sender, MouseEventArgs e)
        {
            alphaTimer.Stop();
            this.Opacity = 1;
        }
        void PopupWindow_MouseLeave(object sender, MouseEventArgs e)
        {
            countdownStart = DateTime.Now.AddSeconds(-WAIT_SEC);
            alphaTimer.Start();
        }

        // Event handlers - named so that they can be removed when closing
        void player_TrackChanged(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() => setTrack(player.Track));
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            alphaTimer.Stop();

            this.player.TrackChanged -= player_TrackChanged;

            base.OnClosing(e);
        }

        private void alphaTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var diff = (e.SignalTime - countdownStart).TotalSeconds;
            diff -= WAIT_SEC;
            if (diff > 0)
            {
                if (diff >= FADE_SEC)
                {
                    alphaTimer.Stop();
                    this.Dispatcher.Invoke(() => this.Hide());
                }
                else
                    this.Dispatcher.Invoke(() => this.Opacity = 1 - (diff / FADE_SEC));
            }
        }

        private void PopupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var s = (System.Windows.Interop.HwndSource)System.Windows.Interop.HwndSource.FromVisual(this);
            s.AddHook(new HwndSourceHook(WndProc));
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_MOUSEACTIVATE = 0x0021;
            const int MA_NOACTIVATE = 3;

            if (msg == WM_MOUSEACTIVATE)
            {
                handled = true;
                return new IntPtr(MA_NOACTIVATE);
            }

            return IntPtr.Zero;
        }

        public void ShowPopup()
        {
            this.Show();

            this.Left = SystemParameters.PrimaryScreenWidth - this.Width;
            this.Top = SystemParameters.WorkArea.Height - this.Height;

            alphaTimer.Stop();
            countdownStart = DateTime.Now;
            alphaTimer.Start();
            this.Opacity = 1;
        }

        private void setTrack(Track track)
        {
            var cover_source = track == null ? null : coverLoader[track.Album];

            if (cover_source == null)
                cover_border.Visibility = System.Windows.Visibility.Collapsed;
            else
                cover_border.Visibility = System.Windows.Visibility.Visible;

            cover.Source = cover_source;
        }

        private void ImagePanel_Drop(object sender, DragEventArgs e)
        {
            string file = getFile(e);

            if (file != null && player.Track != null && player.Track.Album != null && !player.Track.Album.IsUnknown)
            {
                var cover_source = coverLoader.LoadLocally(player.Track.Album, file);

                if (cover_source == null)
                    cover_border.Visibility = System.Windows.Visibility.Collapsed;
                else
                    cover_border.Visibility = System.Windows.Visibility.Visible;

                cover.Source = cover_source;
            }
        }

        private void ImagePanel_DragEnter(object sender, DragEventArgs e)
        {
            alphaTimer.Stop();
            this.Opacity = 1;

            string file = getFile(e);
            if (file == null || player.Track == null || player.Track.Album == null || player.Track.Album.IsUnknown)
                e.Effects = DragDropEffects.None;
        }

        private string getFile(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length != 1)
                    return null;

                string file = files[0];
                if (!System.IO.File.Exists(file))
                    return null;

                return file;
            }
            else
                return null;
        }
    }
}
