using DeadDog.Audio;
using DeadDog.Audio.Libraries;
using DeadDog.Audio.Playback;
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
using System.Windows.Shapes;

namespace SimpleAudio
{
    /// <summary>
    /// Interaction logic for PopupWindow.xaml
    /// </summary>
    public partial class PopupWindow : Window
    {
        private Player<Track> player = null;

        public PopupWindow()
        {
            InitializeComponent();
        }

        public PopupWindow(Player<Track> player)
            : this()
        {
            this.player = player;
            setTrack(player.Track);

            this.player.TrackChanged += (s, e) => this.Dispatcher.Invoke(() => setTrack(player.Track));
            this.player.StatusChanged += (s, e) => this.Dispatcher.Invoke(player_StatusChanged);
            this.player.PositionChanged += (s, e) => this.Dispatcher.Invoke(player_PositionChanged);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            this.Left = SystemParameters.PrimaryScreenWidth - this.Width;
            this.Top = SystemParameters.WorkArea.Height - this.Height;
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
        }

        private void player_PositionChanged()
        {
            if (!this.IsVisible)
                return;

            var p = player.PercentPlayed;

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
