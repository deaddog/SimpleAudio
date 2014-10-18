using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAudio
{
    public class CoverLoader
    {
        private const int AUDIO_DB_API_KEY = 1;

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

            return string.Format(format, AUDIO_DB_API_KEY, artist, album);
        }
    }
}
