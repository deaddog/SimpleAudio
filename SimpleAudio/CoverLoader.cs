using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

        public CoverLoader(int api_key = AUDIO_DB_API_KEY_TEST)
        {
            this.api_key = api_key;
            this.images = new Dictionary<Tuple<string, string>, BitmapImage>();
        }

        public BitmapImage this[string artist, string album]
        {
            get { throw new NotImplementedException(); }
        }

        private void loadFromAudioDb(string artistName, string albumTitle)
        {
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
                "http://www.theaudiodb.com/api/v1/json/{0}/searchalbum.php?a={2}" :
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
