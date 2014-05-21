using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SimpleAudio
{
    public class Settings
    {
        ///XML example Format
        /*
         <?xml version="1.0" encoding="UTF-8"?>                     
            <settings>  
		        <media>   
			        <local path="C:\Users\Stefan\Downloads"/>                                                
			        <local path="C:\Users\Stefan\Music"/>                                       
		        </media>
            </settings>
         */

        private List<String> mediapaths;

        public IEnumerable<string> Mediapaths
        {
            get
            {
                foreach (var s in mediapaths)
                    yield return s;
            }
        }

        public Settings()
        {
            mediapaths = new List<string>();
        }

        public void AddMediaPaths(IEnumerable<string> mediapaths)
        {
            this.mediapaths.AddRange(mediapaths);
        }

        /// <summary>
        /// Load settings from an xmlfile into the application
        /// </summary>
        /// <param name="path">Path to the settings file</param>
        /// <returns></returns>
        public static Settings LoadSettings(string path)
        {
            Settings settings = new Settings();

            if (!File.Exists(path))
                throw new FileNotFoundException("No settings.xml found");

            using (FileStream filestream = File.OpenRead(path))
            {
                XmlReaderSettings s = new XmlReaderSettings();

                using (XmlReader reader = XmlReader.Create(filestream, s))
                {
                    reader.ReadToFollowing("media");
                    XmlReader inner = reader.ReadSubtree();
                    settings.AddMediaPaths(ReadMedia(inner));
                }
            }

            return settings;
        }
        private static IEnumerable<string> ReadMedia(XmlReader media)
        {
            while (media.ReadToFollowing("local"))
            {
                media.MoveToFirstAttribute();
                yield return media.ReadContentAsString();
            }
        }
    }
}
