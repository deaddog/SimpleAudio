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

        public PopupWindow()
        {
            InitializeComponent();
        }

        public PopupWindow(Player<Track> player)
            : this()
        {
            this.player = player;
            setTrack(player.Track);

            this.player.TrackChanged += player_TrackChanged;
            this.player.StatusChanged += player_StatusChanged;
            this.player.PositionChanged += player_PositionChanged;

            TaskbarHider.HideMe(this);
            this.Loaded += PopupWindow_Loaded;

            this.alphaTimer = new Timer(20);
            this.alphaTimer.AutoReset = true;
            this.alphaTimer.Elapsed += alphaTimer_Elapsed;
        }

        // Event handlers - named so that they can be removed when closing
        void player_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            this.Dispatcher.Invoke(player_PositionChanged);
        }
        void player_StatusChanged(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(player_StatusChanged);
        }
        void player_TrackChanged(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() => setTrack(player.Track));
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            alphaTimer.Stop();

            this.player.TrackChanged -= player_TrackChanged;
            this.player.StatusChanged -= player_StatusChanged;
            this.player.PositionChanged -= player_PositionChanged;

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
            string cover_source = null;

            if (track == null)
            {
                title.Content = "";
                artist.Content = "";
                album.Content = "";
                time_length.Content = "0:00";
                time_position.Content = "0:00";
            }
            else
            {
                title.Content = track.Title;
                artist.Content = track.Artist.Name;
                album.Content = track.Album.Title + " #" + track.Tracknumber;
                time_length.Content = ToTime(player.Length);
            }

            if (cover_source == null)
                cover.Visibility = System.Windows.Visibility.Collapsed;
            else
            {
                cover.Visibility = System.Windows.Visibility.Visible;
                throw new NotImplementedException();
            }
        }

        private void player_PositionChanged()
        {
            if (!this.IsVisible)
                return;

            var p = 1 - player.PercentPlayed;

            var w = this.ActualWidth * p;
            Thickness th = progress.Margin;
            th.Right = w;

            progress.Margin = th;
            time_position.Content = ToTime(player.Position);
        }

        private string ToTime(uint milliseconds)
        {
            milliseconds /= 1000;
            var s = milliseconds % 60;
            milliseconds -= s;
            milliseconds /= 60;
            var m = milliseconds;
            return string.Format("{0:0}:{1:00}", m, s);
        }

        private void player_StatusChanged()
        {
            switch (player.Status)
            {
                case PlayerStatus.Playing:
                    status.Content = "Playing";
                    break;
                case PlayerStatus.Paused:
                    status.Content = "Paused";
                    break;
                case PlayerStatus.Stopped:
                case PlayerStatus.NoFileOpen:
                    status.Content = "Stopped";
                    break;
            }
        }
    }
}
