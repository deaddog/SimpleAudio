using DeadDog.Audio.AudioDB;
using DeadDog.Audio.Libraries;
using Flurl.Http;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
            if (filepath == null)
            {
                image.Source = null;
                border.Visibility = Visibility.Collapsed;
            }
            else
            {
                var img = new BitmapImage();
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                img.UriSource = filepath;
                img.EndInit();

                image.Source = img;
                border.Visibility = Visibility.Visible;
            }
        }

        public Album Album
        {
            get => GetValue(AlbumProperty) as Album;
            set => SetValue(AlbumProperty, value);
        }
        
        private void HandleDragDrop(object sender, DragEventArgs e)
        {
            string file = DownloadDragData(e);

            if (file != null && Album != null && !Album.IsUnknown)
            {
                Application.Current.Dispatcher.Invoke(() => ChangeImage(null));

                var filepath = GetCoverFilepath(Album);
                File.Copy(file, filepath, true);

                Application.Current.Dispatcher.Invoke(() => ChangeImage(new Uri(filepath)));
            }
        }
        private void HandleDragEnter(object sender, DragEventArgs e)
        {
            string dragData = MapDragData(e);
            if (dragData == null || Album == null || Album.IsUnknown)
                e.Effects = DragDropEffects.None;
        }

        private string MapDragData(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(string)))
                return e.Data.GetData(typeof(string)) as string;

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
        private string DownloadDragData(DragEventArgs e)
        {
            var dragData = MapDragData(e);

            if (dragData == null)
                return null;

            if (Regex.IsMatch(dragData, @"^[a-zA-Z]:\\"))
                return dragData;
            else if (Regex.IsMatch(dragData, @"https?://.*\.(jpg|jpeg|png|gif)", RegexOptions.IgnoreCase))
            {
                var tempPath = Path.GetTempFileName();
                var tempPath2 = Path.GetTempFileName();

                var data = dragData.GetBytesAsync().Result;
                File.WriteAllBytes(tempPath, data);

                try
                {
                    using (var tempImage = System.Drawing.Image.FromFile(tempPath))
                        tempImage.Save(tempPath2, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                catch
                {
                    File.Delete(tempPath2);
                    return null;
                }
                finally
                {
                    File.Delete(tempPath);
                }

                return tempPath2;
            }

            return null;
        }
    }
}
