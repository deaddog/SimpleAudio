﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SimpleAudio
{
    public class CoverLoader
    {
        private const int AUDIO_DB_API_KEY_TEST = 1;
        private static MD5 hashing = MD5.Create();

        private static string hashString(string input)
        {
            if (input == null || input.Length == 0)
                return null;

            StringBuilder sb = new StringBuilder();

            byte[] hash = hashing.ComputeHash(Encoding.UTF8.GetBytes(input));
            foreach (var b in hash)
                sb.AppendFormat("{0:x2}", b);

            return sb.ToString();
        }

        private Dictionary<Tuple<string, string>, BitmapImage> images;

        private readonly int api_key;
        private readonly Size imageSize;

        public CoverLoader(Size imageSize, int api_key = AUDIO_DB_API_KEY_TEST)
        {
            this.api_key = api_key;
            this.imageSize = imageSize;
            this.images = new Dictionary<Tuple<string, string>, BitmapImage>();
        }

        public BitmapImage this[DeadDog.Audio.Libraries.Album album]
        {
            get
            {
                if (album.IsUnknown)
                    return null;
                if (album.HasArtist && !album.Artist.IsUnknown)
                    return this[album.Artist.Name, album.Title];
                else
                    return this[null, album.Title];
            }
        }
        public BitmapImage this[string artist, string album]
        {
            get
            {
                var key = getKey(artist, album);
                BitmapImage image;

                if (!images.TryGetValue(key, out image))
                {
                    string filepath = getFilePath(artist, album);

                    if (!File.Exists(filepath))
                        loadFromAudioDb(artist, album);

                    image = File.Exists(filepath) ? loadBitmapImageFromFile(filepath) : null;

                    images.Add(key, image);
                }

                return image;
            }
        }

        public BitmapImage LoadLocally(DeadDog.Audio.Libraries.Album album, string filepath)
        {
            var key = getKey(album);
            if (key == null)
                return null;

            string localpath = getFilePath(key.Item1, key.Item2);

            var image = Image.FromFile(filepath);
            image.Save(localpath);
            image.Dispose();

            images[key] = loadBitmapImageFromFile(filepath);

            return images[key];
        }

        private BitmapImage loadBitmapImageFromFile(string filepath)
        {
            var bitmap = new Bitmap(imageSize.Width, imageSize.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (var graphics = Graphics.FromImage(bitmap))
            using (var tempImage = Image.FromFile(filepath))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(tempImage, new Rectangle(0, 0, imageSize.Width, imageSize.Height));
            }

            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();
            stream.Close();
            stream.Dispose();
            bitmap.Dispose();
            return bitmapImage;
        }

        private static Tuple<string, string> getKey(string artist, string album)
        {
            if (artist != null)
            {
                artist = artist.Trim();
                if (artist.Length == 0)
                    artist = null;
            }

            if (album == null)
                throw new ArgumentNullException("album");

            if (album != null) album = album.Trim();
            if (album.Length == 0)
                throw new ArgumentException("Cannot query an empty album name.");

            return Tuple.Create(artist, album);
        }
        private static Tuple<string, string> getKey(DeadDog.Audio.Libraries.Album album)
        {
            if (album.IsUnknown)
                return null;
            if (album.HasArtist && !album.Artist.IsUnknown)
                return Tuple.Create(album.Artist.Name, album.Title);
            else
                return new Tuple<string, string>(null, album.Title);
        }

        private void loadFromAudioDb(string artistName, string albumTitle)
        {
            string albumJson = queryJson(getSearchURL(artistName, albumTitle));

            JArray albums = JObject.Parse(albumJson)["album"] as JArray;
            if (albums != null && albums.Count > 0)
            {
                JObject album = albums[0] as JObject;
                if (album != null)
                {
                    string cover = (album["strAlbumThumb"] as JValue).Value<string>();
                    if (cover != null && cover != "")
                    {
                        downloadFile(cover + "/preview", getFilePath(artistName, albumTitle));
                    }
                }
            }
        }

        private string getFilePath(string artist, string album)
        {
            if (artist != null)
            {
                artist = artist.Trim();
                if (artist.Length == 0)
                    artist = null;
            }

            if (album == null)
                throw new ArgumentNullException("album");

            if (album != null) album = album.Trim();
            if (album.Length == 0)
                throw new ArgumentException("Cannot query an empty album name.");

            string format = artist == null ?
                "{1}.jpg" :
                "{0}_{1}.jpg";

            string filename = string.Format(format, hashString(artist), hashString(album));
            string directory = Path.Combine(App.ApplicationDataPath, "covers");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return Path.Combine(directory, filename);
        }
        private string getSearchURL(string artist, string album)
        {
            if (artist != null)
            {
                artist = artist.Trim();
                if (artist.Length == 0)
                    artist = null;
            }

            if (album == null)
                throw new ArgumentNullException("album");

            if (album != null) album = album.Trim();
            if (album.Length == 0)
                throw new ArgumentException("Cannot query an empty album name.");

            string format = artist == null ?
                "http://www.theaudiodb.com/api/v1/json/{0}/searchalbum.php?s=Various%20Artists&a={2}" :
                "http://www.theaudiodb.com/api/v1/json/{0}/searchalbum.php?s={1}&a={2}";

            return string.Format(format, api_key, artist, album);
        }

        private string queryJson(string url)
        {
            string result;
            using (MemoryStream stream = new MemoryStream())
            {
                queryURL(url, stream);
                result = Encoding.UTF8.GetString(stream.GetBuffer());
            }
            return result;
        }
        private void downloadFile(string url, string filepath)
        {
            using (FileStream fs = new FileStream(filepath, FileMode.Create))
                queryURL(url, fs);
        }

        private void queryURL(string url, Stream stream)
        {
            byte[] buf = new byte[8192];

            HttpWebRequest request;
            HttpWebResponse response = null;
            Stream resStream = null;

            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                response = (HttpWebResponse)request.GetResponse();

                resStream = response.GetResponseStream();
                copyStream(resStream, stream);
                var v = response.ContentEncoding;
            }
            catch
            {
                return;
            }
            finally
            {
                if (resStream != null)
                {
                    resStream.Close();
                    resStream.Dispose();
                }
                if (response != null)
                {
                    response.Close();
                    response.Dispose();
                }
            }
        }

        private void copyStream(Stream source, Stream destination)
        {
            byte[] buf = new byte[8192];
            int count = 0;
            do
            {
                count = source.Read(buf, 0, buf.Length);
                if (count > 0)
                    destination.Write(buf, 0, count);
            }
            while (count > 0);
        }
    }
}
