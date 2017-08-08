using System.Collections.Generic;

namespace SimpleAudio
{
    public class Settings
    {
        public List<MediaSource> MediaSources { get; }

        public Settings(IEnumerable<MediaSource> mediaSources)
        {
            MediaSources = new List<MediaSource>(mediaSources);
        }
    }

    public class MediaSource
    {
        public string Path { get; }

        public MediaSource(string path)
        {
            Path = path;
        }
    }
}
