using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAudio
{
    public class Settings
    {
        List<String> mediapaths;

        public List<String> Mediapaths{get{return mediapaths;}}

        public Settings()
        {
            mediapaths = new List<string>();
        }

        public void AddMediaPaths(IEnumerable<string> mediapaths)
        {
            this.mediapaths.AddRange(mediapaths);
        }
    }
}
