using DeadDog.Audio.AudioDB;
using DeadDog.Audio.Libraries;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SimpleAudio.Controls
{
    /// <summary>
    /// Interaction logic for AlbumCoverImage.xaml
    /// </summary>
    public partial class AlbumCoverImage : UserControl
    {
        private static readonly MD5 hashing = MD5.Create();
        private readonly IAudioDbRepository _audioDb = new AudioDbRepository();

        public AlbumCoverImage()
        {
            InitializeComponent();
        }

        private static string GetHashString(string input)
        {
            if (input == null || input.Length == 0)
                return null;

            StringBuilder sb = new StringBuilder();

            byte[] hash = hashing.ComputeHash(Encoding.UTF8.GetBytes(input));
            foreach (var b in hash)
                sb.AppendFormat("{0:x2}", b);

            return sb.ToString();
        }
        private static string GetCoverFilepath(Album album)
        {
            var albumTitle = album?.Title.Trim();
            var artistName = album?.Artist?.Name.Trim();

            if (string.IsNullOrEmpty(artistName))
                artistName = null;

            var filename = artistName == null ?
                $"{GetHashString(albumTitle)}.jpg" :
                $"{GetHashString(artistName)}_{GetHashString(albumTitle)}.jpg";

            string directory = Path.Combine(App.ApplicationDataPath, "covers");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return Path.Combine(directory, filename);
        }

        private static readonly DependencyProperty AlbumProperty = DependencyProperty.Register(nameof(Album), typeof(Album), typeof(AlbumCoverImage), new PropertyMetadata(AlbumChanged));

        private static void AlbumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as AlbumCoverImage;
            var album = e.NewValue as Album;

            control.AlbumChanged(album);
        }

        private void AlbumChanged(Album album)
        {
            if (album == null)
            {
                ChangeImage(null);
                return;
            }

            var filepath = GetCoverFilepath(album);
            if (File.Exists(filepath))
                ChangeImage(new Uri(filepath));
            else
                Task.Run(async () =>
                {
                    var albums = (await _audioDb.SearchForAlbum(album))?.Albums;

                    var thumbnail = albums?.FirstOrDefault(x => x.AlbumThumbnail != null).AlbumThumbnail;
                    if (thumbnail != null)
                    {
                        var data = await _audioDb.DownloadFile(thumbnail);
                        File.WriteAllBytes(filepath, data);
                        Application.Current.Dispatcher.Invoke(() => ChangeImage(new Uri(filepath)));
                    }
                });
        }

        private void ChangeImage(Uri filepath)
        {
            if(filepath == null)
            {
                image.Source = null;
                border.Visibility = Visibility.Collapsed;
            }
            else
            {
                image.Source = new BitmapImage(filepath);
                border.Visibility = Visibility.Visible;
            }
        }

        public Album Album
        {
            get => GetValue(AlbumProperty) as Album;
            set => SetValue(AlbumProperty, value);
        }
    }
}
