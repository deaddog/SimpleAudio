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

        private BitmapImage playImage, pauseImage, stopImage;
        private CoverLoader coverLoader;

        private readonly string queueElement;
        private Queue<Panel> queueUI;

        public PopupWindow()
        {
            InitializeComponent();

            playImage = new BitmapImage();
            playImage.BeginInit();
            playImage.UriSource = new Uri("pack://siteoforigin:,,,/Resources/play.png");
            playImage.EndInit();

            pauseImage = new BitmapImage();
            pauseImage.BeginInit();
            pauseImage.UriSource = new Uri("pack://siteoforigin:,,,/Resources/pause.png");
            pauseImage.EndInit();

            stopImage = new BitmapImage();
            stopImage.BeginInit();
            stopImage.UriSource = new Uri("pack://siteoforigin:,,,/Resources/stop.png");
            stopImage.EndInit();

            coverLoader = new CoverLoader(new System.Drawing.Size(100, 100));

            queueElement = System.Windows.Markup.XamlWriter.Save(QField);
            queueUI = new Queue<Panel>();
        }

        private Panel loadElement(Track track)
        {
            System.IO.StringReader stringReader = new System.IO.StringReader(queueElement);
            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(stringReader);
            Panel newGrid = (Panel)System.Windows.Markup.XamlReader.Load(xmlReader);
            newGrid.Visibility = System.Windows.Visibility.Visible;

            updateElement(newGrid, track);

            (this.Content as Panel).Children.Add(newGrid);

            return newGrid;
        }

        private void updateElement(UIElement e, Track t)
        {
            if (e is Panel)
            {
                foreach (UIElement c in (e as Panel).Children)
                    updateElement(c, t);
            }
            else if (e is Label)
                updateElement(e as Label, t);
            else if (e is Border)
                setCover(e as Border, t);
        }
        private void updateElement(Label l, Track t)
        {
            switch (l.Content as string)
            {
                case "track title": l.Content = t.Title; return;
                case "artist name": l.Content = t.Artist.Name; return;
                case "album title": l.Content = t.Album.Title; return;
                case "track number": l.Content = t.Tracknumber.HasValue ? ("#" + t.Tracknumber.Value) : ""; return;
            }
        }
        private void setCover(Border border, Track track)
        {
            if (!(border.Child is Image))
                return;

            Image c = border.Child as Image;
            var cover_source = track == null ? null : coverLoader[track.Album];

            if (cover_source == null)
                border.Visibility = System.Windows.Visibility.Collapsed;
            else
                border.Visibility = System.Windows.Visibility.Visible;

            c.Source = cover_source;
        }

        public PopupWindow(Player<Track> player, EventQueue queue)
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

            this.MouseMove += PopupWindow_MouseMove;
            this.MouseLeave += PopupWindow_MouseLeave;

            queue.Enqueued += (s, e) =>
                {
                    var el = loadElement(e.Track);
                    queueUI.Enqueue(el);
                    ShowPopup();
                    this.Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Render);
                    this.Top = SystemParameters.WorkArea.Height - this.Height;
                };
            queue.Dequeued += (s, e) =>
                {
                    var el = queueUI.Dequeue();
                    this.Dispatcher.Invoke(() =>
                    {
                        (this.Content as Panel).Children.Remove(el);
                        this.UpdateLayout();
                        this.Top = SystemParameters.WorkArea.Height - this.Height;
                    });
                };
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

            var cover_source = track == null ? null : coverLoader[track.Album];

            if (cover_source == null)
                cover_border.Visibility = System.Windows.Visibility.Collapsed;
            else
                cover_border.Visibility = System.Windows.Visibility.Visible;

            cover.Source = cover_source;
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
                    status_icon.Source = playImage;
                    break;
                case PlayerStatus.Paused:
                    status.Content = "Paused";
                    status_icon.Source = pauseImage;
                    break;
                case PlayerStatus.Stopped:
                case PlayerStatus.NoFileOpen:
                    status.Content = "Stopped";
                    status_icon.Source = stopImage;
                    break;
            }
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
